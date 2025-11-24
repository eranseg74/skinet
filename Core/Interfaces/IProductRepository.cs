using Core.Entities;

namespace Core.Interfaces;

public interface IProductRepository
{
  // We use the IReadOnlyList<T> interface to indicate that the returned list of products should not be modified.
  Task<IReadOnlyList<Product>> GetProductsAsync(string? brand, string? type, string? sort);

  // The nullable Product? return type indicates that the method may return null if no product with the specified ID is found. We want to handle cases where a product might not exist in the controllers and not the repository.
  Task<Product?> GetProductByIdAsync(int id);

  Task<IReadOnlyList<string>> GetBrandsAsync();
  Task<IReadOnlyList<string>> GetTypesAsync();

  // The add, update, and delete methods do not return any value. They perform their operations on the data store. They are defined as synchronous methods here because we use them only for tracking changes in the DbContext. The actual saving to the database is done asynchronously in the Unit of Work pattern.
  void AddProduct(Product product);
  void UpdateProduct(Product product);
  void DeleteProduct(Product product);
  bool ProductExists(int id);
  Task<bool> SaveChangesAsync(); // Commit the changes to the database asynchronously. This method returns a boolean indicating whether any changes were saved. The implementation of this method will typically call the SaveChangesAsync method of the DbContext and return the number of changes.
}
