using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Auth.Model.Token;

namespace Auth.UI.Utility
{
    public class ProtectedSessionTokenStore : ITokenStore
    {
        private readonly ProtectedSessionStorage _sessionStorage;
        private string? _cachedToken;

        public ProtectedSessionTokenStore(ProtectedSessionStorage sessionStorage)
        {
            _sessionStorage = sessionStorage;
        }

        public async Task SetToken(string token)
        {
            _cachedToken = token;
            try
            {
                await _sessionStorage.SetAsync("authToken", token);
            }
            catch (InvalidOperationException)
            {
            }
        }

        public async Task<string?> GetToken()
        {
            if (_cachedToken != null) return _cachedToken;
            try
            {
                var result = await _sessionStorage.GetAsync<string>("authToken");
                _cachedToken = result.Success ? result.Value : null;
            }
            catch (InvalidOperationException)
            {
            }
            return _cachedToken;
        }

        public async Task Clear()
        {
            _cachedToken = null;
            try
            {
                await _sessionStorage.DeleteAsync("authToken");
            }
            catch (InvalidOperationException)
            {
            }
        }
    }
}
