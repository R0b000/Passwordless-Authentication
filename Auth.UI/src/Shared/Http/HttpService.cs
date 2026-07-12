using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Auth.UI.src.Shared.Http
{
    public class HttpService : IHttpService
    {
        private readonly HttpClient _httpClient;

        public HttpService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResult<T>> GetAsync<T>(string url, string? bearerToken = null)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            ApplyBearer(request, bearerToken);

            using var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                return new HttpResult<T> { Succeeded = false, StatusCode = (int)response.StatusCode, Error = await ReadErrorAsync(response) };
            }

            var data = await response.Content.ReadFromJsonAsync<T>();
            return new HttpResult<T> { Succeeded = true, StatusCode = (int)response.StatusCode, Data = data };
        }

        public async Task<HttpResult<TResponse>> PostAsync<TRequest, TResponse>(string url, TRequest body, string? bearerToken = null)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, url);
            ApplyBearer(request, bearerToken);

            if (body is not null)
            {
                request.Content = JsonContent.Create(body);
            }

            using var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                return new HttpResult<TResponse> { Succeeded = false, StatusCode = (int)response.StatusCode, Error = await ReadErrorAsync(response) };
            }

            var data = await response.Content.ReadFromJsonAsync<TResponse>();
            return new HttpResult<TResponse> { Succeeded = true, StatusCode = (int)response.StatusCode, Data = data };
        }

        private static async Task<string> ReadErrorAsync(HttpResponseMessage response)
        {
            var body = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(body))
            {
                return $"Request failed with status {(int)response.StatusCode}.";
            }

            try
            {
                using var doc = JsonDocument.Parse(body);
                var root = doc.RootElement;

                if (root.TryGetProperty("errors", out var errorsEl) && errorsEl.ValueKind == JsonValueKind.Object)
                {
                    var messages = new List<string>();
                    foreach (var prop in errorsEl.EnumerateObject())
                    {
                        if (prop.Value.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var item in prop.Value.EnumerateArray())
                            {
                                if (item.ValueKind == JsonValueKind.String)
                                {
                                    messages.Add(item.GetString()!);
                                }
                            }
                        }
                    }

                    if (messages.Count > 0)
                    {
                        return string.Join(" ", messages);
                    }
                }

                foreach (var key in new[] { "message", "Message", "title", "Title", "detail", "Detail" })
                {
                    if (root.TryGetProperty(key, out var el) && el.ValueKind == JsonValueKind.String)
                    {
                        return el.GetString()!;
                    }
                }
            }
            catch
            {
                // Fall through to returning the raw body.
            }

            return body.Length > 300 ? body[..300] : body;
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
