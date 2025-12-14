using Core.Entities.OrderAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Config;

// Aclass for configuring the Order entity
public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
  // Configuring the Order entity
  public void Configure(EntityTypeBuilder<Order> builder)
  {
    // This specifies that the Order owns the ShippingAddress. The OwnsOne configures a relationship where the target entity is owned by (or part of) this entity.
    builder.OwnsOne(x => x.ShippingAddress, o => o.WithOwner());
    builder.OwnsOne(x => x.PaymentSummary, o => o.WithOwner());
    builder.Property(x => x.Status).HasConversion(
      o => o.ToString(),
      o => Enum.Parse<OrderStatus>(o) // Insteadof o => (OrderStatus)Enum.Parse(typeof(OrderStatus), o)
    );
    builder.Property(x => x.Subtotal).HasColumnType("decimal(18,2)");
    // Specifying that one order may have many order items, with the Delete behavior as cascade, which means that deleting an order will delete all of its order items
    builder.HasMany(x => x.OrderItems).WithOne().OnDelete(DeleteBehavior.Cascade);
    // Specifying the Date conversion because SQLServer does not save UTC format although we are sending it to the DB in a UTC format, so we need to make sure to treat what we get from SQL as UTC:
    builder.Property(x => x.OrderDate).HasConversion(
      d => d.ToUniversalTime(),
      d => DateTime.SpecifyKind(d, DateTimeKind.Utc)
    );
  }
}
