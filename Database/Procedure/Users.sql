-- ============================================================================
--  Legacy dispatcher (deprecated)
--  The monolithic sp_Users has been refactored into per-concern procedures to
--  follow the N-tier / single-responsibility architecture:
--    * Database/Procedure/Users/Register.sql   -> dbo.sp_Users_Register
--    * Database/Procedure/Users/Login.sql      -> dbo.sp_Users_Login
--    * Database/Procedure/Fido/*.sql           -> dbo.sp_Fido_*
--  This wrapper is retained only for backward compatibility and simply
--  forwards to the new procedures. Prefer calling the dedicated procedures.
-- ============================================================================

CREATE OR ALTER PROCEDURE dbo.sp_Users
    @AuthType NVARCHAR(20),
    @Username NVARCHAR(200) = NULL,
    @PasswordHash NVARCHAR(200) = NULL,
    @UserId INT = NULL,
    @FIDOOperation NVARCHAR(50) = NULL,
    @Challenge NVARCHAR(500) = NULL,
    @ExpiresAt DATETIME2 = NULL,
    @CredentialId NVARCHAR(300) = NULL,
    @PublicKey NVARCHAR(MAX) = NULL,
    @SignCount BIGINT = NULL,
    @Transports NVARCHAR(500) = NULL,
    @Now DATETIME2 = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @AuthType = 'Register'
    BEGIN
        EXEC dbo.sp_Users_Register @Username = @Username, @PasswordHash = @PasswordHash;
        RETURN;
    END

    IF @AuthType = 'Login'
    BEGIN
        EXEC dbo.sp_Users_Login @UserId = @UserId, @Username = @Username;
        RETURN;
    END

    IF @AuthType = 'FIDO'
    BEGIN
        IF @FIDOOperation = 'CreateChallenge'
            EXEC dbo.sp_Fido_CreateChallenge @UserId = @UserId, @Challenge = @Challenge, @ExpiresAt = @ExpiresAt;
        ELSE IF @FIDOOperation = 'ConsumeChallenge'
            EXEC dbo.sp_Fido_ConsumeChallenge @UserId = @UserId, @Challenge = @Challenge, @Now = @Now;
        ELSE IF @FIDOOperation = 'UpsertCredential'
            EXEC dbo.sp_Fido_UpsertCredential @UserId = @UserId, @CredentialId = @CredentialId,
                 @PublicKey = @PublicKey, @SignCount = @SignCount, @Transports = @Transports;
        ELSE IF @FIDOOperation = 'GetCredential'
            EXEC dbo.sp_Fido_GetCredential @CredentialId = @CredentialId;
        ELSE IF @FIDOOperation = 'GetCredentialsByUserId'
            EXEC dbo.sp_Fido_GetCredentialsByUserId @UserId = @UserId;
        RETURN;
    END
END;
GO
