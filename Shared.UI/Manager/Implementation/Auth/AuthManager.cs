using Shared.Core.Wrapper;
using Shared.Core.UIModels.Auth;
using Shared.Core.Token;
using Shared.UI.Manager.Interface.Auth;
using Shared.UI.Http;
using Shared.UI.Manager.Routes;

namespace Shared.UI.Manager.Implementation.Auth
{
    public class AuthManager : IAuthManager
    {
        private readonly IHttpServices _httpService;
        private readonly ITokenStore _tokenStore;

        public AuthManager(IHttpServices httpService, ITokenStore tokenStore)
        {
            _httpService = httpService;
            _tokenStore = tokenStore;
        }

        public async Task<IResponse<AuthResponse>> RegisterAsync(RegisterRequest request)
        {
            try
            {
                var result = await _httpService.PostAsJsonAsync<AuthResponse>(AuthRoute.Register, request);
                if (result.Succeeded && result.Data?.Token is not null)
                {
                    _tokenStore.SetToken(result.Data.Token);
                    return Response<AuthResponse>.Success(result.Data, "Registration successful");
                }
                else
                {
                    return Response<AuthResponse>.Fail(result.Messages ?? "Registration failed");
                }
            }
            catch (Exception ex)
            {
                return Response<AuthResponse>.Fail($"An error occurred during registration: {ex.Message}");
            }
        }

        public async Task<IResponse<AuthResponse>> LoginAsync(LoginRequest request)
        {
            try
            {
                var result = await _httpService.PostAsJsonAsync<AuthResponse>(AuthRoute.Login, request);
                if (result.Succeeded && result.Data?.Token is not null)
                {
                    _tokenStore.SetToken(result.Data.Token);
                    return Response<AuthResponse>.Success(result.Data, "Login successful");
                }
                else if (result.Succeeded && result.Data?.RequiresFido2 == true)
                {
                    return Response<AuthResponse>.Success(result.Data, "Two-factor authentication required");
                }
                else
                {
                    return Response<AuthResponse>.Fail(result.Messages ?? "Login failed");
                }
            }
            catch (Exception ex)
            {
                return Response<AuthResponse>.Fail($"An error occurred during login: {ex.Message}");
            }
        }

        public async Task<IResponse<AuthResponse>> GetCurrentUserAsync()
        {
            try
            {
                var result = await _httpService.GetAsync<AuthResponse>(AuthRoute.Me);
                if (result.Succeeded && result.Data is not null)
                {
                    return Response<AuthResponse>.Success(result.Data, "Current user retrieved");
                }
                else
                {
                    return Response<AuthResponse>.Fail(result.Messages ?? "Failed to retrieve current user");
                }
            }
            catch (Exception ex)
            {
                return Response<AuthResponse>.Fail($"An error occurred while retrieving the current user: {ex.Message}");
            }
        }

        public async Task<IResponse<AuthResponse>> GetUserByEmailAsync(string email)
        {
            try
            {
                var result = await _httpService.GetAsync<AuthResponse>($"{AuthRoute.Lookup}?email={Uri.EscapeDataString(email)}");
                if (result.Succeeded && result.Data is not null)
                {
                    return Response<AuthResponse>.Success(result.Data, "User resolved");
                }
                else
                {
                    return Response<AuthResponse>.Fail(result.Messages ?? "User not found");
                }
            }
            catch (Exception ex)
            {
                return Response<AuthResponse>.Fail($"An error occurred while looking up the user: {ex.Message}");
            }
        }

        public async Task<IResponse<Fido2ChallengeResponse>> RequestAttestationOptionsAsync(Fido2AttestationOptionsRequest request)
        {
            try
            {
                var result = await _httpService.PostAsJsonAsync<Fido2ChallengeResponse>(AuthRoute.Fido2Options, request);
                if (result.Succeeded && result.Data is not null)
                {
                    return Response<Fido2ChallengeResponse>.Success(result.Data, "Attestation options generated");
                }
                else
                {
                    return Response<Fido2ChallengeResponse>.Fail(result.Messages ?? "Failed to request attestation options");
                }
            }
            catch (Exception ex)
            {
                return Response<Fido2ChallengeResponse>.Fail($"An error occurred while requesting attestation options: {ex.Message}");
            }
        }

        public async Task<IResponse<Fido2VerifyResponse>> RegisterCredentialAsync(Fido2RegisterRequest request)
        {
            try
            {
                var result = await _httpService.PostAsJsonAsync<Fido2VerifyResponse>(AuthRoute.Fido2Register, request);
                if (result.Succeeded && result.Data is not null)
                {
                    return Response<Fido2VerifyResponse>.Success(result.Data, "Credential registered");
                }
                else
                {
                    return Response<Fido2VerifyResponse>.Fail(result.Messages ?? "Failed to register credential");
                }
            }
            catch (Exception ex)
            {
                return Response<Fido2VerifyResponse>.Fail($"An error occurred while registering the credential: {ex.Message}");
            }
        }

        public async Task<IResponse<Fido2ChallengeResponse>> CreateFido2ChallengeAsync(int userId, string origin)
        {
            try
            {
                var result = await _httpService.PostAsJsonAsync<Fido2ChallengeResponse>(
                    AuthRoute.Fido2Challenge, new Fido2ChallengeRequest { UserId = userId, Origin = origin });
                if (result.Succeeded && result.Data is not null)
                {
                    return Response<Fido2ChallengeResponse>.Success(result.Data, "Challenge created");
                }
                else
                {
                    return Response<Fido2ChallengeResponse>.Fail(result.Messages ?? "Failed to create challenge");
                }
            }
            catch (Exception ex)
            {
                return Response<Fido2ChallengeResponse>.Fail($"An error occurred while creating the challenge: {ex.Message}");
            }
        }

        public async Task<IResponse<Fido2VerifyResponse>> VerifyFido2AssertionAsync(Fido2VerifyRequest request)
        {
            try
            {
                var result = await _httpService.PostAsJsonAsync<Fido2VerifyResponse>(AuthRoute.Fido2Verify, request);
                if (result.Succeeded && result.Data?.Token is not null)
                {
                    _tokenStore.SetToken(result.Data.Token);
                    return Response<Fido2VerifyResponse>.Success(result.Data, "Assertion verified");
                }
                else if (result.Succeeded && result.Data is not null)
                {
                    return Response<Fido2VerifyResponse>.Success(result.Data, "Assertion verified");
                }
                else
                {
                    return Response<Fido2VerifyResponse>.Fail(result.Messages ?? "Failed to verify assertion");
                }
            }
            catch (Exception ex)
            {
                return Response<Fido2VerifyResponse>.Fail($"An error occurred while verifying the assertion: {ex.Message}");
            }
        }

        public async Task<IResponse<OtpResponse>> RequestOtpAsync(OtpRequest request)
        {
            try
            {
                var result = await _httpService.PostAsJsonAsync<OtpResponse>(AuthRoute.OtpRequest, request);
                if (result.Succeeded && result.Data is not null)
                {
                    return Response<OtpResponse>.Success(result.Data, "OTP requested");
                }
                else
                {
                    return Response<OtpResponse>.Fail(result.Messages ?? "Failed to request OTP");
                }
            }
            catch (Exception ex)
            {
                return Response<OtpResponse>.Fail($"An error occurred while requesting the OTP: {ex.Message}");
            }
        }

        public async Task<IResponse<AuthResponse>> VerifyOtpAsync(OtpVerifyRequest request)
        {
            try
            {
                var result = await _httpService.PostAsJsonAsync<AuthResponse>(AuthRoute.OtpVerify, request);
                if (result.Succeeded && result.Data?.Token is not null)
                {
                    _tokenStore.SetToken(result.Data.Token);
                    return Response<AuthResponse>.Success(result.Data, "OTP verified");
                }
                else if (result.Succeeded && result.Data is not null)
                {
                    return Response<AuthResponse>.Success(result.Data, "OTP verified");
                }
                else
                {
                    return Response<AuthResponse>.Fail(result.Messages ?? "Failed to verify OTP");
                }
            }
            catch (Exception ex)
            {
                return Response<AuthResponse>.Fail($"An error occurred while verifying the OTP: {ex.Message}");
            }
        }
    }
}
