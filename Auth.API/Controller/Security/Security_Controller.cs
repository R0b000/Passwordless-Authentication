using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Auth.Model.Models.Account;
using Auth.Model.Models.Security;
using Auth.Model.Models.Rbac;
using Shared.Data.Wrapper;
using Auth.API.Config;
using Auth.API.Service.Interface.Auth;

namespace Auth.API.Controller.Security
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SecurityController : ControllerBase
    {
        private readonly IAuthService _authService;

        public SecurityController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet("settings")]
        public async Task<ActionResult<SecuritySettingsResponse>> GetSettings()
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();

            var result = await _authService.GetSecuritySettingsAsync(userId.Value);
            return Ok(result);
        }

        [HttpPut("settings")]
        public async Task<ActionResult<SecuritySettingsResponse>> UpdateSettings([FromBody] SecuritySettingsResponse request)
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();

            var result = await _authService.UpdateSecuritySettingsAsync(userId.Value, request);
            return Ok(result);
        }

        [HttpPost("change-password")]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();

            var result = await _authService.ChangePasswordAsync(userId.Value, request);
            if (!result.Succeeded) return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("2fa/enable")]
        public async Task<ActionResult<SecuritySettingsResponse>> EnableTwoFactor()
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();

            var result = await _authService.EnableTwoFactorAsync(userId.Value);
            return Ok(result);
        }

        [HttpPost("2fa/disable")]
        public async Task<ActionResult<SecuritySettingsResponse>?> DisableTwoFactor()
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();

            var result = await _authService.DisableTwoFactorAsync(userId.Value);
            return Ok(result);
        }

        [HttpGet("activity")]
        public async Task<ActionResult<ActivityLogResponse>> GetActivity([FromQuery] ActivityQueryRequest query)
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();

            var result = await _authService.GetActivityLogsAsync(userId.Value, query);
            return Ok(result);
        }

        [HttpPost("device/verify")]
        public async Task<ActionResult> VerifyDevice([FromBody] VerifyDeviceRequest request)
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();

            var result = await _authService.VerifyDeviceAsync(userId.Value, request);
            return Ok(result);
        }
    }
}


