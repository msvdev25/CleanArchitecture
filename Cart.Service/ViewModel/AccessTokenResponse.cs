using MFS.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace MFS.Service
{
    public sealed class AccessTokenResponse
    {
        public string AccessToken { get; set; }

        public DateTime Expireation { get; set; }

        public IEnumerable<Tenant> UserTenants { get; set; }
    }
}
