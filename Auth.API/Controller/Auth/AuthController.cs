using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Auth.Model.Models.Auth;
using Auth.Model.Models.Security;
using Shared.Data.Wrapper;
using Auth.API.Config;
using Auth.API.Middleware;
using Auth.API.Service.Interface.Auth;
using Auth.API.Utility.Http;

namespace Auth.API.Controller.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        [EnableRateLimiting(SecurityRateLimiting.RegistrationPolicy)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);
            return Ok(result);
        }

        [HttpPost("login")]
        [EnableRateLimiting(SecurityRateLimiting.LoginPolicy)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var ipAddress = HttpContext.GetClientIpAddress();
            var userAgent = HttpContext.GetUserAgent();

            var result = await _authService.LoginAsync(request, ipAddress, userAgent);
            return Ok(result);
        }

        [HttpPost("fido2/options/register")]
        [EnableRateLimiting(SecurityRateLimiting.GeneralPolicy)]
        public async Task<IActionResult> RequestAttestationOptions([FromBody] Fido2AttestationOptionsRequest request)
        {
            request.Origin ??= Request.Headers["Origin"].ToString();
            var result = await _authService.RequestAttestationOptionsAsync(request);
            return Ok(result);
        }

        [HttpPost("fido2/register")]
        public async Task<IActionResult> RegisterCredential([FromBody] Fido2RegisterRequest request)
        {
            request.Origin ??= Request.Headers["Origin"].ToString();
            var result = await _authService.RegisterCredentialAsync(request);
            return Ok(result);
        }

        [HttpPost("fido2/challenge")]
        public async Task<IActionResult> CreateFido2Challenge([FromBody] Fido2ChallengeRequest request)
        {
            request.Origin ??= Request.Headers["Origin"].ToString();
            var result = await _authService.CreateFido2ChallengeAsync(request);
            return Ok(result);
        }

        [HttpPost("fido2/verify")]
        public async Task<IActionResult> VerifyFido2Assertion([FromBody] Fido2VerifyRequest request)
        {
            request.Origin ??= Request.Headers["Origin"].ToString();
            var result = await _authService.VerifyFido2AssertionAsync(request);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();

            var result = await _authService.GetCurrentUserAsync(userId.Value);
            return Ok(result);
        }

        [HttpGet("lookup")]
        public async Task<IActionResult> Lookup([FromQuery] string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest(Response<AuthResponse>.Fail("Email is required"));
            }

            var result = await _authService.LookupUserByEmailAsync(email);
            if (!result.Succeeded)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpPost("otp/request")]
        [EnableRateLimiting(SecurityRateLimiting.GeneralPolicy)]
        public async Task<IActionResult> RequestOtp([FromBody] OtpRequest request)
        {
            var result = await _authService.RequestOtpAsync(request);
            return Ok(result);
        }

        [HttpPost("otp/verify")]
        public async Task<IActionResult> VerifyOtp([FromBody] OtpVerifyRequest request)
        {
            var result = await _authService.VerifyOtpAsync(request);
            return Ok(result);
        }

        [HttpPost("auth/refresh")]
        [EnableRateLimiting(SecurityRateLimiting.RefreshTokenPolicy)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var ipAddress = HttpContext.GetClientIpAddress();
            var userAgent = HttpContext.GetUserAgent();

            var enrichedRequest = new RefreshTokenRequest
            {
                AccessToken = request.AccessToken,
                RefreshToken = request.RefreshToken,
                IpAddress = ipAddress,
                UserAgent = userAgent
            };

            var result = await _authService.RefreshTokenAsync(enrichedRequest);
            return Ok(result);
        }
    }
}