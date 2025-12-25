using OrderService.Models;

namespace OrderService.Infrastructure
{
  public interface IInboxMessageHandler
  {
    Task HandleAsync(TransactionalInbox message, CancellationToken ct);
  }
}