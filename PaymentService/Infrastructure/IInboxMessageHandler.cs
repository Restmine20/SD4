using PaymentService.Models;

namespace PaymentService.Infrastructure
{
  public interface IInboxMessageHandler
  {
    Task HandleAsync(TransactionalInbox message, CancellationToken ct);
  }
}