CREATE OR ALTER PROCEDURE dbo.sp_Fido_UpsertCredential
    @UserId INT,
    @CredentialId NVARCHAR(300),
    @PublicKey NVARCHAR(MAX),
    @SignCount BIGINT,
    @Transports NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1 FROM dbo.UserCredentials
        WHERE UserId = @UserId AND CredentialId = @CredentialId
    )
    BEGIN
        UPDATE dbo.UserCredentials
        SET PublicKey = @PublicKey,
            SignCount = @SignCount,
            Transports = @Transports,
            UpdatedAt = SYSUTCDATETIME()
        WHERE UserId = @UserId AND CredentialId = @CredentialId;
    END
    ELSE
    BEGIN
        INSERT INTO dbo.UserCredentials (UserId, CredentialId, PublicKey, SignCount, Transports)
        VALUES (@UserId, @CredentialId, @PublicKey, @SignCount, @Transports);
    END

    SELECT 1 AS Result;
END;
GO
