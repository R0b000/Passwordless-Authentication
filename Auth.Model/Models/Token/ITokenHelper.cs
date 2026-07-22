namespace Auth.Model.Token
{
    public interface ITokenHelper
    {
        Task<object> GetToken();
    }
}

