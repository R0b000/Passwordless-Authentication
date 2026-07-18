namespace Auth.UI.Shared.Utility
{
    public interface ITokenHelper
    {
        Task<object> GetToken();
    }
}
