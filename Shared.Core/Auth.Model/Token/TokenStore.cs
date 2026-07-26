namespace Auth.Model.Token
{
    public interface ITokenStore
    {
        Task SetToken(string token);
        Task<string?> GetToken();
        Task Clear();
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
    }
}

