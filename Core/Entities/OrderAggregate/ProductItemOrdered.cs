namespace Core.Entities.OrderAggregate;

// This is used for saving history - what products were ordered. We don't want to have a direct relationship to the Product entity because the product details (name, pictureUrl) might change over time. We want to keep the details as they were at the time of the order. This class will be used as a complex type within the OrderItem entity (Will be owned by the OrderItem entity). The ProductItemOrdered class does not have an Id property and does not inherit from BaseEntity because it will not be mapped to a separate table in the database.
public class ProductItemOrdered
{
  // The id of the product that was ordered. This is not a foreign key relationship, just a simple property to hold the product id.
  public int ProductId { get; set; }
  public required string ProductName { get; set; }
  public required string PictureUrl { get; set; }
}
