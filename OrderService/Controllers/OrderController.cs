using Microsoft.AspNetCore.Mvc;
using OrderService.Infrastructure;
using OrderService.Models;

namespace OrderService.Controllers
{
  [ApiController]
  [Route("orders")]
  public class OrderController : ControllerBase
  {
    private readonly OrderDbContext _context;

    public OrderController(OrderDbContext context, IWebHostEnvironment environment, IConfiguration configuration)
    {
      _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromForm] Guid userId, [FromForm] string? description, [FromForm] decimal totalAmount)
    {
      if (userId == Guid.Empty)
      {
        return BadRequest("UserId is required");
      }
      if (totalAmount < 0)
      {
        return BadRequest("TotalAmount must be >= 0");
      }

      Order order = Order.Create(userId, description, totalAmount);
      TransactionalOutbox transactionalOutbox = TransactionalOutbox.Create(order.Id, userId, totalAmount);

      using var transaction = await _context.Database.BeginTransactionAsync();
      try
      {
        _context.Orders.Add(order);
        _context.TransactionalOutboxes.Add(transactionalOutbox);

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
      }
      catch
      {
        await transaction.RollbackAsync();
        throw;
      }

      return Ok(new { Id = order.Id, Status = order.Status });
    }

    [HttpGet("user/{user_id}")]
    public IActionResult GetOrders(Guid user_id)
    {
      if (user_id == Guid.Empty)
      {
        return BadRequest("UserID must be not null");
      }

      var orders = _context.Orders
          .Where(f => f.UserId == user_id)
          .Select(f => new
          {
            Id = f.Id,
            Status = f.Status,
            Description = f.Description,
            CreationDate = f.CreatedAt,
            TotalAmount = f.TotalAmount
          })
          .ToList();

      return Ok(orders);
    }

    [HttpGet("{order_id}")]
    public IActionResult GetOrder(Guid order_id)
    {
      var order = _context.Orders.Find(order_id);

      if (order == null)
      {
        return NotFound(new { error = "Order not found" });
      }

      return Ok(new
      {
        UserId = order.UserId,
        Status = order.Status,
        Description = order.Description,
        CreationDate = order.CreatedAt,
        TotalAmount = order.TotalAmount
      });
    }
  }
}