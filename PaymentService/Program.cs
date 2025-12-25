
using Microsoft.EntityFrameworkCore;
using PaymentService.Infrastructure;

namespace PaymentService
{
  public class Program
  {
    public static void Main(string[] args)
    {
      var builder = WebApplication.CreateBuilder(args);
      builder.Services.AddDbContext<PaymentDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

      // Регистрация обработчика
      builder.Services.AddScoped<IInboxMessageHandler, InboxMessageHandler>();
      builder.Services.AddScoped<IInboxProcessor, InboxProcessor>();
      builder.Services.AddScoped<IOutboxProcessor, OutboxProcessor>();

      // Регистрация фонового сервиса
      builder.Services.AddHostedService<OutboxPollingService>();
      builder.Services.AddHostedService<RabbitMqService>();
      builder.Services.AddHostedService<InboxPollingService>();

      // Add services to the container.

      builder.Services.AddControllers();
      // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
      builder.Services.AddEndpointsApiExplorer();
      builder.Services.AddSwaggerGen();

      var app = builder.Build();

      using (var scope = app.Services.CreateScope())
      {
        var dbContext = scope.ServiceProvider.GetRequiredService<PaymentDbContext>();
        dbContext.Database.Migrate();
      }
      // Configure the HTTP request pipeline.
      if (app.Environment.IsDevelopment())
      {
        app.UseSwagger();
        app.UseSwaggerUI();
      }

      app.UseHttpsRedirection();

      app.UseAuthorization();


      app.MapControllers();

      app.Run();
    }
  }
}
