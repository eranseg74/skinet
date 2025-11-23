using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Config;

// This class is used to configure the Product entity properties in the database. We use the IEntityTypeConfiguration interface to define the configuration for the Product entity. This is a better approach than using Data Annotations directly in the entity class because it keeps the entity class clean and focused on its purpose (representing the data structure) while the configuration details are handled separately.
// Here we configure the Price property to have a specific column type in the database (decimal with precision 18 and scale 2). This is important for financial data to ensure accuracy and avoid rounding issues.
// We can add more configurations here in the future if needed, such as setting up relationships, indexes, default values, maximum length, etc.
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
  public void Configure(EntityTypeBuilder<Product> builder)
  {
    builder.Property(x => x.Price).HasColumnType("decimal(18,2)");
    builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
  }
}
