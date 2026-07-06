# Passwordless-Authentication

## Current security and auth status

This repository currently implements a username/password authentication flow with BCrypt password hashing and a JWT-based bearer token flow for successful login and registration responses.

### What was verified
- The API builds successfully with dotnet build.
- JWT authentication is now wired to issue signed bearer tokens and validate them with issuer, audience, and expiry checks.
- FIDO2/WebAuthn is not implemented in this repository yet; the current authentication flow is still username/password based.

### Vulnerabilities and fixes applied
- The previous JWT setup used a hard-coded fallback secret, which is insecure. It now requires a configured signing secret and fails fast when one is missing.
- JWT validation now enforces issuer and audience checks plus token lifetime validation.
- Successful login and registration now return a JWT token in the response payload.
- An authenticated endpoint is available at /api/auth/me to verify that JWT validation works end to end.

### How to run locally
1. Set a strong secret value using either the JwtSettings:SecretKey configuration or the JWT_SECRET_KEY environment variable.
2. Run the API with dotnet run from the PasswordlessApi folder.
3. Call /api/auth/login or /api/auth/register to receive a JWT token, then call /api/auth/me with the bearer token in the Authorization header.

### Note on FIDO2
FIDO2/WebAuthn support is not present yet. If you want, the next step can be to add a WebAuthn registration/login flow using a FIDO2-compatible library and a database-backed credential store.