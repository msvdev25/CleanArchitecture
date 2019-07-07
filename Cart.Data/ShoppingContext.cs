using Microsoft.EntityFrameworkCore;
using Cart.Domain;

namespace Cart.Data
{
	public class ShoppingContext : DbContext
	{

		public ShoppingContext(DbContextOptions<ShoppingContext> options)
			:base(options)
		{
			Database.EnsureCreated();
		}

		public DbSet<Category> Categories { get; set; }

		public DbSet<Product> Products { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
		}
	}
}
