CREATE OR ALTER PROCEDURE dbo.sp_Fido_GetCredentialsByUserId
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, UserId, CredentialId, PublicKey, SignCount, Transports, CreatedAt, UpdatedAt
    FROM dbo.UserCredentials
    WHERE UserId = @UserId;
END;
GO
