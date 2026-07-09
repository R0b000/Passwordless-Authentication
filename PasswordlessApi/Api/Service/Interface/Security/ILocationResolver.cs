using System.Net;

namespace PasswordlessApi.Api.Service.Interface.Security
{
    public interface ILocationResolver
    {
        Task<string?> ResolveLocationAsync(string? ipAddress, CancellationToken cancellationToken = default);
    }
}