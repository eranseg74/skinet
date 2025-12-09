using System.Security.Claims;
using API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class BuggyController : BaseApiController
{
  [HttpGet("unauthorized")]
  public IActionResult GetUnauthorized()
  {
    return Unauthorized();
  }

  [HttpGet("badrequest")]
  public IActionResult GetBadRequest()
  {
    return BadRequest("Not a good request");
  }

  [HttpGet("notfound")]
  public IActionResult GetNotFound()
  {
    return NotFound();
  }

  [HttpGet("internalerror")]
  public IActionResult GetInternalError()
  {
    throw new Exception("This is a test exception");
  }

  [HttpPost("validationerror")]
  public IActionResult GetValidationError(CreateProductDto product)
  {
    return Ok();
  }

  // When we are in a context of a controller we get access to the User object that is used when a user authenticates to an EndPoint. This EndPoint contains the User object which is of type ClaimsPrincipal which is the information contained in the cookie that is used to authorize the specified user, based on what was sent back from the API
  [Authorize]
  [HttpGet("secret")]
  public IActionResult GetSecret()
  {
    var name = User.FindFirst(ClaimTypes.Name)?.Value; // The ClaimTypes.Name refers to the UserName property defined in the IdentityUser class (from which the AppUser is derived)
    var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // The ClaimTypes.NameIdentifier refers to the Id property defined in the IdentityUser class (from which the AppUser is derived)
    return Ok("Hello " + name + " with the id of " + id);
  }
}
