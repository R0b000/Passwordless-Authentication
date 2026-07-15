using Auth.UI.src.Common;

namespace Auth.UI.src.Shared.Http
{
    public interface IHttpServices
    {
        Task<IResponse<T>> GetAsync<T>(string url);
        Task<IResponse<T>> PostAsJsonAsync<T>(string url, object Data);
        Task<IResponse<T>> DeleteAsync<T>(string url);
    }
}
