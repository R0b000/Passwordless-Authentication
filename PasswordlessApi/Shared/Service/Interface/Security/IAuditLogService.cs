using API.Shared.Models.Entities;
using Shared.Wrapper;

namespace API.Shared.Service.Interface.Security
{
    public interface IAuditLogService
    {
        Task<IResponse> LogAsync(int? userId, string action, string? entityType = null, string? entityId = null,
            string? oldValue = null, string? newValue = null, string? ipAddress = null, string? userAgent = null);
        Task<IResponse<List<AuditLog>>> GetUserAuditLogsAsync(int userId, int limit = 50);
    }
}
