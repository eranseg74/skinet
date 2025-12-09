using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class AddressDto
{
  // Adding the [Required] notation and not using hte required keyword will give us the ability to return ModelState errors in the correct format rather then just get an array of errors with no code.
  [Required]
  public string Line1 { get; set; } = string.Empty;

  public string? Line2 { get; set; }

  [Required]
  public string City { get; set; } = string.Empty;

  [Required]
  public string State { get; set; } = string.Empty;

  [Required]
  public string PostalCode { get; set; } = string.Empty;

  [Required]
  public string Country { get; set; } = string.Empty;
}
