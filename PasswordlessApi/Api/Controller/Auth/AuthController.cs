using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PasswordlessApi.Api.Middleware;
using PasswordlessApi.Api.Models.RequestModel.Auth;
using PasswordlessApi.Api.Models.RequestModel.Security;
using PasswordlessApi.Api.Models.ResponseModel.Auth;
using PasswordlessApi.Api.Service.Interface.Auth;
using PasswordlessApi.Api.Service.Interface.Rbac;
using PasswordlessApi.Api.Common;
using PasswordlessApi.Api.Models.Common;

namespace PasswordlessApi.Api.Controller.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserRoleService _userRoleService;

        public AuthController(IAuthService authService, IUserRoleService userRoleService)
        {
            _authService = authService;
            _userRoleService = userRoleService;
        }

        [HttpPost("register")]
        [EnableRateLimiting(SecurityRateLimiting.RegistrationPolicy)]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);
            return Ok(result);
        }

        [HttpPost("login")]
        [EnableRateLimiting(SecurityRateLimiting.LoginPolicy)]
        public async Task<ActionResult<Response<AuthResponse>>> Login([FromBody] LoginRequest request)
        {
            var ipAddress = GetClientIpAddress();
            var userAgent = GetUserAgent();

            var result = await _authService.LoginAsync(request, ipAddress, userAgent);
            return Ok(result);
        }

        [HttpPost("fido2/options/register")]
        [EnableRateLimiting(SecurityRateLimiting.GeneralPolicy)]
        public async Task<ActionResult<Fido2ChallengeResponse>> RequestAttestationOptions([FromBody] Fido2AttestationOptionsRequest request)
        {
            request.Origin ??= Request.Headers["Origin"].ToString();
            var result = await _authService.RequestAttestationOptionsAsync(request);
            return Ok(result);
        }

        [HttpPost("fido2/register")]
        public async Task<ActionResult<Fido2VerifyResponse>> RegisterCredential([FromBody] Fido2RegisterRequest request)
        {
            request.Origin ??= Request.Headers["Origin"].ToString();
            var result = await _authService.RegisterCredentialAsync(request);
            return Ok(result);
        }

        [HttpPost("fido2/challenge")]
        public async Task<ActionResult<Fido2ChallengeResponse>> CreateFido2Challenge([FromBody] Fido2ChallengeRequest request)
        {
            request.Origin ??= Request.Headers["Origin"].ToString();
            var result = await _authService.CreateFido2ChallengeAsync(request);
            return Ok(result);
        }

        [HttpPost("fido2/verify")]
        public async Task<ActionResult<Fido2VerifyResponse>> VerifyFido2Assertion([FromBody] Fido2VerifyRequest request)
        {
            request.Origin ??= Request.Headers["Origin"].ToString();
            var result = await _authService.VerifyFido2AssertionAsync(request);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<AuthResponse>> Me()
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();

            var userWithRoles = await _userRoleService.GetUserWithRolesAndPermissionsAsync(userId.Value);
            if (userWithRoles == null)
            {
                return NotFound();
            }

            return Ok(new AuthResponse
            {
                UserId = userWithRoles.Id,
                Username = userWithRoles.Username,
                Email = userWithRoles.Email,
                Message = "Authenticated",
                Role = userWithRoles.Role,
                Permissions = userWithRoles.Permissions ?? new List<string>()
            });
        }

        [Authorize]
        [HttpGet("lookup")]
        public async Task<ActionResult<AuthResponse>> Lookup([FromQuery] string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest(new AuthResponse { Message = "Email is required" });
            }

            var user = await _authService.GetUserByEmailAsync(email);
            if (user == null)
            {
                return NotFound(new AuthResponse { Message = "User not found" });
            }

            return Ok(new AuthResponse
            {
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email,
                Message = "User resolved"
            });
        }

        [HttpPost("otp/request")]
        [EnableRateLimiting(SecurityRateLimiting.GeneralPolicy)]
        public async Task<ActionResult<OtpResponse>> RequestOtp([FromBody] OtpRequest request)
        {
            var result = await _authService.RequestOtpAsync(request);
            return Ok(result);
        }

        [HttpPost("otp/verify")]
        public async Task<ActionResult<AuthResponse>> VerifyOtp([FromBody] OtpVerifyRequest request)
        {
            var result = await _authService.VerifyOtpAsync(request);
            return Ok(result);
        }

        [HttpPost("auth/refresh")]
        [EnableRateLimiting(SecurityRateLimiting.RefreshTokenPolicy)]
        public async Task<ActionResult<AuthResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var ipAddress = GetClientIpAddress();
            var userAgent = GetUserAgent();

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

        private string? GetClientIpAddress()
        {
            if (Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
            {
                return forwardedFor.ToString().Split(',')[0].Trim();
            }

            return Request.HttpContext.Connection.RemoteIpAddress?.ToString();
        }

        private string? GetUserAgent()
        {
            return Request.Headers["User-Agent"].ToString();
        }
    }
}
