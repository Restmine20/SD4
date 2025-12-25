using Microsoft.EntityFrameworkCore;
using OrderService.Models;

namespace OrderService.Infrastructure
{

  public class OrderDbContext : DbContext
  {
    public DbSet<Order> Orders { get; set; }
    public DbSet<TransactionalOutbox> TransactionalOutboxes { get; set; }


    public OrderDbContext(DbContextOptions<OrderDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<Order>(entity =>
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

        entity.Property(o => o.Status)
              .HasConversion(
                  v => (int)v,
                  v => (OrderStatus)v)
              .IsRequired();

        entity.Property(o => o.Description);

        entity.Property(o => o.TotalAmount)
              .HasConversion(
                  v => v.ToString(),
                  v => Decimal.Parse(v))
              .IsRequired();

        entity.ToTable("orders");
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

        entity.ToTable("transactional_outboxes");
      });
    }
  }
}