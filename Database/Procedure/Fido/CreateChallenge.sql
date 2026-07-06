CREATE OR ALTER PROCEDURE dbo.sp_Fido_CreateChallenge
    @UserId INT,
    @Challenge NVARCHAR(500),
    @ExpiresAt DATETIME2
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @NewId UNIQUEIDENTIFIER = NEWID();

    INSERT INTO dbo.AuthChallenges (Id, UserId, Challenge, ExpiresAt, UsedAt)
    VALUES (@NewId, @UserId, @Challenge, @ExpiresAt, NULL);

    SELECT @NewId AS Id;
END;
GO
