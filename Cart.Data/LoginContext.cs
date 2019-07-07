using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Cart.Domain;

namespace Cart.Data
{
    public class LoginContext : IdentityDbContext<AppUser>
    {
        public LoginContext(DbContextOptions<LoginContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        //DbSet<AppUser> ApplicatoinUser {get;set;}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new UserConfiguration());
            base.OnModelCreating(builder);
        }
    }
}
