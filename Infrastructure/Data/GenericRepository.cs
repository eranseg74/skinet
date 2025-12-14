using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class GenericRepository<T>(StoreContext context) : IGenericRepository<T> where T : BaseEntity
{
  public void Add(T entity)
  {
    // Adding the entity to the DbSet corresponding to type T in the DbContext. The Set<T>() method retrieves the DbSet for the specified entity type T, allowing us to perform CRUD operations on that set. The Add method marks the entity as added in the context, so that when SaveChanges is called on the context, this entity will be inserted into the database.
    context.Set<T>().Add(entity);
  }

  public async Task<int> CountAsync(ISpecification<T> spec)
  {
    var query = context.Set<T>().AsQueryable();
    query = spec.ApplyCriteria(query);
    return await query.CountAsync();
  }

  public bool Exists(int id)
  {
    return context.Set<T>().Any(x => x.Id == id);
  }

  public async Task<T?> GetByIdAsync(int id)
  {
    return await context.Set<T>().FindAsync(id);
  }

  public async Task<T?> GetEntityWithSpecAsync(ISpecification<T> spec)
  {
    return await ApplySpecification(spec).FirstOrDefaultAsync();
  }

  public async Task<TResult?> GetEntityWithSpecAsync<TResult>(ISpecification<T, TResult> spec)
  {
    return await ApplySpecification<TResult>(spec).FirstOrDefaultAsync();
  }

  public async Task<IReadOnlyList<T>> ListAllAsync()
  {
    return await context.Set<T>().ToListAsync();
  }

  public async Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec)
  {
    return await ApplySpecification(spec).ToListAsync();
  }

  public async Task<IReadOnlyList<TResult>> ListAsync<TResult>(ISpecification<T, TResult> spec)
  {
    return await ApplySpecification<TResult>(spec).ToListAsync();
  }

  public void Remove(T entity)
  {
    context.Set<T>().Remove(entity);
  }

  // Imlemented in the UnitOfWork
  // public async Task<bool> SaveAllAsync()
  // {
  //   return await context.SaveChangesAsync() > 0;
  // }

  public void Update(T entity)
  {
    // This method begins tracking the entity in the DbContext's change tracker. If the entity has a primary key value, it's assumed to exist in the database, and its state is initially set to Unchanged. If it doesn't have a primary key, it might be marked as Added (though this is less common for Attach when the intention is to update).
    context.Set<T>().Attach(entity);
    // After attaching the entity, this line explicitly overrides its state and sets it to Modified. This tells Entity Framework that the entity has been changed and needs to be updated in the database.
    context.Entry(entity).State = EntityState.Modified;
    // Alternatively, we could use the following line to update the entity:
    // This method is a shorthand that combines the functionality of attaching and marking as modified. It begins tracking the entity (or updates an already tracked entity) and immediately sets its state to Modified.
    // context.Set<T>().Update(entity);
    // Both approaches effectively inform Entity Framework that the entity should be updated in the database when SaveChanges is called.
    /* Note: Using Attach followed by setting the state to Modified is useful when you have an entity that you know exists in the database and you want to update it without first fetching it. Using Update is more straightforward and is often preferred for its simplicity. However, both methods achieve the same end result of marking the entity for update. */
  }

  // This method applies the specification to the DbSet<T> and returns an IQueryable<T> that represents the filtered query based on the specification's criteria.
  private IQueryable<T> ApplySpecification(ISpecification<T> spec)
  {
    return SpecificationEvaluator<T>.GetQuery(context.Set<T>().AsQueryable(), spec);
  }

  private IQueryable<TResult> ApplySpecification<TResult>(ISpecification<T, TResult> spec)
  {
    return SpecificationEvaluator<T>.GetQuery<T, TResult>(context.Set<T>().AsQueryable(), spec);
  }
}
