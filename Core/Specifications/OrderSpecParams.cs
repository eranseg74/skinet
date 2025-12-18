namespace Core.Specifications;

public class OrderSpecParams : PagingParams
{
  // For filtering the result according to the order status
  public string? Status { get; set; }
}
