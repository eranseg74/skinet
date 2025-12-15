using Core.Entities.OrderAggregate;

namespace Core.Specifications;

public class OrderSpecification : BaseSpecification<Order>
{
  public OrderSpecification(string email) : base(x => x.BuyerEmail == email)
  {
    AddInclude(x => x.OrderItems);
    AddInclude(x => x.DeliveryMethod);
    AddOrderByDescending(x => x.OrderDate);
  }

  public OrderSpecification(string email, int id) : base(x => x.BuyerEmail == email && x.Id == id)
  {
    AddInclude("OrderItems"); // In case we want to define a 'ThenInclude' functionality we would write it by adding a period and the related field, like this: AddInclude("OrderItems.ProductItem"); We don't need to do that because the OrderItem 'ownes' the related fields we need
    AddInclude("DeliveryMethod"); // An example of a case that will use the ThenInclude because the DeliveryMethods is in a related entity that is owned by the Order entity
  }

  // Adding the isPaymentIntent (of type bool) to distinguish this constructor from the first constructor which also gets a string. Constructors of the same class cannot have the same signature.
  public OrderSpecification(string paymentIntentId, bool isPaymentIntent) : base(x => x.PaymentIntentId == paymentIntentId)
  {
    // We need the order items and the delivery method so include it also here
    AddInclude("OrderItems");
    AddInclude("DeliveryMethod");
  }
}
