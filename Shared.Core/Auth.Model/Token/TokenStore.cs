namespace Auth.Model.Token
{
    public interface ITokenStore
    {
        Task SetToken(string token);
        Task<string?> GetToken();
        Task Clear();
        Task<bool> IsAvailableAsync();
    }

    public class TokenStore : ITokenStore
    {
        private string? _token;

        public Task SetToken(string token)
        {
            _token = token;
            return Task.CompletedTask;
        }

        public Task<string?> GetToken()
        {
            return Task.FromResult(_token);
        }

        public Task Clear()
        {
            _token = null;
            return Task.CompletedTask;
        }

        public Task<bool> IsAvailableAsync()
        {
            return Task.FromResult(true);
        }
    }
}

