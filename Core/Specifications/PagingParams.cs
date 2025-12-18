namespace Core.Specifications;

public class PagingParams
{
  private const int MaxPageSize = 50; // Maximum allowed page size to prevent excessive data retrieval.
  private int _pageSize = 6; // Default page size is set to 6.
  public int PageIndex { get; set; } = 1; // Current page index, default is 1.
  public int PageSize { get => _pageSize; set => _pageSize = value > MaxPageSize ? MaxPageSize : value; }
}
