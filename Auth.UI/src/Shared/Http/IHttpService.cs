using Auth.UI.src.Common;

namespace Auth.UI.src.Shared.Http
{
    public interface IHttpService
    {
        Task<Response<T>> GetAsync<T>(string url);
        Task<Response<TResponse>> PostAsync<TRequest, TResponse>(string url, TRequest body);
        Task<Response<T>> DeleteAsync<T>(string url);
    }
}
