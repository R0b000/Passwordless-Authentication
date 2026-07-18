using Shared.Wrapper;
using Auth.UI.Shared.Model.Toast;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using UI.Shared.Manager.Interface.Http;
using Auth.UI.Shared.Utility;
using System.Net.Http.Json;

namespace UI.Shared.Manager.Implementation.Http
{
    public class HttpServices : IHttpServices
    {
        private readonly HttpClient _httpClient = new();
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ToastService _toastService;
        private readonly ITokenHelper _tokenHelper;
        private readonly NavigationManager _navigationManager;
        public HttpServices(IHttpClientFactory httpClientFactory, ToastService toastService, ITokenHelper tokenHelper, NavigationManager navigationManager)
        {
            _httpClientFactory = httpClientFactory;
            _httpClient = _httpClientFactory.CreateClient("ApiGateway");
            _httpClient.Timeout= TimeSpan.FromMinutes(20);
            _toastService = toastService;
            _tokenHelper = tokenHelper;
            _navigationManager = navigationManager;
        }

        public async Task<IResponse<T>> GetAsync<T>(string url)
        {
            try
            {
                await SetHeader();
                var response = await _httpClient.GetAsync(url);
                var responseAsString = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    if (((int)response.StatusCode) == 401)
                    {
                        _navigationManager.NavigateTo("/login", true);
                    }
                    else
                    {
                        HandleErrorResponse(response);
                    }
                }
                var responseObject = JsonSerializer.Deserialize<Response<T>>(responseAsString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    ReferenceHandler = ReferenceHandler.Preserve
                });
                return responseObject!;
            }
            catch (Exception ex)
            {
                _toastService.Notify(new(ToastType.Warning, ex.Message));
                throw new ApplicationException(ex.Message);
            }
        }
        public async Task<IResponse<T>> PostAsJsonAsync<T>(string url, object Data)
        {
            try
            {
                await SetHeader();
                var response = await _httpClient.PostAsJsonAsync(url, Data);
                var responseAsString = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    if (((int)response.StatusCode) == 401)
                    {
                        _navigationManager.NavigateTo("/login", true);
                    }
                    else
                    {
                        HandleErrorResponse(response);
                    }
                }
                var responseObject = JsonSerializer.Deserialize<Response<T>>(responseAsString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    ReferenceHandler = ReferenceHandler.Preserve
                });
                return responseObject!;
            }
            catch (Exception ex)
            {
                _toastService.Notify(new(ToastType.Warning, ex.Message));
                throw new ApplicationException(ex.Message);
            }
        }
        public async Task<IResponse<T>> DeleteAsync<T>(string url)
        {
            try
            {
                await SetHeader();
                var response = await _httpClient.DeleteAsync(url);
                var responseAsString = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    if (((int)response.StatusCode) == 401)
                    {
                        _navigationManager.NavigateTo("/login", true);
                    }
                    else
                    {
                        HandleErrorResponse(response);
                    }
                }
                var responseObject = JsonSerializer.Deserialize<Response<T>>(responseAsString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    ReferenceHandler = ReferenceHandler.Preserve
                });
                return responseObject!;
            }
            catch (Exception ex)
            {
                _toastService.Notify(new(ToastType.Warning, ex.Message));
                throw new ApplicationException(ex.Message);
            }
        }

        private void HandleErrorResponse(HttpResponseMessage response)
        {
            string message = response.StatusCode switch
            {
                System.Net.HttpStatusCode.BadRequest => "Bad request. Please check your input and try again.",
                System.Net.HttpStatusCode.Unauthorized => "Unauthorized. Please check your credentials.",
                System.Net.HttpStatusCode.Forbidden => "Forbidden. You do not have permission to access this resource.",
                System.Net.HttpStatusCode.NotFound => "Resource not found. Please check the URL and try again.",
                System.Net.HttpStatusCode.InternalServerError => "Internal server error. Please try again later.",
                _ => "An error occurred. Please try again."
            };
            throw new ApplicationException(message);
        }
        public async Task SetHeader()
        {
            var token = (await _tokenHelper.GetToken()).ToString();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
