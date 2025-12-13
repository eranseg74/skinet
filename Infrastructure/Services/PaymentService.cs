using Core.Entities;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace Infrastructure.Services;

public class PaymentService(IConfiguration configuration, ICartService cartService, IGenericRepository<Core.Entities.Product> productRepo, IGenericRepository<DeliveryMethod> dmRepo) : IPaymentService
{
  public async Task<ShoppingCart?> CreateOrUpdatePaymentIntent(string cartId)
  {
    // Getting the secret key from the appsettings file
    StripeConfiguration.ApiKey = configuration["StripeSettings:SecretKey"];
    // Getting the cart using the cart service according to the cart id.
    var cart = await cartService.GetCartAsync(cartId);
    if (cart == null)
    {
      return null;
    }
    var shippingPrice = 0m; // The m represents a decimal
    if (cart.DeliveryMethodId.HasValue) // We have access to the HasValue method because we declared the DeliveryMethodId as optional
    {
      // Getting the delivery method in order to extract from it the shipping price
      var deliverMethod = await dmRepo.GetByIdAsync((int)cart.DeliveryMethodId);
      if (deliverMethod == null) return null;
      shippingPrice = deliverMethod.Price;
    }
    foreach (var item in cart.Items)
    {
      var productItem = await productRepo.GetByIdAsync(item.ProductId);
      if (productItem == null) return null;
      if (item.Price != productItem.Price)
      {
        item.Price = productItem.Price;
      }
    }
    var service = new PaymentIntentService();
    PaymentIntent? intent = null;
    if (string.IsNullOrEmpty(cart.PaymentIntentId))
    {
      // We need to create options because this is what Stripe expects
      var options = new PaymentIntentCreateOptions
      {
        // Since the Amount expects a long value we need to cast the result which is a decimal. We are multiplying by 100 because decimal here can be with 2 digits on the right side of the decimal point (we defined the decimal to be decimal(18,2))
        Amount = (long)cart.Items.Sum(x => x.Quantity * (x.Price * 100)) + (long)shippingPrice * 100,
        Currency = "usd",
        PaymentMethodTypes = ["card"]
      };
      intent = await service.CreateAsync(options);
      cart.PaymentIntentId = intent.Id;
      cart.ClientSecret = intent.ClientSecret;
    }
    else // In case there is already an intent for this cart. Then we only need to update the amount
    {
      var options = new PaymentIntentUpdateOptions
      {
        Amount = (long)cart.Items.Sum(x => x.Quantity * (x.Price * 100)) + (long)shippingPrice * 100
      };
      intent = await service.UpdateAsync(cart.PaymentIntentId, options); // Updating the existing intent
    }
    await cartService.SetCartAsync(cart);
    return cart;
  }
}
