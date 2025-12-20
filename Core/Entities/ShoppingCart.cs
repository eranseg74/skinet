namespace Core.Entities;

public class ShoppingCart
{
  public required string Id { get; set; }
  public List<CartItem> Items { get; set; } = [];

  // The following 3 properties are optional because we won't have them on the cart initialization
  public int? DeliveryMethodId { get; set; }
  public string? ClientSecret { get; set; }
  public string? PaymentIntentId { get; set; }
  public AppCoupon? Coupon { get; set; }
}
