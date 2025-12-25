namespace PaymentService.Models
{
  public class TransactionalOutbox
  {
    public Guid Id { get; init; }
    public Guid OrderId { get; init; }
    public bool IsSuccessful { get; init; }
    public bool IsProcessed { get; set; }


    public TransactionalOutbox(Guid id, Guid orderId, bool isSuccessful, bool isProcessed)
    {
      if (id == Guid.Empty)
      {
        throw new ArgumentException("ID cannot be empty.");
      }
      if (orderId == Guid.Empty)
      {
        throw new ArgumentException("OrderID cannot be empty.");
      }

      Id = id;
      OrderId = orderId;
      IsSuccessful = isSuccessful;
      IsProcessed = isProcessed;
    }

    public static TransactionalOutbox Create(Guid orderId, bool isSuccessful)
    {
      return new TransactionalOutbox(Guid.NewGuid(), orderId, isSuccessful, false);
    }
  }
}
