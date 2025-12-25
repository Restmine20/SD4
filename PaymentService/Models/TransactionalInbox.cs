namespace PaymentService.Models
{
  public class TransactionalInbox
  {
    public Guid Id { get; init; }
    public Guid OrderId { get; init; }
    public Guid UserId { get; init; }
    public decimal TotalAmount { get; init; }
    public bool IsProcessed { get; set; }


    public TransactionalInbox(Guid id, Guid orderId, Guid userId, decimal totalAmount, bool isProcessed)
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
      UserId = userId;
      TotalAmount = totalAmount;
      IsProcessed = isProcessed;
    }

    public static TransactionalInbox Create(Guid orderId, Guid userId, decimal totalAmount)
    {
      return new TransactionalInbox(Guid.NewGuid(), orderId, userId, totalAmount, false);
    }
  }
}
