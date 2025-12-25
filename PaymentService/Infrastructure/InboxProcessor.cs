using Microsoft.EntityFrameworkCore;
using PaymentService.Models;

namespace PaymentService.Infrastructure
{
  public class InboxProcessor : IInboxProcessor
  {
    private readonly PaymentDbContext _dbContext;
    public InboxProcessor(PaymentDbContext dbContext)
    {
      _dbContext = dbContext;
    }

    public async Task ProcessAsync(TransactionalInbox message, CancellationToken ct)
    {
      BankAccount? account = await _dbContext.Accounts.FirstOrDefaultAsync(x => x.UserId == message.UserId);
      bool isSuccessful = false;

      if (account != null && account.Balance >= message.TotalAmount)
      {
        account.Balance -= message.TotalAmount;
        isSuccessful = true;
      }

      TransactionalOutbox outMessage = TransactionalOutbox.Create(message.OrderId, isSuccessful);
      _dbContext.TransactionalOutbox.Add(outMessage);

      await _dbContext.SaveChangesAsync();
    }
  }
}