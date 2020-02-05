using MSF.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace MSF.Data
{
    public class TranDbContext : DbContext
    {
        private readonly string connectionString;
        private readonly IHttpContextAccessor httpContextAccessor;

        public TranDbContext(
            DbContextOptions<TranDbContext> options,
            IConfiguration config,
            IHttpContextAccessor httpContextAccessor
            )
            : base(options)
        {            
            connectionString = config.GetConnectionString("TranDbConnection");
            this.httpContextAccessor = httpContextAccessor;
            Database.EnsureCreated();            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string dbName = httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.UserData).Value;

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
