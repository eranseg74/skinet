namespace Core.Entities;

public class Product : BaseEntity
{
  // Important to have Id named property for EF Core to recognize it as the primary key. If not named Id or <ClassName>Id, it needs to be configured in the DbContext. Or we can add here the [Key] attribute from System.ComponentModel.DataAnnotations.
  // We removed the Id property from here because we want it in all of our entities, so we moved it to the BaseEntity class. This way we avoid duplication. All the entities will inherit from BaseEntity. It contains only the Id property for now, but we can add more common properties in the future if needed.
  public required string Name { get; set; }
  public required string Description { get; set; }
  public decimal Price { get; set; }
  public required string PictureUrl { get; set; }
  public required string Type { get; set; }
  public required string Brand { get; set; }
  public int QuantityInStock { get; set; }
}
