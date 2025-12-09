using System;
using System.Security.Authentication;
using System.Security.Claims;
using Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions;

public static class ClaimsPrincipleExtensions
{
  // When we are extending something we are using the this keyword and we are passing the thing that we are extending. The ClaimsPrinciple in this case is the User object
  public static async Task<AppUser> GetUserByEmail(this UserManager<AppUser> userManager, ClaimsPrincipal user)
  {
    var userToReturn = await userManager.Users.FirstOrDefaultAsync(x => x.Email == user.GetEmail()) ?? throw new AuthenticationException("User not found");
    return userToReturn;
  }

  public static string GetEmail(this ClaimsPrincipal user)
  {
    var email = user.FindFirstValue(ClaimTypes.Email) ?? throw new AuthenticationException("Email claim not found");
    return email;
  }

  public static async Task<AppUser> GetUserByEmailWithAddress(this UserManager<AppUser> userManager, ClaimsPrincipal user)
  {
    var userToReturn = await userManager.Users
      .Include(x => x.Address) // Since the Address is a related property it will not be given automatically so we need to specify that we also want this property in the result. We specify it by using the include() function and define the desired property that will be added to the object
      .FirstOrDefaultAsync(x => x.Email == user.GetEmail()) ?? throw new AuthenticationException("User not found");
    return userToReturn;
  }
}
