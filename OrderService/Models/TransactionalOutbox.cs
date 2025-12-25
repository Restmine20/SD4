namespace OrderService.Models
{
  public class TransactionalOutbox
  {
    public Guid Id { get; init; }
    public Guid OrderId { get; init; }
    public Guid UserId { get; init; }
    public decimal TotalAmount { get; init; }
    public bool IsProcessed { get; set; }


    public TransactionalOutbox(Guid id, Guid orderId, Guid userId, decimal totalAmount, bool isProcessed)
    {
      if (id == Guid.Empty)
      {
        throw new ArgumentException("OrderID cannot be empty.");
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

    public static TransactionalOutbox Create(Guid orderId, Guid userId, decimal totalAmount)
    {
      return new TransactionalOutbox(Guid.NewGuid(), orderId, userId, totalAmount, false);
    }
  }
}
