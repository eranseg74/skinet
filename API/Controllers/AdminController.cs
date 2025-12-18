using API.DTOs;
using API.Extensions;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController(IUnitOfWork unitOfWork, IPaymentService paymentService) : BaseApiController
{
  [HttpGet("orders")]
  public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetOrders([FromQuery] OrderSpecParams specParams)
  {
    var spec = new OrderSpecification(specParams);
    return await CreatePagedResult(unitOfWork.Repository<Order>(), spec, specParams.PageIndex, specParams.PageSize, o => o.ToDto());
    // This will also work, but we prefer to keep the pagination functionality in the BaseApiController for consistency
    /*
    var orders = await unitOfWork.Repository<Order>().ListAsync(spec);
    var ordersToReturn = orders.Select(o => o.ToDto()).ToList();
    return Ok(ordersToReturn);
    */
  }

  [HttpGet("orders/{id:int}")]
  public async Task<ActionResult<OrderDto>> GetOrderById(int id)
  {
    var spec = new OrderSpecification(id);
    var order = await unitOfWork.Repository<Order>().GetEntityWithSpecAsync(spec);
    if (order == null) return BadRequest("No order with the specified Id was found");
    return order.ToDto();
  }

  [HttpPost("orders/refund/{id:int}")]
  public async Task<ActionResult<OrderDto>> RefundOrder(int id)
  {
    var spec = new OrderSpecification(id);
    var order = await unitOfWork.Repository<Order>().GetEntityWithSpecAsync(spec);
    if (order == null) return BadRequest("No order with that id");
    if (order.Status == OrderStatus.Pending)
    {
      return BadRequest("Payment not received for this order");
    }
    var result = await paymentService.RefundPayment(order.PaymentIntentId);
    if (result == "succeeded")
    {
      order.Status = OrderStatus.Refunded;
      await unitOfWork.Complete();
      return order.ToDto();
    }
    return BadRequest("Problem refunding order");
  }
}
