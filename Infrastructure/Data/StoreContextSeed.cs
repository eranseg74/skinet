using System.Reflection;
using System.Text.Json;
using Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Data;

public class StoreContextSeed
{
  public static async Task SeedAsync(StoreContext context, UserManager<AppUser> userManager)
  {
    if (!userManager.Users.Any(x => x.UserName == "admin@test.com"))
    {
      var user = new AppUser
      {
        UserName = "admin@test.com",
        Email = "admin@test.com",
      };
      await userManager.CreateAsync(user, "Pa$$w0rd");
      await userManager.AddToRoleAsync(user, "Admin");
    }
    // The location returns the full path of the loaded file that contains the manifest. Then,in the productsData initialization, instead of passing a relative address we can write as defined here. This type of path will work both in development and production
    var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    if (!context.Products.Any())
    {
      // Read the JSON file containing the seed data for products. Using the path relative to the project structure. We are specifying the path although we are in the same folder because for later when we will deploy the application.
      var productsData = File.ReadAllText(path + @"/Data/SeedData/products.json");
      var products = JsonSerializer.Deserialize<List<Product>>(productsData);
      if (products != null)
      {
        context.Products.AddRange(products);
        await context.SaveChangesAsync();
      }
    }

    // Seeding data for the Delivery Methods
    if (!context.DeliveryMethods.Any())
    {
      // Read the JSON file containing the seed data for delivery methods. Using the path relative to the project structure. We are specifying the path although we are in the same folder because for later when we will deploy the application.
      var deliveryMethodsData = File.ReadAllText(path + @"/Data/SeedData/delivery.json");
      var deliveryMethods = JsonSerializer.Deserialize<List<DeliveryMethod>>(deliveryMethodsData);
      if (deliveryMethods != null)
      {
        context.DeliveryMethods.AddRange(deliveryMethods);
        await context.SaveChangesAsync();
      }
    }
  }
}
