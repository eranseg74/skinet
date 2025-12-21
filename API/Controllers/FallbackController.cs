using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;


// Fallback controller to handle requests that are not handled by other controllers. The name "Fallback" is used in the Program.cs file to map the fallback route.
public class FallbackController : Controller
{
  // Action to serve the index.html file for any request that is not handled by other controllers. This allows the Angular application to handle client-side routing. The index.html file is located in the wwwroot folder. The name "Index" is used in the Program.cs file to map the fallback route.
  public IActionResult Index()
  {
    return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "index.html"), "text/HTML");
  }
}
