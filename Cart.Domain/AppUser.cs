using Microsoft.AspNetCore.Identity;

namespace Cart.Domain
{
	public class AppUser: IdentityUser
	{

		public string FirstName { get; set; }

		public string LastName { get; set; }
	}
}
