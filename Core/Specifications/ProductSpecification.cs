using Core.Entities;

namespace Core.Specifications;

public class ProductSpecification : BaseSpecification<Product>
{
  // The constructor takes two string parameters, brand and type, which represent the brand and type of products to filter.
  // It calls the base class constructor with a lambda expression that defines the filtering criteria.
  // The lambda expression checks if the brand parameter is null or empty. If it is, it does not filter by brand; otherwise, it checks if the product's Brand property matches the brand parameter.
  // Similarly, it checks the type parameter and filters by the product's Type property if the type parameter is not null or empty.
  public ProductSpecification(string? brand, string? type, string? sort) : base(p =>
    (string.IsNullOrEmpty(brand) || p.Brand == brand) &&
    (string.IsNullOrEmpty(type) || p.Type == type))
  {
    // Here we can add sorting criteria if needed. For example, we can sort by product name in ascending order.
    switch (sort)
    {
      case "priceAsc":
        AddOrderBy(p => p.Price);
        break;
      case "priceDesc":
        AddOrderByDescending(p => p.Price);
        break;
      default:
        AddOrderBy(p => p.Name);
        break;
    }
  }
}
