using PaymentService.Models;

namespace PaymentService.Infrastructure
{
  public interface IOutboxProcessor
  {
    Task ProcessAsync(TransactionalOutbox message, CancellationToken ct);
  }
}