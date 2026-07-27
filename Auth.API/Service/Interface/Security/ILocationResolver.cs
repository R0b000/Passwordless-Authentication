using System.Net;

namespace Auth.API.Service.Interface.Security
{
    public interface ILocationResolver
    {
        Task<string?> ResolveLocationAsync(string? ipAddress, CancellationToken cancellationToken = default);
    }
}