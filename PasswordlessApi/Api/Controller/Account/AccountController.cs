using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PasswordlessApi.Api.Models.RequestModel.Account;
using PasswordlessApi.Api.Models.ResponseModel.Account;
using PasswordlessApi.Api.Service.Interface.Auth;

namespace PasswordlessApi.Api.Controller.Account
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
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (userId <= 0) return Unauthorized();

            var user = await _authService.GetUserByIdAsync(userId);
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
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (userId <= 0) return Unauthorized();

            var result = await _authService.UpdateProfileAsync(userId, request);
            if (result == null) return NotFound();

            return Ok(result);
        }

        [HttpGet("settings")]
        public async Task<ActionResult<AccountSettingsResponse>> GetSettings()
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (userId <= 0) return Unauthorized();

            var result = await _authService.GetAccountSettingsAsync(userId);
            return Ok(result);
        }

        [HttpPut("settings")]
        public async Task<ActionResult<AccountSettingsResponse>> UpdateSettings([FromBody] UpdateSettingsRequest request)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (userId <= 0) return Unauthorized();

            var result = await _authService.UpdateAccountSettingsAsync(userId, request);
            return Ok(result);
        }

        [HttpGet("privacy")]
        public async Task<ActionResult<PrivacySettingsResponse>> GetPrivacy()
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (userId <= 0) return Unauthorized();

            var result = await _authService.GetPrivacySettingsAsync(userId);
            return Ok(result);
        }

        [HttpPut("privacy")]
        public async Task<ActionResult<PrivacySettingsResponse>> UpdatePrivacy([FromBody] UpdatePrivacyRequest request)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (userId <= 0) return Unauthorized();

            var result = await _authService.UpdatePrivacySettingsAsync(userId, request);
            return Ok(result);
        }

        [HttpPost("password-reset")]
        public async Task<ActionResult> RequestPasswordReset([FromBody] PasswordResetRequest request)
        {
            await _authService.RequestPasswordResetAsync(request.Email);
            return Ok(new { succeeded = true, message = "If an account with that email exists, a reset link was sent." });
        }

        [HttpPost("password-reset/confirm")]
        public async Task<ActionResult> ConfirmPasswordReset([FromBody] ConfirmPasswordResetRequest request)
        {
            var result = await _authService.ResetPasswordAsync(request.Token, request.NewPassword);
            if (!result.Succeeded) return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("data-export")]
        public async Task<ActionResult> DownloadData()
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (userId <= 0) return Unauthorized();

            var result = await _authService.GetUserDataExportAsync(userId);
            return Ok(result);
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteAccount()
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (userId <= 0) return Unauthorized();

            var result = await _authService.DeleteAccountAsync(userId);
            if (!result.Succeeded) return BadRequest(result);

            return Ok(result);
        }
    }
}
