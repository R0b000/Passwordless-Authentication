using Auth.Model.Token;

namespace Auth.API.Utility.Auth
{
    public class TokenHelper : ITokenHelper
    {
        private readonly ITokenStore _tokenStore;
        public TokenHelper(ITokenStore tokenStore)
        {
            _tokenStore = tokenStore;
        }
        public async Task<object> GetToken()
        {
            return await _tokenStore.GetToken() ?? string.Empty;
        }
    }
}

