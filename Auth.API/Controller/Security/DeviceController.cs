using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Auth.API.Config;
using Auth.API.Service.Interface.Auth;
using Auth.Model.Models.Common;
using Shared.Data.Wrapper;

namespace Auth.API.Controller.Security
{
    [ApiController]
    [Route("api/auth/devices")]
    [Authorize]
    public class DeviceController : ControllerBase
    {
        private readonly IAuthService _authService;

        public DeviceController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public async Task<IActionResult> GetActiveSessions()
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();

            var result = await _authService.GetActiveSessionsAsync(userId.Value);
            return Ok(result);
        }

        [HttpPost("logout-all")]
        public async Task<IActionResult> RevokeAllSessions()
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();

            var result = await _authService.RevokeAllSessionsAsync(userId.Value);
            return Ok(result);
        }

        [HttpDelete("{sessionId:int}")]
        public async Task<IActionResult> RevokeSession(int sessionId)
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();

            var result = await _authService.RevokeSessionAsync(sessionId, userId.Value);
            return Ok(result);
        }
    }
}