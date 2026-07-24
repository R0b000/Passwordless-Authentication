using System.Text.Json.Serialization;

namespace Auth.Model.Models.Auth
{
    public class WebAuthnAssertion
    {
        [JsonPropertyName("id")] public string id { get; set; } = string.Empty;
        [JsonPropertyName("rawId")] public string rawId { get; set; } = string.Empty;
        [JsonPropertyName("type")] public string type { get; set; } = string.Empty;
        [JsonPropertyName("challenge")] public string challenge { get; set; } = string.Empty;
        [JsonPropertyName("response")] public WebAuthnAssertionResponse response { get; set; } = new();
    }

    public class WebAuthnAssertionResponse
    {
        [JsonPropertyName("clientDataJSON")] public string clientDataJSON { get; set; } = string.Empty;
        [JsonPropertyName("authenticatorData")] public string authenticatorData { get; set; } = string.Empty;
        [JsonPropertyName("signature")] public string signature { get; set; } = string.Empty;
        [JsonPropertyName("userHandle")] public string? userHandle { get; set; }
    }
}


public class WebAuthnCredential
{
    [System.Text.Json.Serialization.JsonPropertyName("id")] public string id { get; set; } = string.Empty;
    [System.Text.Json.Serialization.JsonPropertyName("rawId")] public string rawId { get; set; } = string.Empty;
    [System.Text.Json.Serialization.JsonPropertyName("type")] public string type { get; set; } = string.Empty;
    [System.Text.Json.Serialization.JsonPropertyName("response")] public WebAuthnAttestationResponse response { get; set; } = new();
}

public class WebAuthnAttestationResponse
{
    [System.Text.Json.Serialization.JsonPropertyName("clientDataJSON")] public string clientDataJSON { get; set; } = string.Empty;
    [System.Text.Json.Serialization.JsonPropertyName("attestationObject")] public string attestationObject { get; set; } = string.Empty;
}
