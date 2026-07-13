using System.Security.Claims;
using System.Text.Json;
using Auth.UI.src.Utility;
using Microsoft.AspNetCore.Components.Authorization;

namespace Auth.UI.src.Manager.Security
{
    public class TokenStoreAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ITokenStore _tokenStore;

        public TokenStoreAuthenticationStateProvider(ITokenStore tokenStore)
        {
            _tokenStore = tokenStore;
            _tokenStore.TokenChanged += OnTokenChanged;
        }

        private void OnTokenChanged() => NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = _tokenStore.GetToken();
            var user = string.IsNullOrWhiteSpace(token)
                ? new ClaimsPrincipal(new ClaimsIdentity())
                : new ClaimsPrincipal(ParseIdentity(token));
            return Task.FromResult(new AuthenticationState(user));
        }

        private static ClaimsIdentity ParseIdentity(string token)
        {
            var identity = new ClaimsIdentity("passkey");

            var parts = token.Split('.');
            if (parts.Length < 2)
            {
                identity.AddClaim(new Claim(ClaimTypes.Name, "user"));
                return identity;
            }

            try
            {
                var json = DecodeBase64Url(parts[1]);
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.ValueKind == JsonValueKind.Object)
                {
                    foreach (var prop in doc.RootElement.EnumerateObject())
                    {
                        var claimType = MapClaimType(prop.Name);
                        if (prop.Value.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var item in prop.Value.EnumerateArray())
                            {
                                if (item.ValueKind == JsonValueKind.String)
                                {
                                    identity.AddClaim(new Claim(claimType, item.GetString()!));
                                }
                            }
                        }
                        else if (prop.Value.ValueKind == JsonValueKind.String)
                        {
                            identity.AddClaim(new Claim(claimType, prop.Value.GetString()!));
                        }
                    }
                }
            }
            catch
            {
                identity.AddClaim(new Claim(ClaimTypes.Name, "user"));
            }

            if (!identity.Claims.Any())
            {
                identity.AddClaim(new Claim(ClaimTypes.Name, "user"));
            }

            return identity;
        }

        private static string MapClaimType(string name)
        {
            return name switch
            {
                "sub" => ClaimTypes.NameIdentifier,
                "nameid" => ClaimTypes.NameIdentifier,
                "unique_name" => ClaimTypes.Name,
                "name" => ClaimTypes.Name,
                "email" => ClaimTypes.Email,
                "role" => ClaimTypes.Role,
                _ => name
            };
        }

        private static string DecodeBase64Url(string input)
        {
            var base64 = input.Replace('-', '+').Replace('_', '/');
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }

            var bytes = Convert.FromBase64String(base64);
            return System.Text.Encoding.UTF8.GetString(bytes);
        }
    }
}
