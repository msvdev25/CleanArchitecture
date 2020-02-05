using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MSF.Domain;

namespace MSF.Data
{
	public static class DependencyHandler
	{
		public static void ConfigureDataContext(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddDbContext<UserContext>(options => options
				.UseSqlServer(configuration.GetConnectionString("LoginConnection")));

			// Configure Login context. 
			services.AddScoped<IdentityDbContext<AppUser>, UserContext>();

			services.AddDbContext<TranDbContext>();

			// Configure Transaction DB context.
			services.AddScoped<DbContext, TranDbContext>();

			// Identity for authentication.
			services.AddIdentity<AppUser, IdentityRole>(opt =>
			{
				opt.User.RequireUniqueEmail = true;
				opt.Password.RequiredLength = 6;
				opt.Password.RequireNonAlphanumeric = true;
			})
			.AddEntityFrameworkStores<UserContext>()
			.AddDefaultTokenProviders();
		}
	}
}
