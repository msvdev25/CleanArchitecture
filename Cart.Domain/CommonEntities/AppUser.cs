using Microsoft.AspNetCore.Identity;

namespace MFS.Domain
{
	public sealed class AppUser: IdentityUser
	{

		public string FirstName { get; set; }

		public string LastName { get; set; }

    }
}
