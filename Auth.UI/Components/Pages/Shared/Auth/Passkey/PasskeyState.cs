namespace Auth.UI.Components.Pages.Shared.Auth.Passkey;

public enum PasskeyState
{
    Idle,
    Requesting,
    Awaiting,
    Verifying,
    Success,
    Error
}

public enum SetupState
{
    Idle,
    Processing,
    Success,
    Error
}
