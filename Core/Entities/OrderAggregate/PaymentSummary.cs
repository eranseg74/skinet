namespace Core.Entities.OrderAggregate;

// Also owned property by the Order entity to hold payment details
public class PaymentSummary
{
  public int Last4 { get; set; }
  public required string Brand { get; set; }
  public int ExpMonth { get; set; }
  public int ExpYear { get; set; }
}
