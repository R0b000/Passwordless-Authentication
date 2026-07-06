CREATE OR ALTER PROCEDURE dbo.sp_Fido_GetCredential
    @CredentialId NVARCHAR(300)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1
        Id,
        UserId,
        CredentialId,
        PublicKey,
        SignCount,
        Transports,
        CreatedAt,
        UpdatedAt
    FROM dbo.UserCredentials
    WHERE CredentialId = @CredentialId;
END;
GO
