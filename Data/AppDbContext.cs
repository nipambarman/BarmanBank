using BarmanBank.Models;
using Microsoft.EntityFrameworkCore;

namespace BarmanBank.Data
{
      public class AppDbContext : DbContext
      {
            public AppDbContext(DbContextOptions<AppDbContext> options)
                : base(options) { }

            public DbSet<User> Users { get; set; }
            public DbSet<Transaction> Transactions { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                  base.OnModelCreating(modelBuilder);

                  modelBuilder.Entity<Transaction>(entity =>
                  {
                        entity.Property(t => t.Amount)
                        .HasColumnType("decimal(18,2)")
                        .IsRequired();

                        entity.Property(t => t.BalanceAfter)
                        .HasColumnType("decimal(18,2)")
                        .IsRequired();

                        entity.Property(t => t.ReferenceId)
                  .IsRequired();

                        entity.Property(t => t.RazorpayOrderId)
                        .IsRequired(false); // mandatory

                        entity.Property(t => t.PaymentGatewayId)
                        .IsRequired(false); // optional, can be null initially



                        entity.Property(t => t.Timestamp)
                        .HasDefaultValueSql("GETUTCDATE()");

                        entity.Property(t => t.CreatedAt)
                        .HasDefaultValueSql("GETUTCDATE()");
                  });
            }
      }
}
