using MFS.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MFS.Data
{
    public class TranDbContext : DbContext
    {
        private readonly string connectionString;
        private readonly IAppClaimHandler claimHandler;

        public TranDbContext(
            DbContextOptions<TranDbContext> options,
            IConfiguration config,
            IAppClaimHandler claimHandler
            )
            : base(options)
        {
            Database.EnsureCreated();
            connectionString = config.GetConnectionString("TranDbConnection");
            this.claimHandler = claimHandler;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string dbName = claimHandler.GetTenantDbName();

            optionsBuilder.UseSqlServer(string.Format(connectionString, dbName));
            base.OnConfiguring(optionsBuilder);
        }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<Person> Persons { get; set; }

        public DbSet<Customer> Customers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
            modelBuilder.ApplyConfiguration(new PersonConfiguration());
            modelBuilder.ApplyConfiguration(new CustomerConfiguration());
        }


    }
}
