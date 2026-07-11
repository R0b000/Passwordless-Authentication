using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PasswordlessApi.Api.Models.RequestModel.Auth;
using PasswordlessApi.Api.Models.ResponseModel.Auth;
using PasswordlessApi.Api.Service.Interface.Auth;
using PasswordlessApi.Api.Service.Interface.Rbac;

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
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);
            return Ok(result);
        }

        [HttpPost("login")]
        [EnableRateLimiting(SecurityRateLimiting.LoginPolicy)]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
        {
            var ipAddress = GetClientIpAddress();
            var userAgent = GetUserAgent();

            if (!string.IsNullOrEmpty(ipAddress) && await _rateLimiter.IsLimitedAsync($"login:{ipAddress}", 5, TimeSpan.FromMinutes(15)))
            {
                return StatusCode(429, new AuthResponse { Message = "Too many login attempts. Please try again later." });
            }

            var result = await _authService.LoginAsync(request, ipAddress, userAgent);
            return Ok(result);
        }

        [HttpPost("fido2/options/register")]
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
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
            {
                return Unauthorized();
            }

            var userWithRoles = await _userRoleService.GetUserWithRolesAndPermissionsAsync(userId);
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
        public async Task<ActionResult<AuthResponse>> RefreshToken([FromBody] PasswordlessApi.Api.Models.RequestModel.Security.RefreshTokenRequest request)
        {
            var ipAddress = GetClientIpAddress();
            var userAgent = GetUserAgent();

            var enrichedRequest = new PasswordlessApi.Api.Models.RequestModel.Security.RefreshTokenRequest
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
