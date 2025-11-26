namespace Core.Specifications;

public class ProductSpecParams
{
  private const int MaxPageSize = 50; // Maximum allowed page size to prevent excessive data retrieval.
  private int _pageSize = 6; // Default page size is set to 6.
  public int PageIndex { get => field; set => field = value; } = 1; // Current page index, default is 1.
  private List<string> _brands = [];
  public List<string> Brands
  {
    get => _brands;
    set
    {
      _brands = [.. value.SelectMany(x => x.Split(',', StringSplitOptions.RemoveEmptyEntries))]; // Split by comma and flatten. The [.. ] syntax is used to create a new array by spreading the elements of the source array. This is like the spread operator in JavaScript and is the same as wusing the ToList().
    }
  }

  // Property and backing field for Types. This means that when we set the Types property, we split the input strings by commas and flatten the result into a single list of type strings.
  private List<string> _types = [];
  public List<string> Types
  {
    get => _types;
    set
    {
      _types = [.. value.SelectMany(x => x.Split(',', StringSplitOptions.RemoveEmptyEntries))]; // Split by comma and flatten. The [.. ] syntax is used to create a new array by spreading the elements of the source array. This is like the spread operator in JavaScript and is the same as wusing the ToList(). RemoveEmptyEntries to avoid empty strings in the result.
    }
  }

  public string? Sort { get; set; }
  public int PageSize { get => _pageSize; set => _pageSize = value > MaxPageSize ? MaxPageSize : value; }
  private string? _search;
  public string Search
  {
    get { return _search ?? ""; }
    set { _search = value.ToLower(); }
  }

}
