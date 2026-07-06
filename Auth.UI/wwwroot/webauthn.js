export async function createCredential(optionsJson, challenge) {
    const options = JSON.parse(optionsJson);
    options.challenge = base64UrlToArrayBuffer(options.challenge);
    if (options.user) {
        options.user.id = base64UrlToArrayBuffer(options.user.id);
    }
    if (options.excludeCredentials) {
        options.excludeCredentials = options.excludeCredentials.map(c => ({
            ...c,
            id: base64UrlToArrayBuffer(c.id)
        }));
    }

    const cred = await navigator.credentials.create({ publicKey: options });
    return {
        id: bufferToBase64Url(cred.rawId),
        rawId: bufferToBase64Url(cred.rawId),
        type: cred.type,
        response: {
            clientDataJSON: bufferToBase64Url(cred.response.clientDataJSON),
            attestationObject: bufferToBase64Url(cred.response.attestationObject)
        },
        challenge: challenge,
        transports: cred.response.getTransports ? cred.response.getTransports() : []
    };
}

export async function getCredential(optionsJson, challenge) {
    const options = JSON.parse(optionsJson);
    options.challenge = base64UrlToArrayBuffer(options.challenge);
    if (options.allowCredentials) {
        options.allowCredentials = options.allowCredentials.map(c => ({
            ...c,
            id: base64UrlToArrayBuffer(c.id)
        }));
    }

    const cred = await navigator.credentials.get({ publicKey: options });
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
