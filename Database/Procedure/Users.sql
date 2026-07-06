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
    END
    ELSE IF @AuthType = 'Login'
    BEGIN
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
    END
    ELSE IF @AuthType = 'FIDO'
    BEGIN
        IF @FIDOOperation = 'CreateChallenge'
        BEGIN
            DECLARE @NewId UNIQUEIDENTIFIER = NEWID();

            INSERT INTO dbo.AuthChallenges (Id, UserId, Challenge, ExpiresAt, UsedAt)
            VALUES (@NewId, @UserId, @Challenge, @ExpiresAt, NULL);

            SELECT @NewId AS Id, @Challenge AS Challenge;
        END
        ELSE IF @FIDOOperation = 'GetUserChallenge'
        BEGIN
            IF @Now IS NULL SET @Now = SYSUTCDATETIME();

            SELECT TOP 1 Id, Challenge, ExpiresAt
            FROM dbo.AuthChallenges
            WHERE UserId = @UserId
              AND Challenge = @Challenge
              AND UsedAt IS NULL
              AND ExpiresAt > @Now
            ORDER BY CreatedAt DESC;
        END
        ELSE IF @FIDOOperation = 'ConsumeChallenge'
        BEGIN
            IF @Now IS NULL SET @Now = SYSUTCDATETIME();

            ;WITH c AS (
                SELECT TOP 1 Id
                FROM dbo.AuthChallenges
                WHERE UserId = @UserId
                  AND Challenge = @Challenge
                  AND UsedAt IS NULL
                  AND ExpiresAt > @Now
                ORDER BY CreatedAt DESC
            )
            UPDATE ac
            SET UsedAt = @Now
            FROM dbo.AuthChallenges ac
            INNER JOIN c ON c.Id = ac.Id;

            SELECT CASE WHEN EXISTS (
                SELECT 1
                FROM dbo.AuthChallenges
                WHERE UserId = @UserId
                  AND Challenge = @Challenge
                  AND UsedAt IS NOT NULL
                  AND ExpiresAt > @Now
            ) THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS Consumed;
        END
        ELSE IF @FIDOOperation = 'UpsertCredential'
        BEGIN
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
        END
        ELSE IF @FIDOOperation = 'GetCredential'
        BEGIN
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
        END
        ELSE IF @FIDOOperation = 'GetCredentialsByUserId'
        BEGIN
            SELECT
                Id,
                UserId,
                CredentialId,
                PublicKey,
                SignCount,
                Transports,
                CreatedAt,
                UpdatedAt
            FROM dbo.UserCredentials
            WHERE UserId = @UserId;
        END
    END
END;
GO
