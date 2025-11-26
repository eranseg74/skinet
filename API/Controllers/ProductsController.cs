using API.RequestHelpers;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;


// The StoreContext is injected into the controller through its constructor. This allows the controller to access the database context and perform operations on the Product entity.
// The ProductsController class inherits from ControllerBase, which provides the basic functionality for handling HTTP requests and responses in an API controller.
// The IGenericRepository<Product> is injected into the controller through its constructor. This allows the controller to access the repository methods for performing CRUD operations on the Product entity.
// Because the repository is registered as a service in the dependency injection container, ASP.NET Core will automatically provide an instance of the repository when creating the ProductsController.
// Because the IGenericRepository is a generic interface, it can be reused for different entities, promoting code reusability and separation of concerns. We set it up specifically for the Product entity in this controller.
public class ProductsController(IGenericRepository<Product> repo) : BaseApiController
{
  // When a request is made to "api/products", this method will be invoked. The request comes as a thread and the method is asynchronous to avoid blocking the thread while waiting for the database operation to complete. 
  [HttpGet] // This attribute indicates that this action method responds to HTTP GET requests.
  // The Task<ActionResult<IEnumerable<string>>> indicates that this method is asynchronous and returns an ActionResult containing an IEnumerable of strings. ActionResult is a base class for HTTP responses in ASP.NET Core, allowing for various response types (like Ok, NotFound, etc.).
  // The Task part indicates that this method is asynchronous and can be awaited.
  public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts([FromQuery] ProductSpecParams specParams)
  {
    // We have to wrap the expression in an Object Result (In this case - the OkActionResult)
    // return Ok(await productRepository.GetProductsAsync(brand, type, sort));
    // Using the generic repository instead of the specific product repository. This will give us an unsorted and unfiltered list of products because the parameters are not used in the generic implementation.
    var spec = new ProductSpecification(specParams);

    return await CreatePagedResult(repo, spec, specParams.PageIndex, specParams.PageSize);
  }

  [HttpGet("{id:int}")] // This attribute indicates that this action method responds to HTTP GET requests with an integer parameter in the URL. // api/products/3
  public async Task<ActionResult<Product>> GetProduct(int id)
  {
    var product = await repo.GetByIdAsync(id);
    if (product == null)
    {
      return NotFound();
    }
    return product;
  }

  [HttpPost]
  public async Task<ActionResult<Product>> CreateProduct(Product product)
  {
    repo.Add(product);
    if (await repo.SaveAllAsync())
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
    repo.Update(product);

    // Save the changes to the database asynchronously. After we defined the entity as modified, we need to call SaveChangesAsync to persist the changes to the database.
    if (await repo.SaveAllAsync())
    {
      return NoContent();
    }
    return BadRequest("Problem with updating the product");
    //return NoContent(); // Standard response for a successful PUT request that doesn't return any content.
  }

  [HttpDelete("{id:int}")]
  public async Task<ActionResult> DeleteProduct(int id)
  {
    var product = await repo.GetByIdAsync(id);
    if (product == null)
    {
      return NotFound();
    }
    repo.Remove(product);
    if (await repo.SaveAllAsync())
    {
      return NoContent();
    }
    return BadRequest("Problem with deleting the product");
  }

  [HttpGet("brands")]
  public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
  {
    // return Ok(await productRepository.GetBrandsAsync());
    var spec = new BrandListSpecification();
    return Ok(await repo.ListAsync(spec));
  }

  [HttpGet("types")]
  public async Task<ActionResult<IReadOnlyList<string>>> GetTypes()
  {
    // return Ok(await productRepository.GetTypesAsync());
    var spec = new TypeListSpecification();
    return Ok(await repo.ListAsync(spec));
  }

  private bool ProductExists(int id)
  {
    return repo.Exists(id);
  }
}
