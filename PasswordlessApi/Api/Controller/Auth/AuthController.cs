using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PasswordlessApi.Api.Models.RequestModel.Auth;
using PasswordlessApi.Api.Models.ResponseModel.Auth;
using PasswordlessApi.Api.Service.Interface.Auth;

namespace PasswordlessApi.Api.Controller.Auth
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
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);
            return Ok(result);
        }

        [HttpPost("fido2/options/register")]
        public async Task<ActionResult<Fido2ChallengeResponse>> RequestAttestationOptions([FromBody] Fido2AttestationOptionsRequest request)
        {
            var result = await _authService.RequestAttestationOptionsAsync(request);
            return Ok(result);
        }

        [HttpPost("fido2/register")]
        public async Task<ActionResult<Fido2VerifyResponse>> RegisterCredential([FromBody] Fido2RegisterRequest request)
        {
            var result = await _authService.RegisterCredentialAsync(request);
            return Ok(result);
        }

        [HttpPost("fido2/challenge")]
        public async Task<ActionResult<Fido2ChallengeResponse>> CreateFido2Challenge([FromBody] Fido2ChallengeRequest request)
        {
            var result = await _authService.CreateFido2ChallengeAsync(request);
            return Ok(result);
        }

        [HttpPost("fido2/verify")]
        public async Task<ActionResult<Fido2VerifyResponse>> VerifyFido2Assertion([FromBody] Fido2VerifyRequest request)
        {
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

            var user = await _authService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var userRoleService = HttpContext.RequestServices.GetService<PasswordlessApi.Api.Service.Interface.Rbac.IUserRoleService>();
            var userWithRoles = userRoleService != null
                ? await userRoleService.GetUserWithRolesAndPermissionsAsync(userId)
                : null;

            return Ok(new AuthResponse
            {
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email,
                Message = "Authenticated",
                Role = userWithRoles?.Role,
                Permissions = userWithRoles?.Permissions ?? new List<string>()
            });
        }

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
        public async Task<ActionResult<AuthResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var result = await _authService.RefreshTokenAsync(request);
            return Ok(result);
        }
    }
}
