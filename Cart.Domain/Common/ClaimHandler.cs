using MFS.Domain;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace MFS.Domain
{
    public interface IAppClaimHandler
    {
        string GetFromClaim(string Name);

        string GetTenantDbName();

        void SetActiveTenant(int TenantId);

        void SetDbTenantList(IEnumerable<Tenant> tenants);
    }

    public sealed class AppClaimHandler : IAppClaimHandler
    {
        private const string TenantDbClaimName = "TenantDbClaimName";
        private const string TenantIdClaimName = "TenantIdClaimName";

        private readonly IList<Claim> Claims;

        public AppClaimHandler(SignInManager<AppUser> signinManager)
        {
            Claims = signinManager.Context.User.Claims.ToList();
        }

        string IAppClaimHandler.GetFromClaim(string Name)
        {
            return GetValueFromClaim(Name);
        }

        string IAppClaimHandler.GetTenantDbName()
        {
            string tenantId = GetValueFromClaim(TenantIdClaimName);
            string tenantKey = tenantId + ":";
            string tenantDbValues = GetValueFromClaim(TenantDbClaimName);
            string[] tenants = tenantDbValues.Split("|");
            var tenantDbData = tenants.FirstOrDefault(v => v.StartsWith(tenantKey));
            return tenantDbData.Substring(tenantDbData.IndexOf(tenantKey));
        }

        void IAppClaimHandler.SetActiveTenant(int TenantId)
        {
            RemoveOldValue(TenantIdClaimName);
            Claims.Append(new Claim(TenantIdClaimName, TenantId.ToString()));
        }

        void IAppClaimHandler.SetDbTenantList(IEnumerable<Tenant> tenants)
        {
            RemoveOldValue(TenantDbClaimName);
            string value = string.Join("|", tenants.Select(t => t.TenantId + ":" + t.DataBaseName).ToArray());
            Claims.Append(new Claim(TenantDbClaimName, value));
        }

        private void RemoveOldValue(string claimName)
        {
            var claim = Claims.FirstOrDefault(c => c.Type.Equals(claimName));
            if (claim != null)
            {
                Claims.Remove(claim);
            }
        }

        private string GetValueFromClaim(string key)
        {
            var claim = Claims.FirstOrDefault(c =>
               c.Type.Equals(key));

            if (claim != null) return claim.Value;

            return string.Empty;
        }
    }
}
