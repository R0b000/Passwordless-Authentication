using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Auth.API.Config;
using Auth.API.Middleware;
using Auth.API.Service.Interface.Auth;
using Auth.API.Service.Interface.Security;
using Auth.Model.Models.Account;
using Shared.Data.Wrapper;

namespace Auth.API.Controller.Account
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserProfileService _profileService;
        private readonly IAccountSettingsService _settingsService;
        private readonly IPrivacySettingsService _privacyService;

        public AccountController(
            IAuthService authService,
            IUserProfileService profileService,
            IAccountSettingsService settingsService,
            IPrivacySettingsService privacyService)
        {
            _authService = authService;
            _profileService = profileService;
            _settingsService = settingsService;
            _privacyService = privacyService;
        }

        [HttpGet("profile")]
        public async Task<ActionResult<UserProfileResponse>> GetProfile()
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();

            var result = await _profileService.GetProfileAsync(userId.Value);
            var user = result.Data;
            if (user == null) return NotFound();

            return Ok(result.Data);
        }

        [HttpPut("profile")]
        public async Task<ActionResult<UserProfileResponse>> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();

            var result = await _profileService.UpdateProfileAsync(userId.Value, request);
            if (result.Data == null) return NotFound();

            return Ok(result.Data);
        }

        [HttpGet("settings")]
        public async Task<ActionResult<AccountSettingsResponse>> GetSettings()
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();

            var result = await _settingsService.GetAccountSettingsAsync(userId.Value);
            return Ok(result.Data);
        }

        [HttpPut("settings")]
        public async Task<ActionResult<AccountSettingsResponse>> UpdateSettings([FromBody] UpdateSettingsRequest request)
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();

            var result = await _settingsService.UpdateAccountSettingsAsync(userId.Value, request);
            return Ok(result.Data);
        }

        [HttpGet("privacy")]
        public async Task<ActionResult<PrivacySettingsResponse>> GetPrivacy()
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();

            var result = await _privacyService.GetPrivacySettingsAsync(userId.Value);
            return Ok(result.Data);
        }

        [HttpPut("privacy")]
        public async Task<ActionResult<PrivacySettingsResponse>> UpdatePrivacy([FromBody] UpdatePrivacyRequest request)
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();

            var result = await _privacyService.UpdatePrivacySettingsAsync(userId.Value, request);
            return Ok(result.Data);
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


