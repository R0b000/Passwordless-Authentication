namespace Auth.UI.src.Shared.Http
{
    public interface IHttpService
    {
        Task<T?> GetAsync<T>(string url, string? bearerToken = null);
        Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest body, string? bearerToken = null);
    }
}
