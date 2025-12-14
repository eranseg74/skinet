using Core.Entities;

namespace Core.Interfaces;

public interface IGenericRepository<T> where T : BaseEntity
{
  Task<T?> GetByIdAsync(int id);
  Task<IReadOnlyList<T>> ListAllAsync();
  Task<T?> GetEntityWithSpecAsync(ISpecification<T> spec);
  Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec);
  Task<TResult?> GetEntityWithSpecAsync<TResult>(ISpecification<T, TResult> spec);
  Task<IReadOnlyList<TResult>> ListAsync<TResult>(ISpecification<T, TResult> spec);
  void Add(T entity);
  void Update(T entity);
  void Remove(T entity);
  // Task<bool> SaveAllAsync(); // Implemented in the UnitOfWork
  bool Exists(int id);
  Task<int> CountAsync(ISpecification<T> spec); // new method to count entities based on a specification. We need this for pagination so we know how many pages there are.
}
