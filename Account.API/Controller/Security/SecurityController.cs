using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Auth.API.Config;
using Auth.API.Service.Interface.Auth;
using Account.API.Service.Interface;
using Shared.API.Service.Interface;
using Auth.Model.Models.Account;
using Auth.Model.Models.Security;
using Shared.Data.Wrapper;

namespace Account.API.Controller.Security
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SecurityController : ControllerBase
    {
        private readonly ISecurityService _securityService;

        public SecurityController(ISecurityService securityService)
        {
            _securityService = securityService;
        }

        [HttpGet("settings")]
        public async Task<IActionResult> GetSettings()
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();

            var result = await _securityService.GetSecuritySettingsAsync(userId.Value);
            return Ok(result);
        }

        [HttpPut("settings")]
        public async Task<IActionResult> UpdateSettings([FromBody] SecuritySettingsResponse request)
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();

            var result = await _securityService.UpdateSecuritySettingsAsync(userId.Value, request);
            return Ok(result);
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();

            var result = await _securityService.ChangePasswordAsync(userId.Value, request);
            if (!result.Succeeded) return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("2fa/enable")]
        public async Task<IActionResult> EnableTwoFactor()
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();

            var result = await _securityService.EnableTwoFactorAsync(userId.Value);
            return Ok(result);
        }

        [HttpPost("2fa/disable")]
        public async Task<IActionResult> DisableTwoFactor()
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();

            var result = await _securityService.DisableTwoFactorAsync(userId.Value);
            return Ok(result);
        }

        [HttpGet("activity")]
        public async Task<IActionResult> GetActivity([FromQuery] ActivityQueryRequest query)
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();

            var result = await _securityService.GetActivityLogsAsync(userId.Value, query);
            return Ok(result);
        }

        [HttpPost("device/verify")]
        public async Task<IActionResult> VerifyDevice([FromBody] VerifyDeviceRequest request)
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();

            var result = await _securityService.VerifyDeviceAsync(userId.Value, request);
            return Ok(result);
        }
    }
}
