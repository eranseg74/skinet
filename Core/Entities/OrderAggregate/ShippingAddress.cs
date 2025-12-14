namespace Core.Entities.OrderAggregate;

// This entity represents the shipping address for an order. It will not be mapped to a separate table in the database, but rather will be stored as part of the Order entity. THe Order entity will have a property of type ShippingAddress to hold the shipping address details (own this class). This is why this class does not contain an Id property and does not inherit from the BaseEntity class.
public class ShippingAddress
{
  public required string Name { get; set; }
  public required string Line1 { get; set; }
  public string? Line2 { get; set; }
  public required string City { get; set; }
  public string? State { get; set; }
  public required string PostalCode { get; set; }
  public required string Country { get; set; }
}
