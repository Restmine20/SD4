
namespace APIGateway
{
  public class Program
  {
    public static void Main(string[] args)
    {
      var builder = WebApplication.CreateBuilder(args);

      // Add services to the container.

      builder.Services.AddHttpClient("OrderService", client =>
      {
        client.BaseAddress = new Uri("http://order:8080/");
      });

      builder.Services.AddHttpClient("PaymentService", client =>
      {
        client.BaseAddress = new Uri("http://payment:8080/");
      });



      builder.Services.AddCors(options =>
      {
        options.AddPolicy("AllowFrontend", policy =>
        {
          policy.WithOrigins("http://localhost:8083") // ? порт твоего фронтенда
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
      });


      builder.Services.AddControllers();
      // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
      builder.Services.AddEndpointsApiExplorer();
      builder.Services.AddSwaggerGen();

      var app = builder.Build();

      // Configure the HTTP request pipeline.
      if (app.Environment.IsDevelopment())
      {
        app.UseSwagger();
        app.UseSwaggerUI();
      }

      app.UseHttpsRedirection();



      app.UseRouting();
      app.UseCors("AllowFrontend");





      app.UseAuthorization();


      app.MapControllers();

      app.Run();
    }
  }
}
