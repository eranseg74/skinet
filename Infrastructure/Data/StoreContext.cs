using Core.Entities;
using Infrastructure.Config;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

// The parentheses syntax is used here to define a primary constructor. It is like writing a constructor
// that takes DbContextOptions as a parameter and passes it to the base DbContext class.
// We need to pass to the DbContext (through the options which are passed to the base DbContext) the connection string to the SqlServer and also register this as a service in the application (Program.cs). Everything we register as a service can be injected to other classes using the dependency injection mechanism

// public class StoreContext(DbContextOptions options) : DbContext(options)

// The following implements using the identity framework
public class StoreContext(DbContextOptions options) : IdentityDbContext<AppUser>(options)
{
  // Entity Framework will take the Products name and create a table with this name in the database
  public DbSet<Product> Products { get; set; }
  public DbSet<Address> Addresses { get; set; }
  public DbSet<DeliveryMethod> DeliveryMethods { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    // This method is called when the model for a derived context has been initialized, but
    // before the model has been locked down and used to initialize the context. The default
    // implementation of this method does nothing, but it can be overridden in a derived class
    // such that the model can be further configured before it is locked down.
    base.OnModelCreating(modelBuilder);

    // ApplyConfigurationsFromAssembly is a method that applies all the configurations defined in the specified assembly. 
    // This will scan the assembly for any classes that implement IEntityTypeConfiguration<T>
    // and apply their configurations to the model. This is a way to keep the entity configurations separate from the DbContext class, promoting better organization and maintainability of the code.
    // Any new configuration class created in the Infrastructure/Config folder will be automatically applied here without needing to modify this method.
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductConfiguration).Assembly);
  }
}

// Applying configurations from the assembly where the ProductConfiguration is located. This will automatically apply all configurations defined in that assembly without needing to specify each one individually.