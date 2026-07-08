using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Auth.UI.src.Shared.Http
{
    public class HttpService : IHttpService
    {
        private readonly HttpClient _httpClient;

        public HttpService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<T?> GetAsync<T>(string url, string? bearerToken = null)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            ApplyBearer(request, bearerToken);

            using var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                return default;
            }

            return await response.Content.ReadFromJsonAsync<T>();
        }

        public async Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest body, string? bearerToken = null)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = JsonContent.Create(body)
            };
            ApplyBearer(request, bearerToken);

            using var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                return default;
            }

            return await response.Content.ReadFromJsonAsync<TResponse>();
        }

        private static void ApplyBearer(HttpRequestMessage request, string? bearerToken)
        {
            if (!string.IsNullOrWhiteSpace(bearerToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            }
        }
    }
}
