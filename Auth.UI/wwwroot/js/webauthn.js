export async function createCredential(optionsJson, challenge) {
    const options = JSON.parse(optionsJson);

    // Helper to convert base64url to ArrayBuffer
    const base64urlToBuffer = (base64url) => {
        const base64 = base64url.replace(/-/g, '+').replace(/_/g, '/');
        const padLen = (4 - base64.length % 4) % 4;
        const padded = base64 + '='.repeat(padLen);
        const binary = atob(padded);
        const bytes = new Uint8Array(binary.length);
        for (let i = 0; i < binary.length; i++) bytes[i] = binary.charCodeAt(i);
        return bytes.buffer;
    };

    // Helper to convert ArrayBuffer to base64url
    const bufferToBase64url = (buffer) => {
        const bytes = new Uint8Array(buffer);
        let binary = '';
        for (let i = 0; i < bytes.byteLength; i++) binary += String.fromCharCode(bytes[i]);
        return btoa(binary).replace(/\+/g, '-').replace(/\//g, '_').replace(/=/g, '');
    };

    // WebAuthn requires ArrayBuffers for challenges and user IDs
    options.challenge = base64urlToBuffer(options.challenge);
    options.user.id = base64urlToBuffer(options.user.id);
    if (options.excludeCredentials) {
        for (let cred of options.excludeCredentials) {
            cred.id = base64urlToBuffer(cred.id);
        }
    }

    // Trigger the browser's native passkey/biometric creation prompt
    const credential = await navigator.credentials.create({ publicKey: options });

    return {
        id: credential.id,
        rawId: bufferToBase64url(credential.rawId),
        type: credential.type,
        response: {
            clientDataJSON: bufferToBase64url(credential.response.clientDataJSON),
            attestationObject: bufferToBase64url(credential.response.attestationObject)
        },
        transports: credential.response.getTransports ? credential.response.getTransports() : []
    };
}

// Request a FIDO2 / WebAuthn assertion (passkey or security key sign-in).
// `overrides` lets the UI steer the ceremony:
//   - authenticatorAttachment: "platform" | "cross-platform" (hint a security key)
//   - userVerification: "required" | "preferred" | "discouraged"
//   - timeout: override in milliseconds
//   - mediation: "conditional" (passkey autofill / conditional UI)
export async function getCredential(optionsJson, challenge, overrides = {}) {
    const options = JSON.parse(optionsJson);

    options.challenge = base64UrlToArrayBuffer(options.challenge);

    if (options.allowCredentials) {
        options.allowCredentials = options.allowCredentials.map(c => ({
            ...c,
            id: base64UrlToArrayBuffer(c.id)
        }));
    }

    if (overrides.authenticatorAttachment) {
        options.authenticatorAttachment = overrides.authenticatorAttachment;
    }
    if (overrides.userVerification) {
        options.userVerification = overrides.userVerification;
    }
    if (overrides.timeout) {
        options.timeout = overrides.timeout;
    }

    const request = { publicKey: options };
    if (overrides.mediation) {
        request.mediation = overrides.mediation;
    }

    const cred = await navigator.credentials.get(request);
    return {
        id: bufferToBase64Url(cred.rawId),
        rawId: bufferToBase64Url(cred.rawId),
        type: cred.type,
        challenge: challenge,
        response: {
            clientDataJSON: bufferToBase64Url(cred.response.clientDataJSON),
            authenticatorData: bufferToBase64Url(cred.response.authenticatorData),
            signature: bufferToBase64Url(cred.response.signature),
            userHandle: cred.response.userHandle ? bufferToBase64Url(cred.response.userHandle) : null
        }
    };
}

// Human-readable explanation for the WebAuthn errors users are most likely to hit.
export function describeWebAuthnError(error) {
    const name = error && error.name ? error.name : "Error";
    switch (name) {
        case "NotAllowedError":
            return "The passkey prompt was dismissed, cancelled, or timed out. Make sure your security key is connected (or your biometric reader is ready) and try again.";
        case "SecurityError":
            return "The authenticator request was blocked. Passkeys only work over a secure (HTTPS) origin and a registered domain.";
        case "NotSupportedError":
            return "This device or browser does not support passkeys. Try a different browser, a security key, or sign in with your password.";
        case "InvalidStateError":
            return "This passkey is already registered for your account.";
        case "UnknownError":
            return "The authenticator reported an unknown error. Try again or use another sign-in method.";
        default:
            return error && error.message ? error.message : "The passkey operation failed. Please try again.";
    }
}

function base64UrlToArrayBuffer(base64url) {
    const base64 = base64url.replace(/-/g, '+').replace(/_/g, '/');
    const pad = base64.length % 4 === 0 ? '' : '='.repeat(4 - (base64.length % 4));
    const binary = atob(base64 + pad);
    const bytes = new Uint8Array(binary.length);
    for (let i = 0; i < binary.length; i++) {
        bytes[i] = binary.charCodeAt(i);
    }
    return bytes.buffer;
}

function bufferToBase64Url(buffer) {
    const bytes = new Uint8Array(buffer);
    let binary = '';
    for (let i = 0; i < bytes.length; i++) {
        binary += String.fromCharCode(bytes[i]);
    }
    return btoa(binary).replace(/\+/g, '-').replace(/\//g, '_').replace(/=+$/, '');
}
