using System.Text;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.RequestHelpers;

// Deriving from the Attribute class - This will allow us to use this as an attribute in our controllers.
// The IAsyncActionFilter allows us to perform actions before (ActionExecutingContext) and after (ActionExecutionDelegate) the EndPoint execution. For example, calling all the products, we can check if they exist in the cache before executing the call and we can store the results of the call to the cache after the call, when we have the response
[AttributeUsage(AttributeTargets.All)] // This attribute can be used on all program elements (classes, methods, properties, etc.)
public class CacheAttribute(int timeToLiveSeconds) : Attribute, IAsyncActionFilter
{
  public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
  {
    // We cannot inject here so we use the context that is passed to this method and request the response cache service we created from there.
    var cacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();
    var cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);
    var cachedResponse = await cacheService.GetCachedResponseAsync(cacheKey);
    if (!string.IsNullOrEmpty(cachedResponse))
    {
      // If we have a cached response we need to return it to the client. We create a ContentResult object that will hold the cached response and set it as the result of the context
      var contentResult = new ContentResult
      {
        Content = cachedResponse,
        ContentType = "application/json", // Because we are returning JSON data and we are not in a controller we need to specify the content type
        StatusCode = 200
      };
      context.Result = contentResult;
      return;
    }
    // Until here we have handled the case when we have a cached response, before executing the action we return the cached response to the client
    // From this point we handle the case when we do not have a cached response and we need to execute the action and cache the response afterwards. If we do not have a cached response we need to execute the action
    var executedContext = await next();
    if (executedContext.Result is OkObjectResult okObjectResult)
    {
      if (okObjectResult.Value != null)
      {
        // We cache the response only if the result is of type OkObjectResult (status code 200)
        await cacheService.CacheResponseAsync(cacheKey, okObjectResult.Value, TimeSpan.FromSeconds(timeToLiveSeconds));
      }
    }
  }

  private string GenerateCacheKeyFromRequest(HttpRequest request)
  {
    var keyBuilder = new StringBuilder();
    keyBuilder.Append($"{request.Path}");
    // We order the key-value pairs so even if we get queries with different orders in keys the keys order will always be the same
    foreach (var (key, value) in request.Query.OrderBy(x => x.Key))
    {
      keyBuilder.Append($"|{key}-{value}");
    }
    return keyBuilder.ToString();
  }
}
