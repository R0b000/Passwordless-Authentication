using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Auth.API.Config;
using Auth.API.Middleware;
using Auth.API.Service.Interface.Auth;
using Auth.Model.Models.Account;
using Auth.Model.Models.Auth;
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
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();

            var result = await _profileService.GetProfileAsync(userId.Value);
            if (!result.Succeeded || result.Data == null) return NotFound(result);

            return Ok(result);
        }

        [HttpPut("profile")]
        [HttpPost("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();

            var result = await _profileService.UpdateProfileAsync(userId.Value, request);
            return Ok(result);
        }

        [HttpGet("settings")]
        public async Task<IActionResult> GetSettings()
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();

            var result = await _settingsService.GetAccountSettingsAsync(userId.Value);
            return Ok(result);
        }

        [HttpPut("settings")]
        public async Task<IActionResult> UpdateSettings([FromBody] UpdateSettingsRequest request)
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();

            var result = await _settingsService.UpdateAccountSettingsAsync(userId.Value, request);
            return Ok(result);
        }

        [HttpGet("privacy")]
        public async Task<IActionResult> GetPrivacy()
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();

            var result = await _privacyService.GetPrivacySettingsAsync(userId.Value);
            return Ok(result);
        }

        [HttpPut("privacy")]
        public async Task<IActionResult> UpdatePrivacy([FromBody] UpdatePrivacyRequest request)
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();

            var result = await _privacyService.UpdatePrivacySettingsAsync(userId.Value, request);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("password-reset")]
        [EnableRateLimiting(SecurityRateLimiting.GeneralPolicy)]
        public async Task<IActionResult> RequestPasswordReset([FromBody] ForgotPasswordRequest request)
        {
            var result = await _authService.RequestPasswordResetAsync(request.Email);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("password-reset/confirm")]
        [EnableRateLimiting(SecurityRateLimiting.GeneralPolicy)]
        public async Task<IActionResult> ConfirmPasswordReset([FromBody] ResetPasswordRequest request)
        {
            var result = await _authService.ResetPasswordAsync(request.Email, request.Otp, request.NewPassword);
            if (!result.Succeeded) return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("data-export")]
        public async Task<IActionResult> DownloadData()
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();

            var result = await _authService.GetUserDataExportAsync(userId.Value);
            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAccount()
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();

            var result = await _authService.DeleteAccountAsync(userId.Value);
            if (!result.Succeeded) return BadRequest(result);

            return Ok(result);
        }
    }
}