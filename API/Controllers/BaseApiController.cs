using API.RequestHelpers;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    // The Controllers in ASP.NET Core are classes that handle incoming HTTP requests and send back HTTP responses to the client. They are a fundamental part of the MVC (Model-View-Controller) architecture used in ASP.NET Core applications. Controllers are responsible for processing user input, interacting with the model (data), and returning appropriate views or data to the client. The Controllers typically contain action methods that correspond to specific HTTP verbs (GET, POST, PUT, DELETE, etc.) and routes. They are decorated with attributes to define routing and other behaviors. The ControllerBase does not support views, making it suitable for APIs that return data (like JSON) rather than HTML views. In this application the view will be handled by the Angular frontend application.
    // Automatic model binding means that the framework will automatically map incoming request data to action method parameters based on their names and types. String parameters will be bound from the query string, complex types from the request body, etc.
    [ApiController] // This attribute indicates that the class is an API controller. It enables several features such as automatic model validation and binding source parameter inference.
    [Route("api/[controller]")] // This attribute defines the route template for the controller. The [controller] token is replaced with the name of the controller class, minus the "Controller" suffix. So for ProductsController, the route will be "api/products".
    public class BaseApiController : ControllerBase
    {
        protected async Task<ActionResult> CreatePagedResult<T>(IGenericRepository<T> repo, ISpecification<T> spec, int pageIndex, int pageSize) where T : BaseEntity
        {
            var items = await repo.ListAsync(spec);
            var count = await repo.CountAsync(spec);

            var pagination = new Pagination<T>(pageIndex, pageSize, count, items); // This will give us only the desired amount of products and not the entire products in the dataset
            return Ok(pagination);
        }
    }
}
