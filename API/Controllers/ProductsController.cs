using System;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

// The Controllers in ASP.NET Core are classes that handle incoming HTTP requests and send back HTTP responses to the client. They are a fundamental part of the MVC (Model-View-Controller) architecture used in ASP.NET Core applications. Controllers are responsible for processing user input, interacting with the model (data), and returning appropriate views or data to the client. The Controllers typically contain action methods that correspond to specific HTTP verbs (GET, POST, PUT, DELETE, etc.) and routes. They are decorated with attributes to define routing and other behaviors. The ControllerBase does not support views, making it suitable for APIs that return data (like JSON) rather than HTML views. In this application the view will be handled by the Angular frontend application.
[ApiController] // This attribute indicates that the class is an API controller. It enables several features such as automatic model validation and binding source parameter inference.
// Automatic model binding means that the framework will automatically map incoming request data to action method parameters based on their names and types. String parameters will be bound from the query string, complex types from the request body, etc.
[Route("api/[controller]")] // This attribute defines the route template for the controller. The [controller] token is replaced with the name of the controller class, minus the "Controller" suffix. So for ProductsController, the route will be "api/products".
// The StoreContext is injected into the controller through its constructor. This allows the controller to access the database context and perform operations on the Product entity.
public class ProductsController(IProductRepository productRepository) : ControllerBase
{
  // When a request is made to "api/products", this method will be invoked. The request comes as a thread and the method is asynchronous to avoid blocking the thread while waiting for the database operation to complete. 
  [HttpGet] // This attribute indicates that this action method responds to HTTP GET requests.
  // The Task<ActionResult<IEnumerable<string>>> indicates that this method is asynchronous and returns an ActionResult containing an IEnumerable of strings. ActionResult is a base class for HTTP responses in ASP.NET Core, allowing for various response types (like Ok, NotFound, etc.).
  // The Task part indicates that this method is asynchronous and can be awaited.
  public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts(string? brand, string? type, string? sort)
  {
    // We have to wrap the expression in an Object Result (In this case - the OkActionResult)
    return Ok(await productRepository.GetProductsAsync(brand, type, sort));
  }

  [HttpGet("{id:int}")] // This attribute indicates that this action method responds to HTTP GET requests with an integer parameter in the URL. // api/products/3
  public async Task<ActionResult<Product>> GetProduct(int id)
  {
    var product = await productRepository.GetProductByIdAsync(id);
    if (product == null)
    {
      return NotFound();
    }
    return product;
  }

  [HttpPost]
  public async Task<ActionResult<Product>> CreateProduct(Product product)
  {
    productRepository.AddProduct(product);
    if (await productRepository.SaveChangesAsync())
    {
      // The CreatedAtAction method creates a response with a 201 status code (Created) and includes a Location header with the URI of the newly created resource. It also returns the created product in the response body.
      // The "GetProduct" parameter specifies the action method to use for generating the URI of the newly created product. The new { id = product.Id } parameter provides the route values needed to generate the URI (in this case, the ID of the newly created product that is needed in order to execute the GetProduct -> [HttpGet("{id:int}")]). The product parameter is the content to be returned in the response body.
      // The name of the action method is specified as a string ("GetProduct") to avoid issues with method overloading and to ensure that the correct method is referenced.
      return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }
    return BadRequest("Problem creating product");
  }

  [HttpPut("{id:int}")]
  public async Task<ActionResult> UpdateProduct(int id, Product product)
  {
    if (id != product.Id || !ProductExists(id))
    {
      return BadRequest("Can't update product - Id mismatch");
    }
    // Since the product is not being tracked by the context (it was not retrieved from the database in this request), we need to inform the context that this entity has been modified. This tells the entity tracker that what we are passing here is an updated version of an existing entity and that it has been modified.
    productRepository.UpdateProduct(product);

    // Save the changes to the database asynchronously. After we defined the entity as modified, we need to call SaveChangesAsync to persist the changes to the database.
    if (await productRepository.SaveChangesAsync())
    {
      return NoContent();
    }
    return BadRequest("Problem with updating the product");
    //return NoContent(); // Standard response for a successful PUT request that doesn't return any content.
  }

  [HttpDelete("{id:int}")]
  public async Task<ActionResult> DeleteProduct(int id)
  {
    var product = await productRepository.GetProductByIdAsync(id);
    if (product == null)
    {
      return NotFound();
    }
    productRepository.DeleteProduct(product);
    if (await productRepository.SaveChangesAsync())
    {
      return NoContent();
    }
    return BadRequest("Problem with deleting the product");
  }

  [HttpGet("brands")]
  public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
  {
    return Ok(await productRepository.GetBrandsAsync());
  }

  [HttpGet("types")]
  public async Task<ActionResult<IReadOnlyList<string>>> GetTypes()
  {
    return Ok(await productRepository.GetTypesAsync());
  }

  private bool ProductExists(int id)
  {
    return productRepository.ProductExists(id);
  }
}
