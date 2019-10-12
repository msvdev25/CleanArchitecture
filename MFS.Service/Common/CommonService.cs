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
    public interface ICommonService
    {
        Task<AccessTokenResponse> GetAccessToken(LoginViewModel viewModel);

        Task<bool> RegisterUser(UserViewModel user);

        Task AssignUserToTenant(UserTenantViewModel userTenant);

        Task<bool> IsUniqueEmail(string EMail, string UserId);

        Task<IEnumerable<Tenant>> GetTenants();

        Task<int> SaveTenant(Tenant tenant);
    }

    internal class CommonService : ICommonService
    {
        private readonly ITenantHandler _commonRepository;
        private readonly SignInManager<AppUser> _signinManager;
        private readonly IConfiguration _config;
        private readonly IAppClaimHandler _claimHandler;
        private readonly ITenantHandler _tenantHandler;

        public CommonService(
            IServiceProvider serviceProvider,
            SignInManager<AppUser> signinManager,
            IConfiguration config,
            IAppClaimHandler claimHandler,
            ITenantHandler tenantHandler
            )
        {
            _commonRepository = (ITenantHandler)serviceProvider.GetService(typeof(ITenantHandler));
            this._signinManager = signinManager;
            this._config = config;
            this._claimHandler = claimHandler;
            this._tenantHandler = tenantHandler;
        }

        async Task<AccessTokenResponse> ICommonService.GetAccessToken(LoginViewModel viewModel)
        {
            AppUser user = null;

            if (!string.IsNullOrEmpty(viewModel.UserEmail))
                user = await _signinManager.UserManager.FindByEmailAsync(viewModel.UserEmail);

            if (user != null)
            {
                var result = await _signinManager.CheckPasswordSignInAsync(user, viewModel.Password, false);

                if (result.Succeeded)
                {
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                    var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Email, user.Email, _config["Jwt:Issuer"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString(), _config["Jwt:Issuer"]),
                        new Claim(JwtRegisteredClaimNames.UniqueName,user.UserName, _config["Jwt:Issuer"])
                    };

                    var token = new JwtSecurityToken(
                        _config["Jwt:Issuer"],
                        _config["Jwt:Audience"],
                        claims,
                        expires: DateTime.UtcNow.AddDays(1),
                        signingCredentials: creds);

                    var userTenants = await _tenantHandler.GetUserTenantsAsync(user.Id);
                    _claimHandler.SetDbTenantList(userTenants);

                    if (userTenants.Count() > 0)
                        _claimHandler.SetActiveTenant(userTenants.FirstOrDefault().TenantId);
                    else
                        throw new Exception("User does not belongs to any Tanent..");

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

        async Task<bool> ICommonService.RegisterUser(UserViewModel userModel)
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

        async Task<bool> ICommonService.IsUniqueEmail(string eMail, string userId)
        {
            var user = await _signinManager.UserManager.FindByEmailAsync(eMail);
            return (user == null) || (user.Id != userId);
        }

        async Task<IEnumerable<Tenant>> ICommonService.GetTenants()
        {
            return await _commonRepository.GetAllTenantsAsync();
        }

        async Task<int> ICommonService.SaveTenant(Tenant tenant)
        {
            return await _commonRepository.SaveTenantAsync(tenant);
        }

        async Task ICommonService.AssignUserToTenant(UserTenantViewModel userTenant)
        {
            var user = await _signinManager.UserManager.FindByEmailAsync(userTenant.UserName);
            if (user == null)
                throw new Exception("User does not exist");

            var tenant = await _tenantHandler.GetTenantAsync(userTenant.TenantName);
            if (tenant == null)
                throw new Exception("Tenant does not exist");

            await _tenantHandler.SaveUserTenantAsync(new UserTenant { AppUserId = user.Id, TenantId = tenant.TenantId });
        }
    }
}