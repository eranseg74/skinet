using Core.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.RequestHelpers;

[AttributeUsage(AttributeTargets.Method)]
// This class is for handling actions after the API call was performed and we have a response
public class InvalidateCache(string pattern) : Attribute, IAsyncActionFilter
{
  public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
  {
    var resultContext = await next(); // Calling next() means we are operating after the call has been executed
    if (resultContext.Exception == null || resultContext.ExceptionHandled)
    {
      var cacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();
      await cacheService.RemoveCacheByPattern(pattern);
    }
  }
}
