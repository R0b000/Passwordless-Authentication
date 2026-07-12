using Auth.UI.src.Common;
using Auth.UI.src.Shared.Http;

namespace Auth.UI.src.Shared.Http
{
    public class GenericHttpRepository<T> where T : class
    {
        private readonly IHttpService _httpService;

        public GenericHttpRepository(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<Response<T>> QuerySingleAsync(string endpoint, object request, string? bearerToken = null)
        {
            var result = await _httpService.PostAsync<object, T>(endpoint, request, bearerToken);
            if (!result.Succeeded)
            {
                return Response<T>.Failure(result.Error ?? "Request failed or returned no data");
            }

            if (result.Data is null)
            {
                return Response<T>.Failure(result.Error ?? "Request failed or returned no data");
            }

            return Response<T>.Success(result.Data);
        }

        public async Task<Response<T>> GetSingleAsync(string endpoint, string? bearerToken = null)
        {
            var result = await _httpService.GetAsync<T>(endpoint, bearerToken);
            if (!result.Succeeded)
            {
                return Response<T>.Failure(result.Error ?? "Request failed or returned no data");
            }

            if (result.Data is null)
            {
                return Response<T>.Failure(result.Error ?? "Request failed or returned no data");
            }

            return Response<T>.Success(result.Data);
        }
    }
}
