using PasswordlessApi.Api.Models.Entities;
using PasswordlessApi.Api.Models.RequestModel.Auth;
using PasswordlessApi.Api.Models.ResponseModel.Auth;
using PasswordlessApi.Api.Service.Interface.Repository;
using PasswordlessApi.Api.Service.Interface.Auth;
using PasswordlessApi.Api.Utility.Jwt;
using PasswordlessApi.Api.Configuration;
using Fido2NetLib;
using Fido2NetLib.Objects;
using Microsoft.Extensions.Options;

namespace PasswordlessApi.Api.Service.Implementation.Auth
{
    public class Fido2Service : IFido2Service
    {
        private readonly IDapperRepository _dapperRepository;
        private readonly IJwtHelper _jwtHelper;
        private readonly Fido2 _fido2;
        private readonly Fido2Configuration _fido2Config;

        public Fido2Service(IDapperRepository dapperRepository, IJwtHelper jwtHelper, IOptions<Fido2Settings> fido2Settings)
        {
            _dapperRepository = dapperRepository;
            _jwtHelper = jwtHelper;

            var settings = fido2Settings.Value;
            var serverDomain = settings.ServerDomain;
            var serverName = settings.ServerName;

            _fido2Config = new Fido2Configuration
            {
                ServerDomain = serverDomain,
                ServerName = serverName,
                TimestampDriftTolerance = 300,
                ChallengeSize = 32,
                Origins = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    $"https://{serverDomain}",
                    $"http://{serverDomain}",
                    settings.Origin ?? string.Empty
                }
                .Concat(settings.AllowedOrigins)
                .Where(o => !string.IsNullOrEmpty(o))
                .ToHashSet(StringComparer.OrdinalIgnoreCase)
            };

            _fido2 = new Fido2(_fido2Config);
        }

        public async Task<Fido2ChallengeResponse> RequestAttestationOptionsAsync(int userId, string username)
        {
            var user = new Fido2User
            {
                Id = GetUserHandle(userId),
                Name = username,
                DisplayName = username
            };

            var options = _fido2.RequestNewCredential(new RequestNewCredentialParams
            {
                User = user,
                ExcludeCredentials = new List<PublicKeyCredentialDescriptor>(),
                AuthenticatorSelection = AuthenticatorSelection.Default,
                AttestationPreference = AttestationConveyancePreference.None,
                PubKeyCredParams = new List<PubKeyCredParam> { PubKeyCredParam.ES256 },
                Extensions = new AuthenticationExtensionsClientInputs()
            });

            var challenge = Convert.ToBase64String(options.Challenge);
            var expiresAt = DateTime.UtcNow.AddMinutes(10);

            await _dapperRepository.ExecuteAsync("sp_Users", new
            {
                AuthType = "FIDO",
                FIDOOperation = "CreateChallenge",
                UserId = userId,
                Challenge = challenge,
                ExpiresAt = expiresAt
            });

            return new Fido2ChallengeResponse
            {
                Id = challenge,
                Challenge = challenge,
                PublicKeyCredentialCreationOptions = options.ToJson()
            };
        }

        public async Task<Fido2VerifyResponse> RegisterCredentialAsync(Fido2RegisterRequest request)
        {
            var stored = await _dapperRepository.QueryFirstAsync<AuthChallenge>(
                "sp_Users",
                new { AuthType = "FIDO", FIDOOperation = "GetUserChallenge", UserId = request.UserId, Challenge = request.AttestationChallenge });

            if (stored == null)
            {
                return new Fido2VerifyResponse { Success = false, Message = "No valid challenge found for registration" };
            }

            var originalChallenge = Convert.FromBase64String(stored.Challenge);

            var originalOptions = CredentialCreateOptions.Create(
                _fido2Config,
                originalChallenge,
                new Fido2User
                {
                    Id = GetUserHandle(request.UserId),
                    Name = request.Username,
                    DisplayName = request.Username
                },
                AuthenticatorSelection.Default,
                AttestationConveyancePreference.None,
                new List<PublicKeyCredentialDescriptor>(),
                new AuthenticationExtensionsClientInputs(),
                new List<PubKeyCredParam> { PubKeyCredParam.ES256 });

            AuthenticatorAttestationRawResponse attestationResponse;
            try
            {
                attestationResponse = System.Text.Json.JsonSerializer.Deserialize<AuthenticatorAttestationRawResponse>(request.AttestationResponse)
                    ?? throw new FormatException("Could not parse attestation response");
            }
            catch (Exception ex)
            {
                return new Fido2VerifyResponse { Success = false, Message = $"Invalid attestation response: {ex.Message}" };
            }

            var makeCredentialParams = new MakeNewCredentialParams
            {
                AttestationResponse = attestationResponse,
                OriginalOptions = originalOptions,
                IsCredentialIdUniqueToUserCallback = async (args, ct) =>
                {
                    var existing = await _dapperRepository.QueryAsync<UserCredential>(
                        "sp_Users",
                        new { AuthType = "FIDO", FIDOOperation = "GetCredential", CredentialId = Convert.ToBase64String(args.CredentialId) });
                    return !existing.Any();
                }
            };

            try
            {
                var result = await _fido2.MakeNewCredentialAsync(makeCredentialParams);

                var credentialId = Convert.ToBase64String(result.Id);
                var publicKey = Convert.ToBase64String(result.PublicKey);
                var signCount = (long)result.SignCount;

                await _dapperRepository.ExecuteAsync("sp_Users", new
                {
                    AuthType = "FIDO",
                    FIDOOperation = "UpsertCredential",
                    UserId = request.UserId,
                    CredentialId = credentialId,
                    PublicKey = publicKey,
                    SignCount = signCount,
                    Transports = request.Transports
                });

                return new Fido2VerifyResponse { Success = true, Message = "Passkey registered successfully" };
            }
            catch (Fido2VerificationException ex)
            {
                return new Fido2VerifyResponse { Success = false, Message = $"Passkey registration failed: {ex.Message}" };
            }
        }

        public async Task<Fido2ChallengeResponse> CreateChallengeAsync(int userId)
        {
            var credentials = (await _dapperRepository.QueryAsync<UserCredential>(
                "sp_Users",
                new { AuthType = "FIDO", FIDOOperation = "GetCredentialsByUserId", UserId = userId })).ToList();

            if (!credentials.Any())
            {
                throw new InvalidOperationException("No FIDO2 credentials found for user");
            }

            var allowedCredentials = credentials.Select(c =>
            {
                var transports = ParseTransports(c.Transports);
                return new PublicKeyCredentialDescriptor(
                    PublicKeyCredentialType.PublicKey,
                    Base64UrlDecode(c.CredentialId),
                    transports
                );
            }).ToList();

            var options = _fido2.GetAssertionOptions(new GetAssertionOptionsParams
            {
                AllowedCredentials = allowedCredentials,
                UserVerification = UserVerificationRequirement.Required,
                Extensions = new AuthenticationExtensionsClientInputs()
            });

            var challenge = Convert.ToBase64String(options.Challenge);
            var expiresAt = DateTime.UtcNow.AddMinutes(10);

            await _dapperRepository.ExecuteAsync("sp_Users", new
            {
                AuthType = "FIDO",
                FIDOOperation = "CreateChallenge",
                UserId = userId,
                Challenge = challenge,
                ExpiresAt = expiresAt
            });

            return new Fido2ChallengeResponse
            {
                Id = challenge,
                Challenge = challenge,
                PublicKeyCredentialCreationOptions = options.ToJson()
            };
        }

        public async Task<Fido2VerifyResponse> VerifyAssertionAsync(Fido2VerifyRequest request)
        {
            if (request.UserId <= 0)
            {
                throw new ArgumentException("UserId is required for assertion");
            }

            var credentialIdBase64 = Convert.ToBase64String(Base64UrlDecode(request.CredentialId));
            var credential = await _dapperRepository.QueryFirstAsync<UserCredential>(
                "sp_Users",
                new { AuthType = "FIDO", FIDOOperation = "GetCredential", CredentialId = credentialIdBase64 });

            if (credential == null)
            {
                return new Fido2VerifyResponse { Success = false, Message = "Credential not found" };
            }

            var storedChallenge = await _dapperRepository.QueryFirstAsync<AuthChallenge>(
                "sp_Users",
                new { AuthType = "FIDO", FIDOOperation = "GetUserChallenge", UserId = request.UserId, Challenge = request.Challenge });

            if (storedChallenge == null)
            {
                return new Fido2VerifyResponse { Success = false, Message = "No valid challenge found for assertion" };
            }

            var originalChallenge = Convert.FromBase64String(storedChallenge.Challenge);

            var credentialIdBytes = Base64UrlDecode(request.CredentialId);
            var clientDataBytes = Base64UrlDecode(request.ClientDataJson);
            var authenticatorDataBytes = Base64UrlDecode(request.AuthenticatorData);
            var signatureBytes = Base64UrlDecode(request.Signature);
            var userHandleBytes = GetUserHandle(request.UserId);

            var storedPublicKey = Base64UrlDecode(credential.PublicKey);
            var storedCount = (uint)credential.SignCount;

            var userCredentials = (await _dapperRepository.QueryAsync<UserCredential>(
                "sp_Users",
                new { AuthType = "FIDO", FIDOOperation = "GetCredentialsByUserId", UserId = credential.UserId })).ToList();

            var allowedCredentials = userCredentials.Select(c =>
            {
                var transports = ParseTransports(c.Transports);
                return new PublicKeyCredentialDescriptor(
                    PublicKeyCredentialType.PublicKey,
                    Base64UrlDecode(c.CredentialId),
                    transports
                );
            }).ToList();

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
                IsUserHandleOwnerOfCredentialIdCallback = async (p, ct) => true
            };

            try
            {
                var result = await _fido2.MakeAssertionAsync(makeAssertionParams);

                // Many authenticators (Windows Hello, Touch ID, platform passkeys) report a
                // sign counter of 0 or a non-monotonic value. Only flag regression when a
                // previously recorded non-zero counter decreases; an all-zero counter is valid.
                if (storedCount != 0 && result.SignCount <= storedCount)
                {
                    return new Fido2VerifyResponse { Success = false, Message = "Counter regression detected" };
                }

                await _dapperRepository.ExecuteAsync("sp_Users", new
                {
                    AuthType = "FIDO",
                    FIDOOperation = "UpsertCredential",
                    UserId = credential.UserId,
                    CredentialId = credential.CredentialId,
                    PublicKey = credential.PublicKey,
                    SignCount = (long)result.SignCount,
                    Transports = credential.Transports
                });

                await _dapperRepository.ExecuteAsync("sp_Users", new
                {
                    AuthType = "FIDO",
                    FIDOOperation = "ConsumeChallenge",
                    UserId = request.UserId,
                    Challenge = request.Challenge
                });

                var user = await _dapperRepository.QueryFirstAsync<User>(
                    "sp_Users",
                    new { AuthType = "Login", UserId = credential.UserId });

                var username = user?.Username ?? credential.UserId.ToString();
                var token = _jwtHelper.GenerateToken(credential.UserId, username);

                return new Fido2VerifyResponse { Success = true, Token = token, Message = "FIDO2 verification successful" };
            }
            catch (Fido2VerificationException ex)
            {
                return new Fido2VerifyResponse { Success = false, Message = $"FIDO2 verification error: {ex.Message}" };
            }
        }

        private static byte[] GetUserHandle(int userId)
        {
            return BitConverter.GetBytes(userId);
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

        private static AuthenticatorTransport[]? ParseTransports(string? transports)
        {
            if (string.IsNullOrWhiteSpace(transports))
            {
                return null;
            }

            var trimmed = transports.Trim();
            if (trimmed.StartsWith("[", StringComparison.Ordinal))
            {
                return System.Text.Json.JsonSerializer.Deserialize<AuthenticatorTransport[]>(trimmed);
            }

            return trimmed
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(t => Enum.TryParse<AuthenticatorTransport>(t.Trim(), ignoreCase: true, out var parsed) ? parsed : (AuthenticatorTransport?)null)
                .Where(t => t.HasValue)
                .Select(t => t!.Value)
                .ToArray();
        }
    }
}
