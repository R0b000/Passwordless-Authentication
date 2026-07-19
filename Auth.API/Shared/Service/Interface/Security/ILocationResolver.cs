using System.Net;

namespace API.Shared.Service.Interface.Security
{
    public interface ILocationResolver
    {
        Task<string?> ResolveLocationAsync(string? ipAddress, CancellationToken cancellationToken = default);
    }
}