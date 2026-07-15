using API.Shared.Models.Entities;

namespace API.Shared.Service.Interface.Security
{
    public interface IAuditLogService
    {
        Task LogAsync(int? userId, string action, string? entityType = null, string? entityId = null, 
            string? oldValue = null, string? newValue = null, string? ipAddress = null, string? userAgent = null);
        Task<List<AuditLog>> GetUserAuditLogsAsync(int userId, int limit = 50);
    }
}