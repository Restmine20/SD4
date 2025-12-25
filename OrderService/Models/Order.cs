namespace OrderService.Models
{
  public class Order
  {
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public DateTime CreatedAt { get; init; }
    public OrderStatus Status { get; set; }
    public string? Description { get; private set; }
    public decimal TotalAmount { get; init; }

    public Order(Guid id, Guid userId, DateTime createdAt, OrderStatus status, string? description, decimal totalAmount)
    {
      if (id == Guid.Empty)
      {
        throw new ArgumentException("OrderID cannot be empty.");
      }
      if (userId == Guid.Empty)
      {
        throw new ArgumentException("UserID cannot be empty.");
      }

      Id = id;
      UserId = userId;
      CreatedAt = createdAt;
      Status = status;
      Description = description;
      TotalAmount = totalAmount;
    }

    public static Order Create(Guid userId, string? description, decimal totalAmount)
    {
      return new Order(Guid.NewGuid(), userId, DateTime.UtcNow, OrderStatus.NEW, description, totalAmount);
    }
  }
}
