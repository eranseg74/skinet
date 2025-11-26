namespace API.RequestHelpers;

public class Pagination<T>(int pageIndex, int pageSize, int count, IReadOnlyList<T> data)
{
  public int PageIndex { get; set; } = pageIndex; // current page number
  public int PageSize { get; set; } = pageSize; // number of items per page
  public int Count { get; set; } = count; // total number of items
  public IReadOnlyList<T> Data { get; set; } = data; // the items for the current page

}
