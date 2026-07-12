namespace Auth.UI.src.Shared.Http
{
    public class HttpResult<T>
    {
        public T? Data { get; init; }
        public bool Succeeded { get; init; }
        public string? Error { get; init; }
        public int StatusCode { get; init; }
    }

    public interface IHttpService
    {
        Task<HttpResult<T>> GetAsync<T>(string url, string? bearerToken = null);
        Task<HttpResult<TResponse>> PostAsync<TRequest, TResponse>(string url, TRequest body, string? bearerToken = null);
    }
}
