using System.Collections.Generic;

namespace MFS.Service
{
    public class LoginViewModel
    {
        public string UserEmail { get; set; }
        public string Password { get; set; }
    }

    public class UserViewModel
    {
        public UserViewModel()
        {
            Roles = new List<Role>();
            UserTenants = new List<int>();
        }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Password { get; set; }

        public string UserEmail { get; set; }

        public IList<Role> Roles { get; set; }

        public IList<int> UserTenants{ get; set; }
    }

    public class UserTenantViewModel
    {
        public string  TenantName { get; set; }

        public string UserName { get; set; }
    }
}
