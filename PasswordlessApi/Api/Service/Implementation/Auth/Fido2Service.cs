using System.Security;
using Fido2NetLib;
using Fido2NetLib.Objects;
using Microsoft.Extensions.Options;
using PasswordlessApi.Api.Common;
using PasswordlessApi.Api.Configuration;
using PasswordlessApi.Api.Models.Entities;
using PasswordlessApi.Api.Models.RequestModel.Auth;
using PasswordlessApi.Api.Models.ResponseModel.Auth;
using PasswordlessApi.Api.Service.Interface.Auth;
using PasswordlessApi.Api.Service.Interface.Repository;
using PasswordlessApi.Api.Utility.Jwt;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using PasswordlessApi.Api.Service.Interface.Security;
using PasswordlessApi.Api.Utility.TokenHash;
using System.Transactions;

namespace PasswordlessApi.Api.Service.Implementation.Auth
{
    public class Fido2Service : IFido2Service
    {
        private readonly IDapperRepository _dapperRepository;
        private readonly IJwtHelper _jwtHelper;
        private readonly ILogger<Fido2Service> _logger;
        private readonly ApiSettings _apiSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILocationResolver _locationResolver;

        public Fido2Service(IDapperRepository dapperRepository, IJwtHelper jwtHelper, IOptions<ApiSettings> apiSettings, ILogger<Fido2Service> logger, IHttpContextAccessor httpContextAccessor, ILocationResolver locationResolver)
        {
            _dapperRepository = dapperRepository;
            _jwtHelper = jwtHelper;
            _logger = logger;
            _apiSettings = apiSettings.Value;
            _httpContextAccessor = httpContextAccessor;
            _locationResolver = locationResolver;
        }

        private HashSet<string> GetAllowedOrigins()
        {
            return new HashSet<string>(_apiSettings.GetAllowedOrigins(), StringComparer.OrdinalIgnoreCase);
        }

        private string ExtractRpIdFromOrigin(string origin)
        {
            if (string.IsNullOrEmpty(origin))
                throw new ArgumentException("Origin is required for FIDO2 ceremonies");

            var uri = new Uri(origin);
            var domain = uri.Host;
            var allowedOrigins = GetAllowedOrigins();

            if (allowedOrigins.Contains("*"))
                return domain;

            var allowedHosts = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var o in allowedOrigins)
            {
                if (Uri.TryCreate(o, UriKind.Absolute, out var parsed))
                    allowedHosts.Add(parsed.Host);
            }

            if (!allowedHosts.Contains(domain))
                throw new SecurityException("Origin is not allowed for FIDO2 ceremonies");

            return domain;
        }

        private Fido2Configuration BuildConfig(string origin)
        {
            var rpId = string.IsNullOrEmpty(origin)
                ? _apiSettings.ResolveServerDomain()
                : ExtractRpIdFromOrigin(origin);

            var origins = GetAllowedOrigins();
            if (!string.IsNullOrEmpty(origin))
                origins.Add(origin);

            return new Fido2Configuration
            {
                ServerDomain = rpId,
                ServerName = _apiSettings.ServerName,
                TimestampDriftTolerance = 300,
                ChallengeSize = 32,
                Origins = origins
            };
        }

        public async Task<Fido2ChallengeResponse> RequestAttestationOptionsAsync(int userId, string username, string origin)
        {
            var config = BuildConfig(origin);
            var fido2 = new Fido2(config);
            var user = new Fido2User
            {
                Id = GetUserHandle(userId),
                Name = username,
                DisplayName = username
            };

            var options = fido2.RequestNewCredential(new RequestNewCredentialParams
            {
                User = user,
                ExcludeCredentials = new List<PublicKeyCredentialDescriptor>(),
                AuthenticatorSelection = AuthenticatorSelection.Default,
                AttestationPreference = AttestationConveyancePreference.None,
                PubKeyCredParams = new List<PubKeyCredParam> { PubKeyCredParam.ES256 }
            });

            var challenge = Convert.ToBase64String(options.Challenge);
            var expiresAt = DateTime.UtcNow.AddMinutes(10);

            await _dapperRepository.ExecuteAsync(DbConstants.Procedures.Users, new
            {
                AuthType = DbConstants.AuthTypes.Fido,
                FIDOOperation = DbConstants.FidoOperations.CreateChallenge,
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

        public async Task<Fido2VerifyResponse> RegisterCredentialAsync(Fido2RegisterRequest request, string origin)
        {
            if (request.UserId <= 0)
            {
                _logger.LogWarning("FIDO2 registration rejected: invalid UserId {UserId}", request.UserId);
                return new Fido2VerifyResponse { Success = false, Message = "Registration could not be completed for this account." };
            }

            var stored = await _dapperRepository.QueryFirstAsync<AuthChallenge>(
                DbConstants.Procedures.Users,
                new { AuthType = DbConstants.AuthTypes.Fido, FIDOOperation = DbConstants.FidoOperations.GetUserChallenge, UserId = request.UserId, Challenge = request.AttestationChallenge });

            if (stored == null)
            {
                return new Fido2VerifyResponse { Success = false, Message = "No valid challenge found for registration" };
            }

            var originalChallenge = Convert.FromBase64String(stored.Challenge);
            var config = BuildConfig(origin);

            var originalOptions = CredentialCreateOptions.Create(
                config,
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
                _logger.LogError(ex, "FIDO2 registration parsing failed for user {UserId}", request.UserId);
                return new Fido2VerifyResponse { Success = false, Message = "Invalid registration data." };
            }

            var makeCredentialParams = new MakeNewCredentialParams
            {
                AttestationResponse = attestationResponse,
                OriginalOptions = originalOptions,
                IsCredentialIdUniqueToUserCallback = async (args, ct) =>
                {
                    var existing = await _dapperRepository.QueryAsync<UserCredential>(
                        DbConstants.Procedures.Users,
                        new { AuthType = DbConstants.AuthTypes.Fido, FIDOOperation = DbConstants.FidoOperations.GetCredential, CredentialId = Convert.ToBase64String(args.CredentialId) });
                    return !existing.Any();
                }
            };

            try
            {
                var result = await new Fido2(config).MakeNewCredentialAsync(makeCredentialParams);

                // DB stores Standard Base64
                var credentialId = Convert.ToBase64String(result.Id);
                var publicKey = Convert.ToBase64String(result.PublicKey);
                var signCount = (long)result.SignCount;

                using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
                {
                    await _dapperRepository.ExecuteAsync(DbConstants.Procedures.Users, new
                    {
                        AuthType = DbConstants.AuthTypes.Fido,
                        FIDOOperation = DbConstants.FidoOperations.UpsertCredential,
                        UserId = request.UserId,
                        CredentialId = credentialId,
                        PublicKey = publicKey,
                        SignCount = signCount,
                        Transports = request.Transports
                    });

                    await _dapperRepository.ExecuteAsync(DbConstants.Procedures.Users, new
                    {
                        AuthType = DbConstants.AuthTypes.Fido,
                        FIDOOperation = DbConstants.FidoOperations.ConsumeChallenge,
                        UserId = request.UserId,
                        Challenge = request.AttestationChallenge
                    });

                    scope.Complete();
                }

                return new Fido2VerifyResponse { Success = true, Message = "Passkey registered successfully" };
            }
            catch (Fido2VerificationException ex)
            {
                return new Fido2VerifyResponse { Success = false, Message = $"Passkey registration failed: {ex.Message}" };
            }
        }

        public async Task<Fido2ChallengeResponse> CreateChallengeAsync(int userId, string origin)
        {
            var config = BuildConfig(origin);
            var fido2 = new Fido2(config);

            var credentials = (await _dapperRepository.QueryAsync<UserCredential>(
                DbConstants.Procedures.Users,
                new { AuthType = DbConstants.AuthTypes.Fido, FIDOOperation = DbConstants.FidoOperations.GetCredentialsByUserId, UserId = userId })).ToList();

            if (!credentials.Any())
                throw new InvalidOperationException("No FIDO2 credentials found for user");

            var allowedCredentials = credentials.Select(c =>
            {
                var transports = ParseTransports(c.Transports);
                return new PublicKeyCredentialDescriptor(
                    PublicKeyCredentialType.PublicKey,
                    Convert.FromBase64String(c.CredentialId), // FIXED: Was Base64UrlDecode which corrupts Standard Base64
                    transports
                );
            }).ToList();

            var options = fido2.GetAssertionOptions(new GetAssertionOptionsParams
            {
                AllowedCredentials = allowedCredentials,
                UserVerification = UserVerificationRequirement.Required
            });

            var challenge = Convert.ToBase64String(options.Challenge);
            var expiresAt = DateTime.UtcNow.AddMinutes(10);

            await _dapperRepository.ExecuteAsync(DbConstants.Procedures.Users, new
            {
                AuthType = DbConstants.AuthTypes.Fido,
                FIDOOperation = DbConstants.FidoOperations.CreateChallenge,
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

        public async Task<Fido2VerifyResponse> VerifyAssertionAsync(Fido2VerifyRequest request, string origin)
        {
            if (request.UserId <= 0)
            {
                throw new ArgumentException("UserId is required for assertion");
            }

            // UI sends Base64URL, we convert to Standard Base64 to match DB
            var credentialIdBase64 = Convert.ToBase64String(Base64UrlDecode(request.CredentialId));

            var credential = await _dapperRepository.QueryFirstAsync<UserCredential>(
                DbConstants.Procedures.Users,
                new { AuthType = DbConstants.AuthTypes.Fido, FIDOOperation = DbConstants.FidoOperations.GetCredential, CredentialId = credentialIdBase64 });

            if (credential == null)
            {
                return new Fido2VerifyResponse { Success = false, Message = "Credential not found" };
            }

            var storedChallenge = await _dapperRepository.QueryFirstAsync<AuthChallenge>(
                DbConstants.Procedures.Users,
                new { AuthType = DbConstants.AuthTypes.Fido, FIDOOperation = DbConstants.FidoOperations.GetUserChallenge, UserId = request.UserId, Challenge = request.Challenge });

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

            // FIXED: Was Base64UrlDecode which corrupts Standard Base64 from DB
            var storedPublicKey = Convert.FromBase64String(credential.PublicKey);
            var storedCount = (uint)credential.SignCount;

            var userCredentials = (await _dapperRepository.QueryAsync<UserCredential>(
                DbConstants.Procedures.Users,
                new { AuthType = DbConstants.AuthTypes.Fido, FIDOOperation = DbConstants.FidoOperations.GetCredentialsByUserId, UserId = credential.UserId })).ToList();

            var allowedCredentials = userCredentials.Select(c =>
            {
                var transports = ParseTransports(c.Transports);
                return new PublicKeyCredentialDescriptor(
                    PublicKeyCredentialType.PublicKey,
                    Convert.FromBase64String(c.CredentialId), // FIXED: Was Base64UrlDecode which corrupts Standard Base64
                    transports
                );
            }).ToList();

            var config = BuildConfig(origin);
            var originalOptions = AssertionOptions.Create(
                config,
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
                IsUserHandleOwnerOfCredentialIdCallback = async (args, ct) =>
                {
                    var claimedUserId = BitConverter.ToInt32(args.UserHandle);
                    var credentialIdBase64Callback = Convert.ToBase64String(args.CredentialId);
                    var dbCredential = await _dapperRepository.QueryFirstAsync<UserCredential>(
                        DbConstants.Procedures.Users,
                        new { AuthType = DbConstants.AuthTypes.Fido, FIDOOperation = DbConstants.FidoOperations.GetCredential, CredentialId = credentialIdBase64Callback });
                    return dbCredential != null && dbCredential.UserId == claimedUserId;
                }
            };

            try
            {
                var fido2 = new Fido2(config);
                var result = await fido2.MakeAssertionAsync(makeAssertionParams);

                if (storedCount != 0 && result.SignCount <= storedCount)
                {
                    return new Fido2VerifyResponse { Success = false, Message = "Counter regression detected" };
                }

                await _dapperRepository.ExecuteAsync(DbConstants.Procedures.Users, new
                {
                    AuthType = DbConstants.AuthTypes.Fido,
                    FIDOOperation = DbConstants.FidoOperations.UpsertCredential,
                    UserId = credential.UserId,
                    CredentialId = credential.CredentialId,
                    PublicKey = credential.PublicKey,
                    SignCount = (long)result.SignCount,
                    Transports = credential.Transports
                });

                await _dapperRepository.ExecuteAsync(DbConstants.Procedures.Users, new
                {
                    AuthType = DbConstants.AuthTypes.Fido,
                    FIDOOperation = DbConstants.FidoOperations.ConsumeChallenge,
                    UserId = request.UserId,
                    Challenge = request.Challenge
                });

                var user = await _dapperRepository.QueryFirstAsync<User>(
                    DbConstants.Procedures.Users,
                    new { AuthType = DbConstants.AuthTypes.Login, UserId = credential.UserId });

                var username = user?.Username ?? credential.UserId.ToString();
                var token = _jwtHelper.GenerateToken(credential.UserId, username);

                var ipAddress = GetClientIpAddress();
                var userAgent = GetUserAgent();
                var location = await _locationResolver.ResolveLocationAsync(ipAddress);

                var rawRefreshToken = _jwtHelper.GenerateRefreshToken();
                var refreshTokenHash = TokenHasher.HashToken(rawRefreshToken);
                var refreshExpiryDays = _jwtHelper.GetRefreshTokenExpiryDays();

                await _dapperRepository.ExecuteAsync(DbConstants.Procedures.Users, new
                {
                    AuthType = DbConstants.AuthTypes.RefreshToken,
                    FIDOOperation = DbConstants.FidoOperations.CreateRefreshToken,
                    UserId = credential.UserId,
                    TokenHash = refreshTokenHash,
                    ExpiresAt = DateTime.UtcNow.AddDays(refreshExpiryDays),
                    Now = DateTime.UtcNow,
                    IpAddress = ipAddress,
                    UserAgent = userAgent,
                    Location = location
                });

                return new Fido2VerifyResponse { Success = true, Token = token, RefreshToken = rawRefreshToken, Message = "FIDO2 verification successful" };
            }
            catch (Fido2VerificationException ex)
            {
                _logger.LogError(ex, "FIDO2 verification failed for user {UserId}", request.UserId);
                return new Fido2VerifyResponse { Success = false, Message = "Authentication failed. Please try again." };
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

        private string? GetClientIpAddress()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null) return null;
            if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
            {
                var first = forwardedFor.ToString().Split(',')[0].Trim();
                if (!string.IsNullOrEmpty(first))
                {
                    return first;
                }
            }
            return context.Connection.RemoteIpAddress?.ToString();
        }

        private string? GetUserAgent()
        {
            var context = _httpContextAccessor.HttpContext;
            return context?.Request.Headers["User-Agent"].ToString();
        }
    }
}