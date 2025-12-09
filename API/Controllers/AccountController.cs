using API.DTOs;
using API.Extensions;
using Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    // Using the SignInManager that is provided by the Identity Framework. The SignInManager is used to login, check password, signin etc. To create a new user we need the UserManager but we can get it from the SignInManager
    public class AccountController(SignInManager<AppUser> signInManager) : BaseApiController
    {
        [HttpPost("register")]
        public async Task<ActionResult> Register(RegisterDto registerDto)
        {
            var user = new AppUser
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Email = registerDto.Email,
                UserName = registerDto.Email // This field is required by the Identity Framework
            };
            var result = await signInManager.UserManager.CreateAsync(user, registerDto.Password); // No need to save changes here because the CreateAsync will already save it to the DB and return the IdentityResult object
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    // ModelState is a dictionary that holds all the error codes and descriptions for the model. Basically it is a list of all the response codes with their description. Here we are adding a new code and description to the dictionary to get the same format in the error as any 401, 400 or other known codes
                    ModelState.AddModelError(error.Code, error.Description);
                }
                // The ValidationProblem() Creates an ActionResult that produces a StatusCodes.Status400BadRequest response with validation errors from ControllerBase.ModelState. The ModelState is what we get from the ApiController which is defined in the BaseController and is inherited here in the AccountController or any other controller in this project
                return ValidationProblem();
            }
            return Ok();
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<ActionResult> Logout()
        {
            await signInManager.SignOutAsync(); // This includes the removal of the cookie with the user data
            return NoContent();
        }

        // We are not defining this call as Authorized. This call will be used on the current user but because we don't have access to the cookies in the client side, we do not really know if the user is logged in until we will make this call. We will check first if the current user is authenticated before making this call.
        // If we would use the [Authorized] attribute we would get a 401 error if the user is not logged in. This way we are returning a 204 NoContent
        [HttpGet("user-info")]
        public async Task<ActionResult> GetUserInfo()
        {
            if (User.Identity?.IsAuthenticated == false) return NoContent();

            var user = await signInManager.UserManager.GetUserByEmailWithAddress(User);

            return Ok(new
            {
                user.FirstName,
                user.LastName,
                user.Email,
                Address = user.Address?.ToDto(), // Because it is defined inside an anonymous object and it could be null we need to define it like this and not like the FirstName, LastName, or Email which are strrings
            });
        }

        // An anonymous call that checks if the user is authenticated. If the user is not logged in then the User.Identity is null and a false value is returned
        [HttpGet]
        public ActionResult GetAuthState()
        {
            return Ok(new { IsAuthenticated = User.Identity?.IsAuthenticated ?? false });
        }

        [Authorize]
        [HttpPost("address")]
        public async Task<ActionResult<Address>> CreateOrUpdateAddress(AddressDto addressDto)
        {
            var user = await signInManager.UserManager.GetUserByEmailWithAddress(User);
            if (user.Address == null)
            {
                user.Address = addressDto.ToEntity();
            }
            else
            {
                user.Address.UpdateFromDto(addressDto);
            }
            var result = await signInManager.UserManager.UpdateAsync(user);
            if (!result.Succeeded) return BadRequest("Problem updating user address");
            return Ok(user.Address.ToDto());
        }
    }
}
