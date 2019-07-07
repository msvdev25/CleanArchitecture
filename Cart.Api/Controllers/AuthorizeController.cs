using Cart.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Cart.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthorizeController : ControllerBase
    {
		#region Init

		private IConfiguration _config;
		private readonly SignInManager<AppUser> _signinManager;
        private readonly IHostingEnvironment env;

        public AuthorizeController(IConfiguration config,
			SignInManager<AppUser> signinManager,
            IHostingEnvironment env)
		{
			_config = config;
			_signinManager = signinManager;
            this.env = env;
        }

		#endregion

		#region Users

		[HttpPost("GetToken")]
		public async Task<IActionResult> GetAccessToken([FromBody]UserViewModel userModel)
		{
			IActionResult response = Unauthorized();

			AppUser user = null;

			if (!string.IsNullOrEmpty(userModel.UserEmail))
				user = await _signinManager.UserManager.FindByEmailAsync(userModel.UserEmail);
			else if (!string.IsNullOrEmpty(userModel.UserName))
				user = await _signinManager.UserManager.FindByNameAsync(userModel.UserName);

            if (user != null)
            {
                var result = await _signinManager.CheckPasswordSignInAsync(user, userModel.Password, false);

                if (result.Succeeded)
                {
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                    var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Email, user.Email),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.UniqueName,user.UserName)
                    };

                    var token = new JwtSecurityToken(
                        _config["Jwt:Issuer"],
                        _config["Jwt:Audience"],
                        claims,
                        expires: DateTime.UtcNow.AddDays(1),
                        signingCredentials: creds);

                    response = Ok(new
                    {
                        Token = new JwtSecurityTokenHandler().WriteToken(token),
                        Expiration = token.ValidTo
                    });
                }
            }
            else if (env.IsDevelopment()) {
                return Ok(new {
                    Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6IlN1cGVyVXNlckBTbWFydEVtcGxveWVlLmNvbSIsImp0aSI6ImFiYjZhOWYxLWMxMmEtNDZhMS05YjZiLTU3ZjAzZDJkMGIwZSIsInVuaXF1ZV9uYW1lIjoiU3VwZXJVc2VyIiwiZXhwIjoxNTU4NTE3NjUzLCJpc3MiOiJodHRwOi8vbG9jYWxob3N0OjYwNjAvIiwiYXVkIjoiaHR0cDovL2xvY2FsaG9zdDo2MDYwLyJ9.fhjS3VKDbVO6ZQX-aZNxXo7JV6tmt0Ao64LL6Y63wYo",
                    Expiration = DateTime.UtcNow.AddDays(1)
                });
            }

			return response;
		}

		[HttpPost("Register")]
		public async Task<IActionResult> RegisterUser([FromBody]UserViewModel userModel)
		{

			var newUser = new AppUser()
			{
				FirstName = userModel.FirstName,
				LastName = userModel.LastName,
				Email = userModel.UserEmail,
				UserName = userModel.UserName,
			};

			// Create the user.
			var result = await _signinManager.UserManager.CreateAsync(newUser, userModel.Password);

			if (result.Succeeded)
			{				
				return Ok();
			}

			return StatusCode(500, result.Errors);
		}

		[HttpGet("isUniqueMail")]
		public async Task<bool> IsUniqueEmail(string eMail, string userId)
		{
			var user = await _signinManager.UserManager.FindByEmailAsync(eMail);
			if (user == null)
				return true;
			else
			{
				return (user.Id != userId);
			}
		}

		[HttpGet("isUniqueUsername")]
		public async Task<bool> IsUniqueUsername(string userName, string userId)
		{
			var user = await _signinManager.UserManager.FindByNameAsync(userName);
			if (user == null)
				return true;
			else
			{
				return (user.Id != userId);
			}
		}

		#endregion
	}
}