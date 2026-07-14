using Auth.UI.src.Common;
using Auth.UI.src.Utility;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Auth.UI.src.Shared.Http
{
    public class HttpService : IHttpService
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenStore _tokenStore;
        private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

        public HttpService(HttpClient httpClient, ITokenStore tokenStore)
        {
            _httpClient = httpClient;
            _tokenStore = tokenStore;
        }

        public async Task<Response<T>> GetAsync<T>(string url)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            ApplyBearer(request);
            return await SendAndParseAsync<T>(request);
        }

        public async Task<Response<TResponse>> PostAsync<TRequest, TResponse>(string url, TRequest body)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, url);
            ApplyBearer(request);

            if (body is not null)
            {
                request.Content = JsonContent.Create(body, options: _jsonOptions);
            }

            return await SendAndParseAsync<TResponse>(request);
        }

        public async Task<Response<T>> DeleteAsync<T>(string url)
        {
            using var request = new HttpRequestMessage(HttpMethod.Delete, url);
            ApplyBearer(request);
            return await SendAndParseAsync<T>(request);
        }

        private async Task<Response<T>> SendAndParseAsync<T>(HttpRequestMessage request)
        {
            using var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return Response<T>.Failure(ParseErrorMessage(content) ?? $"HTTP {(int)response.StatusCode}");
            }

            if (string.IsNullOrWhiteSpace(content))
            {
                return Response<T>.Failure("Empty server response");
            }

            try
            {
                using var doc = JsonDocument.Parse(content);
                var root = doc.RootElement;

                // The API consistently returns a Response<T> envelope ({ succeeded, message, data })
                // for wrapped results and action results. Raw domain models (and JSON arrays)
                // are returned unwrapped. Detect the envelope by the presence of a "succeeded"
                // or "data" property.
                var isEnvelope = root.ValueKind == JsonValueKind.Object &&
                                 (root.TryGetProperty("succeeded", out _) || root.TryGetProperty("data", out _));

                if (isEnvelope)
                {
                    var wrapped = JsonSerializer.Deserialize<Response<T>>(content, _jsonOptions);
                    if (wrapped is not null)
                    {
                        return wrapped;
                    }
                }

                // Unwrapped payload: the body IS the T instance.
                var raw = JsonSerializer.Deserialize<T>(content, _jsonOptions);
                if (raw is not null)
                {
                    var message = root.ValueKind == JsonValueKind.Object && root.TryGetProperty("message", out var messageEl)
                        ? messageEl.GetString()
                        : null;
                    return Response<T>.Success(raw, message);
                }

                return Response<T>.Failure("Failed to parse server response");
            }
            catch
            {
                return Response<T>.Failure("Failed to parse server response");
            }
        }

        private static string? ParseErrorMessage(string content)
        {
            if (string.IsNullOrWhiteSpace(content)) return null;
            try
            {
                using var doc = JsonDocument.Parse(content);
                if (doc.RootElement.ValueKind == JsonValueKind.Object)
                {
                    foreach (var key in new[] { "message", "title", "detail" })
                    {
                        if (doc.RootElement.TryGetProperty(key, out var el) && el.ValueKind == JsonValueKind.String)
                        {
                            return el.GetString();
                        }
                    }
                }
            }
            catch { }

            return content.Length > 200 ? content[..200] : content;
        }

        private void ApplyBearer(HttpRequestMessage request)
        {
            var token = _tokenStore.GetToken();
            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
    }
}
