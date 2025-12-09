using Microsoft.AspNetCore.Identity;

namespace Core.Entities;

// Implementing IdentityUser to leverage ASP.NET Core Identity features and use it as the user entity in the application.
public class AppUser : IdentityUser
{
  public string? FirstName { get; set; }
  public string? LastName { get; set; }
  public Address? Address { get; set; }
}
