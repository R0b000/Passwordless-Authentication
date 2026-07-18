using API.Shared.Common;
using Shared.Core.Models.Entities;
using Shared.Data.Repository.Interface;
using API.Shared.Service.Interface.Security;
using Shared.Core.Wrapper;

namespace API.Shared.Service.Implementation.Security
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IGenericRepository<AuditLog> _authRepository;

        public AuditLogService(IGenericRepository<AuditLog> authRepository)
        {
            _authRepository = authRepository;
        }

        public async Task<IResponse> LogAsync(int? userId, string action, string? entityType = null, string? entityId = null,
            string? oldValue = null, string? newValue = null, string? ipAddress = null, string? userAgent = null)
        {
            await _authRepository.ExecuteAsync(
                DbConstants.Procedures.Users,
                new
                {
                    AuthType = DbConstants.AuthTypes.AuditLog,
                    FIDOOperation = "Create",
                    UserId = userId,
                    Action = action,
                    EntityType = entityType,
                    EntityId = entityId,
                    OldValue = oldValue,
                    NewValue = newValue,
                    IpAddress = ipAddress,
                    UserAgent = userAgent
                });

            return Response.Success();
        }

        public async Task<IResponse<List<AuditLog>>> GetUserAuditLogsAsync(int userId, int limit = 50)
        {
            var result = await _authRepository.QueryAsync<AuditLog>(
                DbConstants.Procedures.Users,
                new
                {
                    AuthType = DbConstants.AuthTypes.AuditLog,
                    FIDOOperation = "GetByUser",
                    UserId = userId
                });

            var logs = result?.Succeeded == true && result.Data != null
                ? result.Data.Take(limit).ToList()
                : new List<AuditLog>();

            return Response<List<AuditLog>>.Success(logs);
        }
    }
}
