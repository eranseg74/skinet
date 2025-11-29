using System.Linq.Expressions;
using Core.Interfaces;

namespace Core.Specifications;

// BaseSpecification is a generic class that implements the ISpecification<T> interface. It serves as a base class for creating specifications that define filtering criteria for querying entities of type T.
// The constructor of the BaseSpecification class takes an Expression<Func<T, bool>> parameter called criteria. This parameter represents the filtering criteria that will be used to evaluate whether entities of type T meet certain conditions.
// The Criteria property is implemented to return the criteria expression passed to the constructor. This allows derived specification classes to inherit and utilize the filtering criteria defined in the base class.
// What this means is when we create a new specification by inheriting from BaseSpecification, we can pass a filtering expression to the base constructor, and that expression will be accessible through the Criteria property. This enables us to create reusable and composable specifications for querying entities based on different conditions.
public class BaseSpecification<T>(Expression<Func<T, bool>>? criteria) : ISpecification<T>
{
  // An empty constructor that allows running a query without any criteria. 
  protected BaseSpecification() : this(null)
  {
  }
  public Expression<Func<T, bool>>? Criteria => criteria;

  public Expression<Func<T, object>>? OrderBy { get; private set; }

  public Expression<Func<T, object>>? OrderByDescending { get; private set; }

  // Indicates whether the query should return distinct results. If set to true, the query will eliminate duplicate entries from the result set. The property has a private setter, so it can only be modified within the class or its derived classes.
  public bool IsDistinct { get; private set; }

  public int Take { get; private set; }

  public int Skip { get; private set; }

  public bool IsPagingEnabled { get; private set; }

  public IQueryable<T> ApplyCriteria(IQueryable<T> query)
  {
    if (Criteria != null)
    {
      query = query.Where(Criteria);
    }
    return query;
  }

  // The expression will be evaluated in the specification evaluator in the generic repository to filter the data accordingly. The evaluator will be in the Infrastructure layer.

  // Methods to set the sorting criteria
  protected void AddOrderBy(Expression<Func<T, object>> orderByExpression)
  {
    OrderBy = orderByExpression;
  }
  protected void AddOrderByDescending(Expression<Func<T, object>> orderByDescExpression)
  {
    OrderByDescending = orderByDescExpression;
  }

  // Method to set the IsDistinct property to true
  protected void ApplyDistinct()
  {
    IsDistinct = true;
  }

  // Method to apply pagination
  protected void ApplyPaging(int skip, int take)
  {
    Skip = skip;
    Take = take;
    IsPagingEnabled = true;
  }
}

// We can also create another base specification class that supports projection to a different return type TResult.
public class BaseSpecification<T, TResult>(Expression<Func<T, bool>>? criteria) : BaseSpecification<T>(criteria), ISpecification<T, TResult>
{
  protected BaseSpecification() : this(null)
  {
  }
  public Expression<Func<T, TResult>>? Select { get; private set; }
  // Method to set the projection criteria
  protected void AddSelect(Expression<Func<T, TResult>> selectExpression)
  {
    Select = selectExpression;
  }
}
