using OrderService.Models;

namespace OrderService.Infrastructure
{
  public interface IOutboxProcessor
  {
    Task ProcessAsync(TransactionalOutbox message, CancellationToken ct);
  }
}