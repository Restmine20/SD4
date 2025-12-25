using PaymentService.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace PaymentService.Infrastructure
{
  public class RabbitMqService : BackgroundService
  {
    private readonly IConfiguration _configuration;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly string _queueName;

    private IConnection? _connection;
    private IChannel? _channel;

    public RabbitMqService(
        IConfiguration configuration,
        IServiceScopeFactory serviceScopeFactory)
    {
      _configuration = configuration;
      _serviceScopeFactory = serviceScopeFactory;
      _queueName = configuration["RabbitMQ:Inbound:Queue"]!;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      var factory = new ConnectionFactory
      {
        HostName = _configuration["RabbitMQ:Host"]!,
        Port = int.Parse(_configuration["RabbitMQ:Port"]!),
        UserName = _configuration["RabbitMQ:Username"]!,
        Password = _configuration["RabbitMQ:Password"]!
      };

      _connection = await factory.CreateConnectionAsync(stoppingToken);
      _channel = await _connection.CreateChannelAsync();

      await _channel.QueueDeclareAsync(
          queue: _queueName,
          durable: true,
          exclusive: false,
          autoDelete: false,
          arguments: null,
          cancellationToken: stoppingToken);

      var exchange = _configuration["RabbitMQ:Inbound:BindingExchange"]!;
      var routingKey = _configuration["RabbitMQ:Inbound:RoutingKey"]!;
      await _channel.ExchangeDeclareAsync(exchange, ExchangeType.Direct, durable: true, cancellationToken: stoppingToken);
      await _channel.QueueBindAsync(_queueName, exchange, routingKey, cancellationToken: stoppingToken);

      var consumer = new AsyncEventingBasicConsumer(_channel);
      consumer.ReceivedAsync += async (model, ea) =>
      {
        try
        {
          var body = ea.Body.ToArray();
          var messageStr = Encoding.UTF8.GetString(body);
          var message = JsonSerializer.Deserialize<TransactionalInbox>(messageStr);

          if (message == null)
          {
            await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
            return;
          }

          using var scope = _serviceScopeFactory.CreateScope();
          var handler = scope.ServiceProvider.GetRequiredService<IInboxMessageHandler>();

          await handler.HandleAsync(message, stoppingToken);

          await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
        }
        catch (Exception ex)
        {
          await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false, cancellationToken: stoppingToken);
        }
      };

      await _channel.BasicConsumeAsync(
          queue: _queueName,
          autoAck: false,
          consumer: consumer,
          cancellationToken: stoppingToken);

      await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    public override void Dispose()
    {
      _channel?.CloseAsync();
      _connection?.CloseAsync();
      base.Dispose();
    }
  }
}