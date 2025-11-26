using Core.Entities;

namespace Core.Specifications;

public class ProductSpecification : BaseSpecification<Product>
{
  // The constructor takes two string parameters, brand and type, which represent the brand and type of products to filter.
  // It calls the base class constructor with a lambda expression that defines the filtering criteria.
  // The lambda expression checks if the brand parameter is null or empty. If it is, it does not filter by brand; otherwise, it checks if the product's Brand property matches the brand parameter.
  // Similarly, it checks the type parameter and filters by the product's Type property if the type parameter is not null or empty.
  public ProductSpecification(ProductSpecParams specParams) : base(p =>
    (string.IsNullOrEmpty(specParams.Search) || p.Name.ToLower().Contains(specParams.Search)) &&
    (specParams.Brands.Count == 0 || specParams.Brands.Contains(p.Brand)) &&
    (specParams.Types.Count == 0 || specParams.Types.Contains(p.Type)))
  {
    // Apply pagination using the PageIndex and PageSize from specParams. Calculate the number of records to skip based on the current page index and page size. The calculation is (PageIndex - 1) * PageSize. Then, take the number of records specified by PageSize. For example, if PageIndex is 2 and PageSize is 10, we skip the first 10 records and take the next 10 records.
    ApplyPaging((specParams.PageIndex - 1) * specParams.PageSize, specParams.PageSize);
    // Here we can add sorting criteria if needed. For example, we can sort by product name in ascending order.
    switch (specParams.Sort)
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
