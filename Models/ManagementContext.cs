using Microsoft.EntityFrameworkCore;

namespace Retail_Banking.Models
{
    public class ManagementContext:DbContext
    {
        public ManagementContext(DbContextOptions<ManagementContext> options) : base(options) { }
        public DbSet<Customer> Customer { get; set; }
        public DbSet<Account> Account { get; set; }
        public DbSet<Transactions> Transactions { get; set; }
        public DbSet<Error> Error { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>().HasKey(x => new { x.CustomerID });
            modelBuilder.Entity<Account>().HasKey(x => new { x.AccountID,x.CustomerID });
            modelBuilder.Entity<Transactions>().HasKey(x => new { x.TransactionID });
            modelBuilder.Entity<Error>().HasKey(x => new { x.ErrorID });
        }
    }
}
