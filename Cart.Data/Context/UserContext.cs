using MFS.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MFS.Data
{
    public class UserContext : IdentityDbContext<AppUser>
    {
        public UserContext(DbContextOptions<UserContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Tenant> Tenants { get; set; }

        public DbSet<UserTenant> UserTenants { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new UserConfiguration());
            builder.ApplyConfiguration(new TenantConfiguration());
            base.OnModelCreating(builder);

            // Seed Roles.
            foreach (string r in Global.RoleList)
            {
                builder.Entity<IdentityRole>().HasData(new IdentityRole { Name = r, NormalizedName = r.ToUpper() });
            }
        }
    }
}
