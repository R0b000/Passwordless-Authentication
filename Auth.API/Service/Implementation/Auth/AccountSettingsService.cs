using Auth.Model.Models.Account;
using Auth.Model.Models.Entities;
using Shared.Data.Repository.Interface;
using Shared.Data.Wrapper;
using Auth.API.Config;
using Auth.API.Service.Interface.Auth;
using Auth.API.Service.Interface.Security;

namespace Auth.API.Service.Implementation.Auth
{
    public class AccountSettingsService : IAccountSettingsService
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IAuditLogService _auditLogService;
        private const string ProcedureName = DbConstants.Procedures.Users;

        public AccountSettingsService(IGenericRepository<User> userRepository, IAuditLogService auditLogService)
        {
            _userRepository = userRepository;
            _auditLogService = auditLogService;
        }

        public async Task<IResponse<AccountSettingsResponse>> GetAccountSettingsAsync(int userId)
        {
            var result = await _userRepository.QuerySingleAsync<AccountSettingsResponse>(
                ProcedureName,
                new { AuthType = DbConstants.AuthTypes.GetSettings, UserId = userId });

            if (result.Succeeded && result.Data != null)
                return Response<AccountSettingsResponse>.Success(result.Data);

            return Response<AccountSettingsResponse>.Success(new AccountSettingsResponse());
        }

        public async Task<IResponse<AccountSettingsResponse>> UpdateAccountSettingsAsync(int userId, UpdateSettingsRequest request)
        {
            var result = await _userRepository.QuerySingleAsync<AccountSettingsResponse>(
                ProcedureName,
                new
                {
                    AuthType = DbConstants.AuthTypes.UpdateSettings,
                    UserId = userId,
                    request.Username,
                    request.Email,
                    request.EmailPreferences,
                    request.Timezone,
                    request.Language,
                    request.EmailNotifications,
                    request.PushNotifications,
                    request.SmsAlerts,
                    request.MarketingEmails,
                    Now = DateTime.UtcNow
                });

            if (result.Succeeded && result.Data != null)
            {
                await _auditLogService.LogAsync(userId, "SettingsUpdated", "User", userId.ToString(), null, "Settings updated");
                return Response<AccountSettingsResponse>.Success(result.Data);
            }

            return Response<AccountSettingsResponse>.Success(new AccountSettingsResponse
            {
                DisplayName = request.Username,
                Username = request.Username,
                Email = request.Email,
                EmailPreferences = request.EmailPreferences,
                Timezone = request.Timezone,
                Language = request.Language,
                EmailNotifications = request.EmailNotifications,
                PushNotifications = request.PushNotifications,
                SmsAlerts = request.SmsAlerts,
                MarketingEmails = request.MarketingEmails
            });
        }
    }
}
