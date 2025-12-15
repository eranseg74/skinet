using API.Extensions;
using API.SignalR;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Stripe;

namespace API.Controllers;

// Injecting the hubContext gives us access to the notification hub so we can send notifications outside the hub using this approach
public class PaymentsController(IPaymentService paymentService, IUnitOfWork unitOfWork, ILogger<PaymentsController> logger, IConfiguration configuration, IHubContext<NotificationHub> hubContext) : BaseApiController
{
  private readonly string _whSecret = configuration["StripeSettings:WhSecret"]!;
  [Authorize]
  [HttpPost("{cartId}")]
  public async Task<ActionResult<ShoppingCart>> CreateOrUpdatePaymentIntent(string cartId)
  {
    var cart = await paymentService.CreateOrUpdatePaymentIntent(cartId);
    if (cart == null) return BadRequest("Problem with your cart");
    return Ok(cart);
  }

  [HttpGet("delivery-methods")]
  public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
  {
    return Ok(await unitOfWork.Repository<DeliveryMethod>().ListAllAsync());
  }

  // This EndPoint needs to be anonymous because it will be called by the payment provider which doesn't have our authentication token but it has its own secret to verify the request.
  // The implementation process here is -
  // 1. A request comes from stripe after the payment is successful (that is the event we are interested in checking).
  // 2. We get the event from the request body
  // 3. Constructing a stripe event using the ConstructStripeEvent method we defined
  // 4. Checking if the stripe event is of type Payment intent. If it is not then we cannot do anything with it so we return a bad request.
  // 5. If it is a payment intent then we handle it as long as it is succeeded
  [HttpPost("webhook")]
  public async Task<IActionResult> StripeWebhook()
  {
    // The ReadToEndAsync method reads all characters from the current position to the end of the stream asynchronously and returns them as one string
    var json = await new StreamReader(Request.Body).ReadToEndAsync();
    try
    {
      var stripeEvent = ConstructStripeEvent(json);
      if (stripeEvent.Data.Object is not PaymentIntent intent)
      {
        return BadRequest("Invalid event data");
      }
      await HandlePaymentIntentSucceeded(intent);
      return Ok();
    }
    catch (StripeException ex)
    {
      logger.LogError(ex, "Stripe webhook error");
      return StatusCode(StatusCodes.Status500InternalServerError, "Stripe webhook error");
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "An unexpected error occurred");
      return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred");
    }
  }

  private async Task HandlePaymentIntentSucceeded(PaymentIntent intent) // Not returning anything. This is just a Task
  {
    Console.WriteLine("Before succeeded check");
    if (intent.Status == "succeeded")
    {
      Console.WriteLine("After succeeded check");
      // Adding true (or false) as the second parameter (it does not matter which value because we are not using this parameter) ensures that we will use the correct constructor
      var spec = new OrderSpecification(intent.Id, true);
      // Specifying the full path in order not to mix it with the Order in the Stripe namespace
      var order = await unitOfWork.Repository<Core.Entities.OrderAggregate.Order>().GetEntityWithSpecAsync(spec) ?? throw new Exception("Order not found");
      // Checking if the total price in the order matches what the user paid in Stripe
      // We multiply the total number by 100 because the last 2 digits are considered as the 2 digits on the right side of the decimal point. For example - total of 12345 is equal to an amount of 123.45
      if ((long)order.GetTotal() * 100 != intent.Amount)
      {
        order.Status = OrderStatus.PaymentMismatch;
      }
      else
      {
        order.Status = OrderStatus.PaymentReceived;
      }
      // Updating the DB
      await unitOfWork.Complete();

      // Notifying the user of the order status using SignalR
      var connectionId = NotificationHub.GetConnectionIdByEmail(order.BuyerEmail);
      if (!string.IsNullOrEmpty(connectionId))
      {
        await hubContext.Clients.Client(connectionId).SendAsync("OrderCompleteNotification", order.ToDto()); // This is the method that we are going to use and listen to on the client side so when this notification is received we can react to that and do something with it. We will use the Dto returned here in the client side for the success message
      }
    }
  }

  private Event ConstructStripeEvent(string json)
  {
    try
    {
      return EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], _whSecret);
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "Failed to construct stripe event");
      throw new StripeException("Invalid signature"); // If we get this error then probably there is something wrong with the WhSecret value
    }
  }
}
