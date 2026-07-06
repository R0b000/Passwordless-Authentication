using PasswordlessApi.Api.Models.Entities;
using PasswordlessApi.Api.Models.RequestModel.Auth;
using PasswordlessApi.Api.Models.ResponseModel.Auth;
using PasswordlessApi.Api.Service.Interface.Repository;
using PasswordlessApi.Api.Service.Interface.Auth;
using PasswordlessApi.Api.Utility.Jwt;
using Fido2NetLib;
using Fido2NetLib.Objects;
using Microsoft.Extensions.Configuration;

namespace PasswordlessApi.Api.Service.Implementation.Auth
{
    public class Fido2Service : IFido2Service
    {
        private readonly IDapperRepository _dapperRepository;
        private readonly IJwtHelper _jwtHelper;
        private readonly Fido2 _fido2;
        private readonly Fido2Configuration _fido2Config;
        private readonly string _serverDomain;

        public Fido2Service(IDapperRepository dapperRepository, IJwtHelper jwtHelper, IConfiguration configuration)
        {
            _dapperRepository = dapperRepository;
            _jwtHelper = jwtHelper;

            _serverDomain = configuration["Fido2Settings:ServerDomain"] ?? "localhost";
            var serverName = configuration["Fido2Settings:ServerName"] ?? "PasswordlessApi";

            _fido2Config = new Fido2Configuration
            {
                ServerDomain = _serverDomain,
                ServerName = serverName,
                TimestampDriftTolerance = 300,
                ChallengeSize = 32
            };

            _fido2 = new Fido2(_fido2Config);
        }

        public async Task<Fido2ChallengeResponse> CreateChallengeAsync(int userId)
        {
            var credentials = await _dapperRepository.QueryAsync<UserCredential>(
                "sp_Users",
                new { AuthType = "FIDO", FIDOOperation = "GetCredentialsByUserId", UserId = userId });

            var credentialList = credentials.ToList();
            if (!credentialList.Any())
            {
                throw new InvalidOperationException("No FIDO2 credentials found for user");
            }

            var allowedCredentials = credentialList.Select(c =>
            {
                var transports = string.IsNullOrEmpty(c.Transports)
                    ? null
                    : System.Text.Json.JsonSerializer.Deserialize<AuthenticatorTransport[]>(c.Transports);
                return new PublicKeyCredentialDescriptor(
                    PublicKeyCredentialType.PublicKey,
                    Base64UrlDecode(c.CredentialId),
                    transports
                );
            }).ToList();

            var options = _fido2.GetAssertionOptions(
                allowedCredentials: allowedCredentials,
                userVerification: UserVerificationRequirement.Required,
                extensions: new AuthenticationExtensionsClientInputs()
            );

            var challengeId = Guid.NewGuid();
            var now = DateTime.UtcNow;
            var expiresAt = now.AddMinutes(10);

            await _dapperRepository.ExecuteAsync("sp_Users", new
            {
                AuthType = "FIDO",
                FIDOOperation = "CreateChallenge",
                UserId = userId,
                Challenge = Convert.ToBase64String(options.Challenge),
                ExpiresAt = expiresAt
            });

            return new Fido2ChallengeResponse
            {
                Id = challengeId.ToString(),
                Challenge = Convert.ToBase64String(options.Challenge),
                PublicKeyCredentialCreationOptions = options.ToJson()
            };
        }

        public async Task<Fido2VerifyResponse> VerifyAssertionAsync(Fido2VerifyRequest request)
        {
            if (request.UserId <= 0)
            {
                throw new ArgumentException("UserId is required for assertion");
            }

            var credential = await _dapperRepository.QueryFirstAsync<UserCredential>(
                "sp_Users",
                new { AuthType = "FIDO", FIDOOperation = "GetCredential", CredentialId = request.CredentialId });

            if (credential == null)
            {
                return new Fido2VerifyResponse
                {
                    Success = false,
                    Message = "Credential not found"
                };
            }

            var credentialIdBytes = Base64UrlDecode(request.CredentialId);
            var clientDataBytes = Base64UrlDecode(request.ClientDataJson);
            var authenticatorDataBytes = Base64UrlDecode(request.AuthenticatorData);
            var signatureBytes = Base64UrlDecode(request.Signature);
            var userHandleBytes = BitConverter.GetBytes(request.UserId);

            var storedPublicKey = Base64UrlDecode(credential.PublicKey);
            var storedCount = (uint)credential.SignCount;

            var userCredentials = await _dapperRepository.QueryAsync<UserCredential>(
                "sp_Users",
                new { AuthType = "FIDO", FIDOOperation = "GetCredentialsByUserId", UserId = credential.UserId });

            var allowedCredentials = userCredentials.Select(c =>
            {
                var transports = string.IsNullOrEmpty(c.Transports)
                    ? null
                    : System.Text.Json.JsonSerializer.Deserialize<AuthenticatorTransport[]>(c.Transports);
                return new PublicKeyCredentialDescriptor(
                    PublicKeyCredentialType.PublicKey,
                    Base64UrlDecode(c.CredentialId),
                    transports
                );
            }).ToList();

            var originalChallenge = Convert.FromBase64String(credential.CredentialId);
            var originalOptions = AssertionOptions.Create(
                _fido2Config,
                originalChallenge,
                allowedCredentials,
                UserVerificationRequirement.Required,
                new AuthenticationExtensionsClientInputs()
            );

            var assertionResponse = new AuthenticatorAssertionRawResponse.AssertionResponse
            {
                ClientDataJson = clientDataBytes,
                AuthenticatorData = authenticatorDataBytes,
                Signature = signatureBytes,
                UserHandle = userHandleBytes
            };

            var rawAssertion = new AuthenticatorAssertionRawResponse
            {
                Id = request.CredentialId,
                RawId = credentialIdBytes,
                Response = assertionResponse,
                Type = PublicKeyCredentialType.PublicKey
            };

            var makeAssertionParams = new MakeAssertionParams
            {
                AssertionResponse = rawAssertion,
                OriginalOptions = originalOptions,
                StoredPublicKey = storedPublicKey,
                StoredSignatureCounter = storedCount,
                IsUserHandleOwnerOfCredentialIdCallback = async (IsUserHandleOwnerOfCredentialIdParams p, CancellationToken ct) => true
            };

            try
            {
                var result = await _fido2.MakeAssertionAsync(makeAssertionParams);

                var newCounter = (long)result.SignCount;

                if (request.Counter.HasValue && result.SignCount <= request.Counter.Value)
                {
                    return new Fido2VerifyResponse
                    {
                        Success = false,
                        Message = "Counter regression detected"
                    };
                }

                await _dapperRepository.ExecuteAsync("sp_Users", new
                {
                    AuthType = "FIDO",
                    FIDOOperation = "UpsertCredential",
                    UserId = credential.UserId,
                    CredentialId = credential.CredentialId,
                    PublicKey = credential.PublicKey,
                    SignCount = newCounter,
                    Transports = credential.Transports
                });

                var user = await _dapperRepository.QueryFirstAsync<User>(
                    "sp_Users",
                    new { AuthType = "Login", UserId = credential.UserId });

                var username = user?.Username ?? credential.UserId.ToString();
                var token = _jwtHelper.GenerateToken(credential.UserId, username);

                return new Fido2VerifyResponse
                {
                    Success = true,
                    Token = token,
                    Message = "FIDO2 verification successful"
                };
            }
            catch (Fido2VerificationException ex)
            {
                return new Fido2VerifyResponse
                {
                    Success = false,
                    Message = $"FIDO2 verification error: {ex.Message}"
                };
            }
        }

        private static byte[] Base64UrlDecode(string input)
        {
            string base64 = input.Replace('-', '+').Replace('_', '/');
            switch (base64.Length % 4)
            {
                case 0: break;
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
                default: throw new FormatException("Invalid base64url string");
            }
            return Convert.FromBase64String(base64);
        }
    }
}
