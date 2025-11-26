using System.Linq.Expressions;

namespace Core.Interfaces;

public interface ISpecification<T>
{
  // Marker interface for specifications
  Expression<Func<T, bool>>? Criteria { get; } // Filtering criteria. This is an expression that defines the conditions that entities must meet to be included in the result set. It takes an entity of type T and returns a boolean indicating whether the entity satisfies the criteria. This allows for dynamic querying based on different conditions. We will provide the Criteria to the specification evaluator in the generic repository to filter the data accordingly.
  // Basically, this interface allows us to define specifications that can be used to filter entities of type T based on specific criteria. It gives the ability to evaluate a where condition on the entities.

  // The following will support sorting and pagination.
  Expression<Func<T, object>>? OrderBy { get; } // Sorting criteria for ascending order. This expression defines how the entities should be ordered when retrieved from the data source. It takes an entity of type T and returns an object that represents the property to sort by in ascending order.
  Expression<Func<T, object>>? OrderByDescending { get; } // Sorting criteria for descending order. Similar to OrderBy, but specifies that the sorting should be done in descending order.

  bool IsDistinct { get; } // Indicates whether the query should return distinct results. If set to true, the query will eliminate duplicate entries from the result set.

  int Take { get; } // Number of records to take for pagination. This property specifies how many records should be retrieved from the data source when pagination is applied.
  int Skip { get; } // Number of records to skip for pagination. This property specifies how many records should be skipped before starting to take records for pagination.
  bool IsPagingEnabled { get; } // Indicates whether pagination is enabled. If set to true, the Take and Skip properties will be used to paginate the results.

  IQueryable<T> ApplyCriteria(IQueryable<T> query); // Method to apply the specification to a given query. This method takes an IQueryable of type T as input and applies the criteria, sorting, and pagination defined in the specification to it. It returns the modified IQueryable with the specification applied. This allows for dynamic querying based on the defined specification.
}

// If we want to return a different type than the entity type T, we can create another interface that inherits from this one and adds a generic type parameter for the return type. For now, we will keep it simple with just one generic type parameter T.
public interface ISpecification<T, TResult> : ISpecification<T>
{
  // This interface can be used when we want to project the entity type T to a different return type TResult.
  Expression<Func<T, TResult>>? Select { get; } // Projection criteria. This expression defines how to project or transform the entity of type T into a different type TResult. It takes an entity of type T and returns an instance of TResult. This is useful when we want to retrieve only specific fields or transform the data in some way before returning it.
}