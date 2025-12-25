using PaymentService.Models;

namespace PaymentService.Infrastructure
{
  public interface IInboxProcessor
  {
    Task ProcessAsync(TransactionalInbox message, CancellationToken ct);
  }
}