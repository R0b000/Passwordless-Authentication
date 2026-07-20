using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Auth.API.Config;
using Auth.API.Middleware;
using Auth.API.Service.Interface.Auth;
using Shared.Core.Models.ResponseModel.Account;
using Shared.Core.Models.RequestModel.Account;

namespace Auth.API.Controller.Account
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet("profile")]
        public async Task<ActionResult<UserProfileResponse>> GetProfile()
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();

            var userResult = await _authService.GetUserByIdAsync(userId.Value);
            var user = userResult.Data;
            if (user == null) return NotFound();

            var result = new UserProfileResponse
            {
                UserId = user.Id,
                Username = user.Username ?? string.Empty,
                Email = user.Email ?? string.Empty,
                DateJoined = user.CreatedAt,
                AccountStatus = "active"
            };

            return Ok(result);
        }

        [HttpPut("profile")]
        public async Task<ActionResult<UserProfileResponse>> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();

            var result = await _authService.UpdateProfileAsync(userId.Value, request);
            if (result == null) return NotFound();

            return Ok(result);
        }

        [HttpGet("settings")]
        public async Task<ActionResult<AccountSettingsResponse>> GetSettings()
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();

            var result = await _authService.GetAccountSettingsAsync(userId.Value);
            return Ok(result);
        }

        [HttpPut("settings")]
        public async Task<ActionResult<AccountSettingsResponse>> UpdateSettings([FromBody] UpdateSettingsRequest request)
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();

            var result = await _authService.UpdateAccountSettingsAsync(userId.Value, request);
            return Ok(result);
        }

        [HttpGet("privacy")]
        public async Task<ActionResult<PrivacySettingsResponse>> GetPrivacy()
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();

            var result = await _authService.GetPrivacySettingsAsync(userId.Value);
            return Ok(result);
        }

        [HttpPut("privacy")]
        public async Task<ActionResult<PrivacySettingsResponse>> UpdatePrivacy([FromBody] UpdatePrivacyRequest request)
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();

            var result = await _authService.UpdatePrivacySettingsAsync(userId.Value, request);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("password-reset")]
        [EnableRateLimiting(SecurityRateLimiting.GeneralPolicy)]
        public async Task<ActionResult> RequestPasswordReset([FromBody] PasswordResetRequest request)
        {
            await _authService.RequestPasswordResetAsync(request.Email);
            return Ok(new { succeeded = true, message = "If an account with that email exists, a reset link was sent." });
        }

        [AllowAnonymous]
        [HttpPost("password-reset/confirm")]
        [EnableRateLimiting(SecurityRateLimiting.GeneralPolicy)]
        public async Task<ActionResult> ConfirmPasswordReset([FromBody] ConfirmPasswordResetRequest request)
        {
            var result = await _authService.ResetPasswordAsync(request.Token, request.NewPassword);
            if (!result.Succeeded) return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("data-export")]
        public async Task<ActionResult> DownloadData()
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();

            var result = await _authService.GetUserDataExportAsync(userId.Value);
            return Ok(result);
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteAccount()
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();

            var result = await _authService.DeleteAccountAsync(userId.Value);
            if (!result.Succeeded) return BadRequest(result);

            return Ok(result);
        }
    }
}
