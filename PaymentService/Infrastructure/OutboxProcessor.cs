using PaymentService.Models;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace PaymentService.Infrastructure
{

  public class OutboxProcessor : IOutboxProcessor
  {
    private readonly PaymentDbContext _dbContext;
    private readonly IConfiguration _configuration;

    private async Task<IConnection> CreateRabbitMQConnectionAsync()
    {
      var factory = new ConnectionFactory()
      {
        HostName = _configuration["RabbitMQ:Host"]!,
        Port = int.Parse(_configuration["RabbitMQ:Port"]!),
        UserName = _configuration["RabbitMQ:Username"]!,
        Password = _configuration["RabbitMQ:Password"]!
      };

      return await factory.CreateConnectionAsync();
    }

    public OutboxProcessor(PaymentDbContext dbContext, IConfiguration configuration)
    {
      _dbContext = dbContext;
      _configuration = configuration;
    }

    public async Task ProcessAsync(TransactionalOutbox message, CancellationToken ct)
    {
      using var connection = await CreateRabbitMQConnectionAsync();
      using var channel = await connection.CreateChannelAsync();

      await channel.ExchangeDeclareAsync(
          exchange: _configuration["RabbitMQ:Outbound:Exchange"]!,
          type: ExchangeType.Direct,
          durable: true);

      var jsonMessage = JsonSerializer.Serialize(message);
      var body = Encoding.UTF8.GetBytes(jsonMessage);

      await channel.BasicPublishAsync(
          exchange: _configuration["RabbitMQ:Outbound:Exchange"]!,
          routingKey: _configuration["RabbitMQ:Outbound:RoutingKey"]!,
          body: body);
    }
  }
}