namespace Shared.Core.Token
{
    public interface ITokenHelper
    {
        Task<object> GetToken();
    }
}
