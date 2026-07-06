CREATE OR ALTER PROCEDURE dbo.sp_Users_Login
    @UserId INT = NULL,
    @Username NVARCHAR(200) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @UserId IS NOT NULL
    BEGIN
        SELECT Id, Username, PasswordHash, CreatedAt, UpdatedAt
        FROM dbo.Users
        WHERE Id = @UserId;
    END
    ELSE IF @Username IS NOT NULL
    BEGIN
        DECLARE @FoundUserId INT;

        SELECT @FoundUserId = Id
        FROM dbo.Users
        WHERE Username = @Username;

        SELECT @FoundUserId AS UserId;
    END
END;
GO
