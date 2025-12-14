using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class SpecificationEvaluator<T> where T : BaseEntity
{
  // The GetQuery method takes an IQueryable<T> called inputQuery and an ISpecification<T> called spec as parameters.
  public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> spec)
  {
    var query = inputQuery;

    // Apply criteria from specification
    if (spec.Criteria != null)
    {
      query = query.Where(spec.Criteria); // The Where method is used to filter the query based on the criteria defined in the specification. The Criteria property is an expression that defines the filtering conditions. For example, if the Criteria specifies that a product's price must be greater than 100, the Where method will filter the query to include only those products that meet this condition. (e.g. -> x => x.Brand == brand or x => x.Type == type or x => x.Price > 100 etc.)
    }

    // Apply sorting from specification
    if (spec.OrderBy != null)
    {
      query = query.OrderBy(spec.OrderBy);
    }

    if (spec.OrderByDescending != null)
    {
      query = query.OrderByDescending(spec.OrderByDescending);
    }

    // Apply distinct from specification
    if (spec.IsDistinct)
    {
      query = query.Distinct();
    }

    // Apply pagination from specification
    if (spec.IsPagingEnabled)
    {
      query = query.Skip(spec.Skip).Take(spec.Take);
    }

    // We implement the Include methods to get related entities. We define it only here in the evaluator because it's related to querying the database. Also only in this method because if we are using includes, we do not use it for projection.
    // Here we use the Aggregate function to allow several includes. Each time the function will take the query. The query will be the current and the include will be the expression we want to add. The updated query will be passed again to enter the next include to the updated query until we get a query which aggregates all the required queries.
    query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));
    query = spec.IncludeStrings.Aggregate(query, (current, include) => current.Include(include));

    return query;
  }

  public static IQueryable<TResult> GetQuery<TSpec, TResult>(IQueryable<T> inputQuery, ISpecification<T, TResult> spec)
  {
    var query = inputQuery;

    // Apply criteria from specification
    if (spec.Criteria != null)
    {
      query = query.Where(spec.Criteria); // The Where method is used to filter the query based on the criteria defined in the specification. The Criteria property is an expression that defines the filtering conditions. For example, if the Criteria specifies that a product's price must be greater than 100, the Where method will filter the query to include only those products that meet this condition. (e.g. -> x => x.Brand == brand or x => x.Type == type or x => x.Price > 100 etc.)
    }

    // Apply sorting from specification
    if (spec.OrderBy != null)
    {
      query = query.OrderBy(spec.OrderBy);
    }

    if (spec.OrderByDescending != null)
    {
      query = query.OrderByDescending(spec.OrderByDescending);
    }
    // Apply projection from specification. If no projection is defined, we can cast the query to TResult assuming T and TResult are compatible. The Cast method is used to convert the elements of the query to the specified type TResult. It will be executed only if there is no Select expression defined in the specification.
    var selectQuery = query as IQueryable<TResult>;
    if (spec.Select != null)
    {
      selectQuery = query.Select(spec.Select);
    }

    if (spec.IsDistinct)
    {
      selectQuery = selectQuery?.Distinct();
    }

    // Apply pagination from specification
    if (spec.IsPagingEnabled)
    {
      selectQuery = selectQuery?.Skip(spec.Skip).Take(spec.Take);
    }

    return selectQuery ?? query.Cast<TResult>();
  }
}
