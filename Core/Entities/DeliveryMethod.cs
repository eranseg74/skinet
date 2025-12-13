namespace Core.Entities;

public class DeliveryMethod : BaseEntity
{
  public required string ShortName { get; set; }
  public required string DeliveryTime { get; set; }
  public required string Description { get; set; }
  public decimal Price { get; set; } // The decimal type here requires a configuration which is implemented in the DeliveryMethdConfiguration file under the Infrastructure/Config folder
}
