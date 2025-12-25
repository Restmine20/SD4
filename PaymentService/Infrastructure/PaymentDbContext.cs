using Microsoft.EntityFrameworkCore;
using PaymentService.Models;

namespace PaymentService.Infrastructure
{


  public class PaymentDbContext : DbContext
  {
    public DbSet<TransactionalInbox> TransactionalInbox { get; set; }
    public DbSet<TransactionalOutbox> TransactionalOutbox { get; set; }

    public DbSet<BankAccount> Accounts { get; set; }


    public PaymentDbContext(DbContextOptions<PaymentDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<BankAccount>(entity =>
      {
        entity.HasKey(o => o.Id);
        entity.Property(o => o.Id).ValueGeneratedNever();

        entity.Property(o => o.UserId)
              .HasConversion(
                  v => v.ToString(),
                  v => Guid.Parse(v))
              .IsRequired()
              .HasMaxLength(40);

        entity.Property(o => o.CreatedAt)
              .HasConversion(
                  v => v.ToString(),
                  v => DateTime.Parse(v))
              .IsRequired()
              .HasMaxLength(20);

        entity.Property(o => o.Balance)
              .HasConversion(
                  v => v.ToString(),
                  v => Decimal.Parse(v))
              .IsRequired();

        entity.ToTable("accounts");
      });

      modelBuilder.Entity<TransactionalInbox>(entity =>
      {
        entity.HasKey(o => o.Id);
        entity.Property(o => o.Id).ValueGeneratedNever();

        entity.Property(o => o.OrderId)
              .HasConversion(
                  v => v.ToString(),
                  v => Guid.Parse(v))
              .IsRequired()
              .HasMaxLength(40);
        entity.Property(o => o.UserId)
              .HasConversion(
                  v => v.ToString(),
                  v => Guid.Parse(v))
              .IsRequired()
              .HasMaxLength(40);

        entity.Property(o => o.TotalAmount)
              .HasConversion(
                  v => v.ToString(),
                  v => Decimal.Parse(v))
              .IsRequired();

        entity.Property(o => o.IsProcessed)
              .IsRequired();

        entity.ToTable("transactional_inboxes");
      });

      modelBuilder.Entity<TransactionalOutbox>(entity =>
      {
        entity.HasKey(o => o.Id);
        entity.Property(o => o.Id).ValueGeneratedNever();

        entity.Property(o => o.OrderId)
              .HasConversion(
                  v => v.ToString(),
                  v => Guid.Parse(v))
              .IsRequired()
              .HasMaxLength(40);

        entity.Property(o => o.IsSuccessful)
              .IsRequired();
        entity.Property(o => o.IsProcessed)
              .IsRequired();

        entity.ToTable("transactional_outboxes");
      });
    }
  }
}