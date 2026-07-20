using Shared.Core.Token;

namespace Auth.API.Utility.Auth
{
    public class TokenHelper : ITokenHelper
    {
        private readonly ITokenStore _tokenStore;
        public TokenHelper(ITokenStore tokenStore)
        {
            _tokenStore = tokenStore;
        }
        public Task<object> GetToken()
        {
            return Task.FromResult<object>(_tokenStore.GetToken() ?? string.Empty);
        }
    }
}
