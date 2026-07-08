namespace Auth.UI.src.Utility
{
    public interface ITokenStore
    {
        void SetToken(string token);
        string? GetToken();
        void Clear();
    }

    public class TokenStore : ITokenStore
    {
        private string? _token;

        public void SetToken(string token) => _token = token;
        public string? GetToken() => _token;
        public void Clear() => _token = null;
    }
}
