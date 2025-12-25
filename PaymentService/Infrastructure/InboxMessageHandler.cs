using PaymentService.Models;

namespace PaymentService.Infrastructure
{
  public class InboxMessageHandler : IInboxMessageHandler
  {
    private readonly PaymentDbContext _dbContext;

    public InboxMessageHandler(PaymentDbContext dbContext)
    {
      _dbContext = dbContext;
    }

    public async Task HandleAsync(TransactionalInbox message, CancellationToken ct)
    {
      TransactionalInbox? check = await _dbContext.TransactionalInbox.FindAsync(message.Id);

      if (check != null)
      {
        return;
      }
      _dbContext.TransactionalInbox.Add(message);
      await _dbContext.SaveChangesAsync(ct);
    }
  }
}