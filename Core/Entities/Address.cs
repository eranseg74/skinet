using System;

namespace Core.Entities;

// The address is a property of the user but it is not part of the Identity. We will use this addresss as a related proprty in the AppUser entity class
public class Address : BaseEntity
{
  // Property names match the ones used in stripe
  public required string Line1 { get; set; }
  public string? Line2 { get; set; }
  public required string City { get; set; }
  public string? State { get; set; }
  public required string PostalCode { get; set; }
  public required string Country { get; set; }
}
