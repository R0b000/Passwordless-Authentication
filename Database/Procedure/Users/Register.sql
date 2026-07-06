CREATE OR ALTER PROCEDURE dbo.sp_Users_Register
    @Username NVARCHAR(200),
    @PasswordHash NVARCHAR(200) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @NewUserId INT;

    SELECT @NewUserId = Id
    FROM dbo.Users
    WHERE Username = @Username;

    IF @NewUserId IS NULL
    BEGIN
        INSERT INTO dbo.Users (Username, PasswordHash)
        VALUES (@Username, @PasswordHash);

        SET @NewUserId = SCOPE_IDENTITY();
    END
    ELSE
    BEGIN
        IF @PasswordHash IS NOT NULL
        BEGIN
            UPDATE dbo.Users
            SET PasswordHash = @PasswordHash,
                UpdatedAt = SYSUTCDATETIME()
            WHERE Id = @NewUserId;
        END
    END

    SELECT @NewUserId AS UserId;
END;
GO
