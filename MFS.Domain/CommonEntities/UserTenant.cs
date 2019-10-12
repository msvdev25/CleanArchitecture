using System;
using System.Collections.Generic;
using System.Text;

namespace MSF.Domain
{
    public class UserTenant
    {
        public int UserTenantId { get; set; }

        public string AppUserId { get; set; }

        public int TenantId { get; set; }

        public virtual AppUser AppUser { get; set; }

        public virtual Tenant Tenant { get; set; }
    }
}
