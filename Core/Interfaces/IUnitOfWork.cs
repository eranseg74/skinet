using Core.Entities;

namespace Core.Interfaces;

// Implementing the IDisposable means that we can implement a dispose method so that dotnet will run this method whenever the unit of work instance will be out of scope (completed). It is not quite required because everything that will be out of scope, DotNet will make sure to dispose of it but just to be on the sdafe side
public interface IUnitOfWork : IDisposable
{
  IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity;
  // The Complete method will contain several transactions. The idea here is that if all the transactions are successful they will be executed, but even if one transaction fails, the system will rollback all the transactions that occured up until the failed transaction. This way we keep the integrity of the DB without having a situation of a partial update which is bad!
  Task<bool> Complete();
}
