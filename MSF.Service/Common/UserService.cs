using MSF.Domain;
using MSF.Repo;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MSF.Service
{
	public interface IUserService
	{
		Task<AccessTokenResponse> GetAccessToken(LoginViewModel viewModel);

		Task<bool> RegisterUser(UserViewModel user);

		Task AssignUserToTenant(UserTenantViewModel userTenant);

		Task<bool> IsUniqueEmail(string EMail, string UserId);

		Task<IEnumerable<Tenant>> GetTenants();

		Task<int> SaveTenant(Tenant tenant);

		Task SetActiveTenant(Tenant tenant);
	}

	internal class UserService : IUserService
	{
		private readonly ITenantHandler _tenanaRepository;
		private readonly SignInManager<AppUser> _signinManager;
		private readonly IConfiguration _config;
		private readonly ITenantHandler _tenantHandler;

		public UserService(
			IServiceProvider serviceProvider,
			SignInManager<AppUser> signinManager,
			IConfiguration config,
			ITenantHandler tenantHandler
			)
		{
			_tenanaRepository = (ITenantHandler)serviceProvider.GetService(typeof(ITenantHandler));
			this._signinManager = signinManager;
			this._config = config;
			this._tenantHandler = tenantHandler;
		}

		async Task<AccessTokenResponse> IUserService.GetAccessToken(LoginViewModel viewModel)
		{
			AppUser user = null;

			if (!string.IsNullOrEmpty(viewModel.UserEmail))
				user = await _signinManager.UserManager.FindByEmailAsync(viewModel.UserEmail);

			if (user != null)
			{
				var result = await _signinManager.CheckPasswordSignInAsync(user, viewModel.Password, false);

				if (result.Succeeded)
				{
					var userTenants = await _tenantHandler.GetUserTenantsAsync(user.Id);

					if (userTenants.Count() < 1)
						throw new Exception("User does not belongs to any of the Tanent..");

					var userRoles = await _signinManager.UserManager.GetRolesAsync(user);

					var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
					var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

					string currentUserRole = userRoles.FirstOrDefault() ?? Role.ReadOnly.ToString();

					var claims = new[] {
						new Claim(JwtRegisteredClaimNames.Email, user.Email),
						new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
						new Claim(JwtRegisteredClaimNames.GivenName,user.UserName),
						new Claim(ClaimTypes.Role, currentUserRole),
						new Claim(ClaimTypes.UserData, userTenants.First().DataBaseName)
					};

					var token = new JwtSecurityToken(
						_config["Jwt:Issuer"],
						_config["Jwt:Audience"],
						claims,
						expires: DateTime.UtcNow.AddMinutes(120),
						signingCredentials: creds);
					
					return new AccessTokenResponse
					{
						AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
						Expireation = token.ValidTo,
						UserTenants = userTenants
					};
				}
				else
					throw new Exception("Password is wrong!");
			}

			throw new Exception("User not available");
		}

		async Task<bool> IUserService.RegisterUser(UserViewModel userModel)
		{
			if (string.IsNullOrEmpty(userModel.UserEmail) || string.IsNullOrEmpty(userModel.Password))
				throw new Exception("User Email/Password is required");

			var newUser = new AppUser()
			{
				FirstName = userModel.FirstName,
				LastName = userModel.LastName,
				UserName = userModel.UserEmail,
				Email = userModel.UserEmail,
				NormalizedEmail = userModel.UserEmail,
				PasswordHash = userModel.Password
			};

			// Create the user.
			var result = await _signinManager.UserManager.CreateAsync(newUser, userModel.Password);

			if (result.Succeeded)
			{

				await _signinManager.UserManager.AddToRolesAsync(newUser,
					userModel.Roles.Select(r => r.ToString()).ToArray());
				return true;
			}
			else
			{
				throw new Exception(result.Errors.First().Description);
			}

		}

		async Task<bool> IUserService.IsUniqueEmail(string eMail, string userId)
		{
			var user = await _signinManager.UserManager.FindByEmailAsync(eMail);
			return (user == null) || (user.Id != userId);
		}

		async Task<IEnumerable<Tenant>> IUserService.GetTenants()
		{
			return await _tenanaRepository.GetAllTenantsAsync();
		}

		async Task<int> IUserService.SaveTenant(Tenant tenant)
		{
			return await _tenanaRepository.SaveTenantAsync(tenant);
		}

		async Task IUserService.AssignUserToTenant(UserTenantViewModel userTenant)
		{
			var user = await _signinManager.UserManager.FindByEmailAsync(userTenant.UserName);
			if (user == null)
				throw new Exception("User does not exist");

			var tenant = await _tenantHandler.GetTenantAsync(userTenant.TenantName);
			if (tenant == null)
				throw new Exception("Tenant does not exist");

			await _tenantHandler.SaveUserTenantAsync(new UserTenant { AppUserId = user.Id, TenantId = tenant.TenantId });
		}

		async Task IUserService.SetActiveTenant(Tenant tenant)
		{
			// Update Transaction DB name in claims.
			var user = await _signinManager.UserManager.GetUserAsync(_signinManager.Context.User);
			var claim = _signinManager.Context.User.Claims.First(c => c.Value == ClaimTypes.UserData);

			await _signinManager.UserManager.ReplaceClaimAsync(user, claim , new Claim(ClaimTypes.UserData, tenant.DataBaseName));
		}
	}
}