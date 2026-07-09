using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PasswordlessApi.Api.Models.RequestModel.Account;
using PasswordlessApi.Api.Models.RequestModel.Security;
using PasswordlessApi.Api.Models.ResponseModel.Security;
using PasswordlessApi.Api.Service.Interface.Auth;

namespace PasswordlessApi.Api.Controller.Security
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
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (userId <= 0) return Unauthorized();

            var result = await _authService.GetSecuritySettingsAsync(userId);
            return Ok(result);
        }

        [HttpPut("settings")]
        public async Task<ActionResult<SecuritySettingsResponse>> UpdateSettings([FromBody] SecuritySettingsResponse request)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (userId <= 0) return Unauthorized();

            var result = await _authService.UpdateSecuritySettingsAsync(userId, request);
            return Ok(result);
        }

        [HttpPost("change-password")]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (userId <= 0) return Unauthorized();

            var result = await _authService.ChangePasswordAsync(userId, request);
            if (!result.Succeeded) return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("2fa/enable")]
        public async Task<ActionResult<SecuritySettingsResponse>> EnableTwoFactor()
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (userId <= 0) return Unauthorized();

            var result = await _authService.EnableTwoFactorAsync(userId);
            return Ok(result);
        }

        [HttpPost("2fa/disable")]
        public async Task<ActionResult<SecuritySettingsResponse>> DisableTwoFactor()
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (userId <= 0) return Unauthorized();

            var result = await _authService.DisableTwoFactorAsync(userId);
            return Ok(result);
        }

        [HttpGet("activity")]
        public async Task<ActionResult<ActivityLogResponse>> GetActivity([FromQuery] ActivityQueryRequest query)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (userId <= 0) return Unauthorized();

            var result = await _authService.GetActivityLogsAsync(userId, query);
            return Ok(result);
        }

        [HttpPost("device/verify")]
        public async Task<ActionResult> VerifyDevice([FromBody] VerifyDeviceRequest request)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (userId <= 0) return Unauthorized();

            var result = await _authService.VerifyDeviceAsync(userId, request);
            return Ok(result);
        }
    }
}
