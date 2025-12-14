using System.Collections.Concurrent;
using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Data;

public class UnitOfWork(StoreContext context) : IUnitOfWork
{
  // This dictionary will hold all the repositories which are included in the UnitOfWork. The string will be the name and the object will be the repository itself
  private readonly ConcurrentDictionary<string, object> _repositories = new();
  public async Task<bool> Complete()
  {
    return context.SaveChanges() > 0;
  }

  public void Dispose()
  {
    context.Dispose();
    GC.SuppressFinalize(this);
  }

  // When we will use the UnitOfWork object we will specify the type of the repository (TEntity). then, if we have this repository in the repositories dictionary we will return this repository. Otherwise we will create a new instance of this repository and save it in the _repositories dictionary. If we did not implement such a repository we will throw an exception that no such repository exists
  public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
  {
    // This will give us the name of the entity we are using for a specific repository
    var type = typeof(TEntity).Name;
    return (IGenericRepository<TEntity>)_repositories.GetOrAdd(type, t =>
    {
      var repositoryType = typeof(GenericRepository<>).MakeGenericType(typeof(TEntity));
      return Activator.CreateInstance(repositoryType, context) ?? throw new InvalidOperationException($"Could not create repository instance for {t}");
    });
  }
}
