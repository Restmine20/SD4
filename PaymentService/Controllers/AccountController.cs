using Microsoft.AspNetCore.Mvc;
using PaymentService.Models;
using PaymentService.Infrastructure;

namespace PaymentService.Controllers
{
  [ApiController]
  [Route("accounts")]
  public class AccountController : ControllerBase
  {
    private readonly PaymentDbContext _context;

    public AccountController(PaymentDbContext context)
    {
      _context = context;
    }


    [HttpPost]
    public IActionResult CreateAccount([FromForm] Guid userId)
    {
      if (userId == Guid.Empty)
      {
        return BadRequest("UserId is required");
      }

      var existingAccount = _context.Accounts.FirstOrDefault(a => a.UserId == userId);

      if (existingAccount != null)
      {
        return Conflict("Account already exists for this user");
      }

      BankAccount account = BankAccount.Create(userId);

      _context.Accounts.Add(account);
      _context.SaveChanges();

      return Ok(new { Id = account.Id, Balance = account.Balance });
    }


    [HttpPost("deposit")]
    public IActionResult Deposit([FromForm] Guid id, [FromForm] decimal amount)
    {
      if (id == Guid.Empty)
      {
        return BadRequest("Id is required");
      }

      if (amount <= 0)
      {
        return BadRequest("Amount must be > 0");
      }

      BankAccount? account = _context.Accounts.Find(id);

      if (account == null)
      {
        return NotFound("Account not found for this user");
      }

      account.Balance += amount;
      _context.SaveChanges();

      return Ok(new { Balance = account.Balance });
    }


    [HttpGet("{id}")]
    public IActionResult GetBalance(Guid id)
    {
      if (id == Guid.Empty)
      {
        return BadRequest("Id is required");
      }

      BankAccount? account = _context.Accounts.Find(id);

      if (account == null)
      {
        return NotFound("Account not found for this user");
      }

      return Ok(new { Balance = account.Balance });
    }
  }
}
