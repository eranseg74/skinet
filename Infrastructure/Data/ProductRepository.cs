using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

// After defining the interface and the implementation class, we need to register the repository in the dependency injection container. This is typically done in the Startup.cs or Program.cs file of the ASP.NET Core application. By registering the repository, we enable the application to inject the IProductRepository interface wherever it is needed, allowing for loose coupling and easier testing.
public class ProductRepository(StoreContext context) : IProductRepository
{
  public void AddProduct(Product product)
  {
    context.Products.Add(product);
  }

  public void DeleteProduct(Product product)
  {
    context.Products.Remove(product);
  }

  public async Task<IReadOnlyList<string>> GetBrandsAsync()
  {
    return await context.Products.Select(x => x.Brand).Distinct().ToListAsync(); // The Distinct will remove all repititions from the returned list
  }

  public async Task<Product?> GetProductByIdAsync(int id)
  {
    return await context.Products.FindAsync(id);
  }

  public async Task<IReadOnlyList<Product>> GetProductsAsync(string? brand, string? type, string? sort)
  {
    var query = context.Products.AsQueryable(); // AsQueryable allows us to build the query dynamically based on the provided parameters
    if (!string.IsNullOrWhiteSpace(brand)) // Check if brand parameter is provided. the IsNullOrWhiteSpace method checks if the string is null, empty, or consists only of white-space characters.
    {
      query = query.Where(x => x.Brand == brand);
    }
    if (!string.IsNullOrWhiteSpace(type))
    {
      query = query.Where(x => x.Type == type);
    }
    // Not using the IsNullOrWhiteSpace check to allow default sorting by name when no sort parameter is provided
    // if (!string.IsNullOrWhiteSpace(sort))
    // {
    query = sort switch
    {
      "priceAsc" => query.OrderBy(x => x.Price),
      "priceDesc" => query.OrderByDescending(x => x.Price),
      _ => query.OrderBy(x => x.Name) // Default sorting by Name
    };
    // }
    return await query.ToListAsync();
  }

  public async Task<IReadOnlyList<string>> GetTypesAsync()
  {
    return await context.Products.Select(x => x.Type).Distinct().ToListAsync();
  }

  public bool ProductExists(int id)
  {
    return context.Products.Any(x => x.Id == id);
  }

  public async Task<bool> SaveChangesAsync()
  {
    return await context.SaveChangesAsync() > 0;
  }

  public void UpdateProduct(Product product)
  {
    // Marks the entity for update. This means that the entity is tracked by the entity framework and will be updated in the database when SaveChangesAsync is called.
    context.Entry(product).State = EntityState.Modified;
  }

}
