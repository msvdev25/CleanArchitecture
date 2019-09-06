using MFS.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MFS.Repo
{
    public interface ITenantHandler
    {
        Task<int> SaveTenantAsync(Tenant tenant);

        Task<IEnumerable<Tenant>> GetAllTenantsAsync();

        Task<Tenant> GetTenantAsync(string TenantName);

        Task<IEnumerable<Tenant>> GetUserTenantsAsync(string userId);

        Task SaveUserTenantAsync(UserTenant userTenant);
    }

    internal class TenantHandler : ITenantHandler
    {

        private DbSet<Tenant> Tenants;
        private DbSet<UserTenant> UserTenants;
        private DbContext DbContext;

        public TenantHandler(IdentityDbContext<AppUser> dbContext)
        {
            DbContext = dbContext;
            Tenants = dbContext.Set<Tenant>();
            UserTenants = dbContext.Set<UserTenant>();
        }

        async Task<int> ITenantHandler.SaveTenantAsync(Tenant tenant)
        {
            if (tenant.TenantId < 1)
                await Tenants.AddAsync(tenant);
            else
                Tenants.Attach(tenant).State = EntityState.Modified;

            await DbContext.SaveChangesAsync();
            return tenant.TenantId;
        }

        async Task<IEnumerable<Tenant>> ITenantHandler.GetAllTenantsAsync()
        {
            return await Tenants.ToListAsync();
        }

        async Task<Tenant> ITenantHandler.GetTenantAsync(string TenantName)
        {
            return await Tenants.FirstOrDefaultAsync(t => t.TenantName.Equals(TenantName));
        }

        async Task<IEnumerable<Tenant>> ITenantHandler.GetUserTenantsAsync(string userId)
        {
            return await UserTenants.Include(t => t.Tenant)
                .Where(u => u.AppUserId == userId).Select(ut => ut.Tenant)
                .ToListAsync();
        }

        async Task ITenantHandler.SaveUserTenantAsync(UserTenant userTenant)
        {
            if (userTenant.UserTenantId < 1)
                await UserTenants.AddAsync(userTenant);
            else
                UserTenants.Attach(userTenant).State = EntityState.Modified;

            await DbContext.SaveChangesAsync();
        }

    }
}
