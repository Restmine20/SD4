namespace PaymentService.Models
{
  public class BankAccount
  {
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public DateTime CreatedAt { get; init; }
    public decimal Balance { get; set; }

    public BankAccount(Guid id, Guid userId, DateTime createdAt, decimal balance)
    {
      if (id == Guid.Empty)
      {
        throw new ArgumentException("ID cannot be empty.");
      }
      if (userId == Guid.Empty)
      {
        throw new ArgumentException("UserID cannot be empty.");
      }

      Id = id;
      UserId = userId;
      Balance = balance;
    }

    public static BankAccount Create(Guid userId)
    {
      return new BankAccount(Guid.NewGuid(), userId, DateTime.UtcNow, 0);
    }
  }
}
