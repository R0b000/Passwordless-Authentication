using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.Shared.Common;
using API.Shared.Service.Interface.Auth;

namespace API.Shared.Controller.Security
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

            var sessions = await _authService.GetActiveSessionsAsync(userId.Value);
            return Ok(sessions.Data?.Sessions);
        }

        [HttpPost("logout-all")]
        public async Task<IActionResult> RevokeAllSessions()
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();

            await _authService.RevokeAllSessionsAsync(userId.Value);
            return Ok(new { succeeded = true, message = "All sessions have been revoked" });
        }

        [HttpDelete("{sessionId:int}")]
        public async Task<IActionResult> RevokeSession(int sessionId)
        {
            var userId = User.GetUserId();
            if (userId == null) return Unauthorized();

            await _authService.RevokeSessionAsync(sessionId, userId.Value);
            return Ok(new { succeeded = true, message = "Session has been revoked" });
        }
    }
}
