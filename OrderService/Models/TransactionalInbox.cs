namespace OrderService.Models
{
  public class TransactionalInbox
  {
    public Guid Id { get; init; }
    public Guid OrderId { get; init; }
    public bool IsSuccessful { get; init; }
    public bool IsProcessed { get; set; }


    public TransactionalInbox(Guid id, Guid orderId, bool isSuccessful, bool isProcessed)
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

    public static TransactionalInbox Create(Guid orderId, bool isSuccessful)
    {
      return new TransactionalInbox(Guid.NewGuid(), orderId, isSuccessful, false);
    }
  }
}
