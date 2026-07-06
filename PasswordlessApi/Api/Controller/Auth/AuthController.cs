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
        public ActionResult<AuthResponse> Me()
        {
            return Ok(new AuthResponse
            {
                Username = User.Identity?.Name ?? string.Empty,
                Message = "Authenticated"
            });
        }
    }
}
