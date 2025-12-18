namespace Core.Specifications;

public class ProductSpecParams : PagingParams
{
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

  private string? _search;
  public string Search
  {
    get { return _search ?? ""; }
    set { _search = value.ToLower(); }
  }

}
