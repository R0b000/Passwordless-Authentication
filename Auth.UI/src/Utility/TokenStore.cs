namespace Auth.UI.src.Utility
{
    public interface ITokenStore
    {
        event Action? TokenChanged;
        void SetToken(string token);
        string? GetToken();
        void Clear();
    }

    public class TokenStore : ITokenStore
    {
        private string? _token;

        public event Action? TokenChanged;

        public void SetToken(string token)
        {
            _token = token;
            TokenChanged?.Invoke();
        }

        public string? GetToken() => _token;

        public void Clear()
        {
            if (_token is null) return;
            _token = null;
            TokenChanged?.Invoke();
        }
    }
}
