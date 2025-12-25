using Microsoft.EntityFrameworkCore;

namespace OrderService.Infrastructure
{
  public class OutboxPollingService : BackgroundService
  {
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly TimeSpan _interval;

    public OutboxPollingService(
        IServiceScopeFactory scopeFactory,
        IConfiguration configuration)
    {
      _scopeFactory = scopeFactory;
      _interval = TimeSpan.FromSeconds(int.Parse(configuration["OutboxPolling:IntervalSeconds"] ?? "10"));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      while (!stoppingToken.IsCancellationRequested)
      {
        try
        {
          using var scope = _scopeFactory.CreateScope();
          var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
          var processor = scope.ServiceProvider.GetRequiredService<IOutboxProcessor>();

          var messages = await dbContext.TransactionalOutboxes
              .Where(m => !m.IsProcessed)
              .Take(10)
              .ToListAsync();

          if (messages.Count == 0)
          {
            await Task.Delay(_interval, stoppingToken);
            continue;
          }

          foreach (var message in messages)
          {
            if (stoppingToken.IsCancellationRequested)
              break;

            try
            {
              await processor.ProcessAsync(message, stoppingToken);
              message.IsProcessed = true;

            }
            catch (Exception ex)
            {
            }
          }
          await dbContext.SaveChangesAsync(stoppingToken);
        }
        catch (Exception ex)
        {
        }

        await Task.Delay(_interval, stoppingToken);
      }
    }
  }
}