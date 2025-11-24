using System.Text.Json;
using Core.Entities;

namespace Infrastructure.Data;

public class StoreContextSeed
{
  public static async Task SeedAsync(StoreContext context)
  {
    if (!context.Products.Any())
    {
      // Read the JSON file containing the seed data for products. Using the path relative to the project structure. We are specifying the path although we are in the same folder because for later when we will deploy the application.
      var productsData = File.ReadAllText("../Infrastructure/Data/SeedData/products.json");
      var products = JsonSerializer.Deserialize<List<Product>>(productsData);
      if (products != null)
      {
        context.Products.AddRange(products);
        await context.SaveChangesAsync();
      }
    }
  }
}
