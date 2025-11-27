using System;
using System.Net;
using System.Text.Json;
using API.Errors;

namespace API.Middleware;

// The IHostEnvironment class gives us permission to see the environment vriables such as if we are in a development or production environment
public class ExceptionMiddleware(IHostEnvironment environment, RequestDelegate next)
{
  // This class will be used as a middleware. The middleware expects an InvokeAsync method so we must create such method with this exact name!!!
  public async Task InvokeAsync(HttpContext context)
  {
    try
    {
      await next(context);
    }
    catch (Exception ex)
    {
      await HandleExceptionAsync(context, ex, environment);
    }
  }

  private static Task HandleExceptionAsync(HttpContext context, Exception ex, IHostEnvironment environment)
  {
    context.Response.ContentType = "application/json";
    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

    var response = environment.IsDevelopment() ?
      new ApiErrorResponse(context.Response.StatusCode, ex.Message, ex.StackTrace) :
      new ApiErrorResponse(context.Response.StatusCode, ex.Message, "Internal server error");

    // By default the ApiController will return the response ain a CamelCase format but if we are changing somthing outside the context, it won't so we do need to define it here
    var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    var json = JsonSerializer.Serialize(response, options);

    return context.Response.WriteAsync(json); // Writes the given text to the response body. UTF-8 encoding will be used. Returns a task that represents the completion of the write operation.
  }
}
