using Charlie.Payment.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace Charlie.Payment.DataAccess;

public class PaymentDbContext : DbContext
{

    public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options) { }

    public PaymentDbContext()
    {
        
    }
    public DbSet<PaymentModel> Payments { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<PaymentModel>();

        base.OnModelCreating(modelBuilder);
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
           
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=PaymentDb;Trusted_Connection=True;");
        }
    }
}