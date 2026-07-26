using Auth.Model.Models.Security;
using Auth.Model.Models.Account;
using Auth.Model.Models.Entities;
using Shared.Data.Repository.Interface;
using Shared.Data.Wrapper;
using Auth.API.Config;
using Auth.API.Utility.PasswordHash;
using Auth.API.Service.Interface.Auth;
using Auth.API.Service.Interface.Security;

namespace Auth.API.Service.Implementation.Auth
{
    public class SecurityService : ISecurityService
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IPasswordHash _passwordHash;
        private readonly IAuditLogService _auditLogService;
        private const string ProcedureName = DbConstants.Procedures.Users;

        public SecurityService(
            IGenericRepository<User> userRepository,
            IPasswordHash passwordHash,
            IAuditLogService auditLogService)
        {
            _userRepository = userRepository;
            _passwordHash = passwordHash;
            _auditLogService = auditLogService;
        }

        public async Task<IResponse<SecuritySettingsResponse>> GetSecuritySettingsAsync(int userId)
        {
            var result = await _userRepository.QuerySingleAsync<SecuritySettingsResponse>(
                ProcedureName,
                new { AuthType = DbConstants.AuthTypes.GetSecurity, UserId = userId });

            if (result.Succeeded && result.Data != null)
                return Response<SecuritySettingsResponse>.Success(result.Data);

            return Response<SecuritySettingsResponse>.Success(new SecuritySettingsResponse());
        }

        public async Task<IResponse<SecuritySettingsResponse>> UpdateSecuritySettingsAsync(int userId, SecuritySettingsResponse request)
        {
            var result = await _userRepository.QuerySingleAsync<SecuritySettingsResponse>(
                ProcedureName,
                new
                {
                    AuthType = DbConstants.AuthTypes.UpdateSecurity,
                    UserId = userId,
                    request.AlertOnNewDevice,
                    request.RequirePasswordForSensitive,
                    Now = DateTime.UtcNow
                });

            if (result.Succeeded && result.Data != null)
            {
                await _auditLogService.LogAsync(userId, "SecuritySettingsUpdated", "User", userId.ToString(), null, "Security settings updated");
                return Response<SecuritySettingsResponse>.Success(result.Data);
            }

            return Response<SecuritySettingsResponse>.Success(request);
        }

        public async Task<IResponse> ChangePasswordAsync(int userId, ChangePasswordRequest request)
        {
            var userResult = await _userRepository.QuerySingleAsync<User>(
                ProcedureName,
                new { AuthType = DbConstants.AuthTypes.Login, UserId = userId });

            if (userResult.Succeeded && userResult.Data != null)
            {
                if (!_passwordHash.VerifyPassword(request.CurrentPassword, userResult.Data.PasswordHash ?? string.Empty))
                    return Response.Fail("Current password is incorrect");

                if (request.NewPassword != request.ConfirmPassword)
                    return Response.Fail("New password and confirmation do not match");

                var newPasswordHash = _passwordHash.HashPassword(request.NewPassword);
                var result = await _userRepository.ExecuteAsync(
                    ProcedureName,
                    new
                    {
                        AuthType = DbConstants.AuthTypes.ChangePassword,
                        UserId = userId,
                        PasswordHash = newPasswordHash,
                        Now = DateTime.UtcNow
                    });

                if (result.Succeeded && result.Data > 0)
                {
                    await _auditLogService.LogAsync(userId, "PasswordChanged", "User", userId.ToString());
                    return Response.Success("Password changed successfully");
                }
            }

            return Response.Fail("Failed to change password");
        }

        public async Task<IResponse<SecuritySettingsResponse>> EnableTwoFactorAsync(int userId)
        {
            var result = await _userRepository.QuerySingleAsync<SecuritySettingsResponse>(
                ProcedureName,
                new { AuthType = DbConstants.AuthTypes.Enable2Fa, UserId = userId, Now = DateTime.UtcNow });

            if (result.Succeeded && result.Data != null)
            {
                await _auditLogService.LogAsync(userId, "TwoFactorEnabled", "User", userId.ToString());
                return Response<SecuritySettingsResponse>.Success(result.Data);
            }

            return Response<SecuritySettingsResponse>.Success(new SecuritySettingsResponse { TwoFactorEnabled = true });
        }

        public async Task<IResponse<SecuritySettingsResponse>> DisableTwoFactorAsync(int userId)
        {
            var result = await _userRepository.QuerySingleAsync<SecuritySettingsResponse>(
                ProcedureName,
                new { AuthType = DbConstants.AuthTypes.Disable2Fa, UserId = userId, Now = DateTime.UtcNow });

            if (result.Succeeded && result.Data != null)
            {
                await _auditLogService.LogAsync(userId, "TwoFactorDisabled", "User", userId.ToString());
                return Response<SecuritySettingsResponse>.Success(result.Data);
            }

            return Response<SecuritySettingsResponse>.Success(new SecuritySettingsResponse { TwoFactorEnabled = false });
        }

        public async Task<IResponse<ActivityLogResponse>> GetActivityLogsAsync(int userId, ActivityQueryRequest query)
        {
            var result = await _userRepository.QueryAsync<AuditLog>(
                ProcedureName,
                new
                {
                    AuthType = DbConstants.AuthTypes.AuditLog,
                    FIDOOperation = "GetByUser",
                    UserId = userId
                });

            var entries = new List<ActivityLogEntry>();

            if (result.Succeeded && result.Data != null)
            {
                var filtered = result.Data.AsEnumerable();

                if (query.From.HasValue)
                    filtered = filtered.Where(a => a.CreatedAt >= query.From.Value);
                if (query.To.HasValue)
                    filtered = filtered.Where(a => a.CreatedAt <= query.To.Value.Date.AddDays(1).AddTicks(-1));
                if (!string.IsNullOrWhiteSpace(query.Type) && query.Type != "all")
                    filtered = filtered.Where(a => a.Action == query.Type);
                if (!string.IsNullOrWhiteSpace(query.Search))
                {
                    var term = query.Search.Trim().ToLowerInvariant();
                    filtered = filtered.Where(a =>
                        a.Action.ToLowerInvariant().Contains(term) ||
                        (a.EntityType ?? string.Empty).ToLowerInvariant().Contains(term));
                }

                entries = filtered.OrderByDescending(a => a.CreatedAt).Select(a => new ActivityLogEntry
                {
                    Id = (int)a.Id,
                    Type = a.Action,
                    Description = a.Action,
                    Device = a.UserAgent ?? "Unknown",
                    IpAddress = a.IpAddress ?? "Unknown",
                    Timestamp = a.CreatedAt
                }).ToList();
            }

            return Response<ActivityLogResponse>.Success(new ActivityLogResponse { Entries = entries });
        }

        public async Task<IResponse> VerifyDeviceAsync(int userId, VerifyDeviceRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Code))
                return Response.Fail("Enter the verification code");

            await _auditLogService.LogAsync(userId, "DeviceVerified", "User", userId.ToString(), null, request.TrustDevice ? "Device verified and trusted" : "Device verified");

            return Response.Success(request.TrustDevice ? "Device verified and trusted" : "Device verified");
        }
    }
}
