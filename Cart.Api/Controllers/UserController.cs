using MFS.Domain;
using MFS.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace MFS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        #region Init

        private readonly ICommonService _commonService;

        public UserController(
            ICommonService commonService)
        {
            this._commonService = commonService;
        }

        #endregion

        #region Users

        [HttpPost("GetToken")]
        public async Task<IActionResult> GetAccessToken([FromBody]LoginViewModel userModel)
        {
            IActionResult response = Unauthorized();

            try
            {
                var access = await _commonService.GetAccessToken(userModel);
                response = Ok(access);
            }
            catch (Exception ex)
            {
                response = StatusCode(500, ex.Message);
            }

            return response;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterUser([FromBody]UserViewModel userModel)
        {
            if (string.IsNullOrEmpty(userModel.UserEmail) ||
                string.IsNullOrEmpty(userModel.Password))
                return StatusCode(500, "UserEmail/Password id required");

            try
            {
                if (await _commonService.RegisterUser(userModel))
                    return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(400, ex.Message);
            }

            return StatusCode(500);
        }

        [HttpGet("isUniqueMail")]
        public async Task<bool> IsUniqueEmail(string eMail, string userId)
        {
            return await _commonService.IsUniqueEmail(eMail, userId);

        }

        #endregion

        #region Tenant

        [HttpPost("AddTenant")]
        public async Task<IActionResult> AddTenant(Tenant tenant)
        {
            await _commonService.SaveTenant(tenant);
            return Ok();
        }

        [HttpGet("GetTenants")]
        public async Task<IActionResult> GetAllTenants()
        {
            return Ok(await _commonService.GetTenants());
        }

        [HttpPost("AssignUserToTenant")]
        public async Task AssignUserToTenant([FromBody]UserTenantViewModel userTenant)
        {
            await _commonService.AssignUserToTenant(userTenant);
        }

        #endregion
    }
}