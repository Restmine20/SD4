using OrderService.Models;

namespace OrderService.Infrastructure
{
  public class InboxMessageHandler : IInboxMessageHandler
  {
    private readonly OrderDbContext _dbContext;

    public InboxMessageHandler(OrderDbContext dbContext)
    {
      _dbContext = dbContext;
    }

    public async Task HandleAsync(TransactionalInbox message, CancellationToken ct)
    {
      Order? order = await _dbContext.Orders.FindAsync(message.OrderId);

      if (order == null)
      {
        throw new InvalidOperationException();
      }

      order.Status = message.IsSuccessful ? OrderStatus.FINISHED : OrderStatus.CANCELLED;
      await _dbContext.SaveChangesAsync();
    }
  }
}