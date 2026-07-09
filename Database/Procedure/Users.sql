SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

CREATE OR ALTER PROCEDURE dbo.sp_Users
    @AuthType NVARCHAR(20),
    @Username NVARCHAR(200) = NULL,
    @Email NVARCHAR(300) = NULL,
    @PasswordHash NVARCHAR(200) = NULL,
    @UserId INT = NULL,
    @FIDOOperation NVARCHAR(50) = NULL,
    @Challenge NVARCHAR(500) = NULL,
    @ExpiresAt DATETIME2 = NULL,
    @CredentialId NVARCHAR(300) = NULL,
    @PublicKey NVARCHAR(MAX) = NULL,
    @SignCount BIGINT = NULL,
    @Transports NVARCHAR(500) = NULL,
    @Otp NVARCHAR(10) = NULL,
    @Now DATETIME2 = NULL,
    @TokenHash NVARCHAR(500) = NULL,
    @IpAddress NVARCHAR(45) = NULL,
    @UserAgent NVARCHAR(500) = NULL,
    @Location NVARCHAR(200) = NULL,
    @Action NVARCHAR(200) = NULL,
    @EntityType NVARCHAR(200) = NULL,
    @EntityId NVARCHAR(200) = NULL,
    @OldValue NVARCHAR(MAX) = NULL,
    @NewValue NVARCHAR(MAX) = NULL,
    @SessionId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @Now IS NULL SET @Now = SYSUTCDATETIME();

    IF @AuthType = 'Register'
    BEGIN
        DECLARE @NewUserId INT;

        SELECT @NewUserId = Id
        FROM dbo.Users
        WHERE Username = @Username;

        IF @NewUserId IS NULL
        BEGIN
            INSERT INTO dbo.Users (Username, Email, PasswordHash)
            VALUES (@Username, @Email, @PasswordHash);

            SET @NewUserId = SCOPE_IDENTITY();
        END
        ELSE
        BEGIN
            IF @PasswordHash IS NOT NULL
            BEGIN
                UPDATE dbo.Users
                SET PasswordHash = @PasswordHash,
                    Email = ISNULL(@Email, Email),
                    UpdatedAt = @Now
                WHERE Id = @NewUserId;
            END
        END

        SELECT @NewUserId AS UserId;
    END
    ELSE IF @AuthType = 'Login'
    BEGIN
        IF @UserId IS NOT NULL
        BEGIN
            SELECT Id, Username, Email, PasswordHash, CreatedAt, UpdatedAt
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
        ELSE IF @Email IS NOT NULL
        BEGIN
            SELECT Id, Username, Email, PasswordHash, CreatedAt, UpdatedAt
            FROM dbo.Users
            WHERE Email = @Email;
        END
    END
    ELSE IF @AuthType = 'FIDO'
    BEGIN
        IF @FIDOOperation = 'CreateChallenge'
        BEGIN
            DELETE FROM dbo.AuthChallenges WHERE UserId = @UserId;

            DECLARE @NewId UNIQUEIDENTIFIER = NEWID();

            INSERT INTO dbo.AuthChallenges (Id, UserId, Challenge, ExpiresAt, UsedAt)
            VALUES (@NewId, @UserId, @Challenge, @ExpiresAt, NULL);

            SELECT @NewId AS Id, @Challenge AS Challenge;
        END
        ELSE IF @FIDOOperation = 'GetUserChallenge'
        BEGIN
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
                    UpdatedAt = @Now
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
    ELSE IF @AuthType = 'EmailOtp'
    BEGIN
        IF @FIDOOperation = 'CreateOtp'
        BEGIN
            DECLARE @OtpId UNIQUEIDENTIFIER = NEWID();

            INSERT INTO dbo.EmailOtps (Id, UserId, Otp, ExpiresAt, UsedAt)
            VALUES (@OtpId, @UserId, @Otp, @ExpiresAt, NULL);

            SELECT @OtpId AS Id, @Otp AS Otp;
        END
        ELSE IF @FIDOOperation = 'GetOtp'
        BEGIN
            SELECT TOP 1 Id, Otp, ExpiresAt
            FROM dbo.EmailOtps
            WHERE UserId = @UserId
              AND Otp = @Otp
              AND UsedAt IS NULL
              AND ExpiresAt > @Now
            ORDER BY CreatedAt DESC;
        END
        ELSE IF @FIDOOperation = 'ConsumeOtp'
        BEGIN
            ;WITH c AS (
                SELECT TOP 1 Id
                FROM dbo.EmailOtps
                WHERE UserId = @UserId
                  AND Otp = @Otp
                  AND UsedAt IS NULL
                  AND ExpiresAt > @Now
                ORDER BY CreatedAt DESC
            )
            UPDATE eo
            SET UsedAt = @Now
            FROM dbo.EmailOtps eo
            INNER JOIN c ON c.Id = eo.Id;

            SELECT CASE WHEN EXISTS (
                SELECT 1
                FROM dbo.EmailOtps
                WHERE UserId = @UserId
                  AND Otp = @Otp
                  AND UsedAt IS NOT NULL
                  AND ExpiresAt > @Now
            ) THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS Consumed;
        END
    END
    ELSE IF @AuthType = 'RefreshToken'
    BEGIN
        IF @FIDOOperation = 'CreateRefreshToken'
        BEGIN
            INSERT INTO dbo.RefreshTokens (UserId, TokenHash, ExpiresAt, IpAddress, UserAgent, Location)
            VALUES (@UserId, @TokenHash, @ExpiresAt, @IpAddress, @UserAgent, @Location);

            SELECT TOP 1 Id, UserId, TokenHash, ExpiresAt, IsRevoked, CreatedAt, RevokedAt, IpAddress, UserAgent, Location, LastUsedAt
            FROM dbo.RefreshTokens
            WHERE TokenHash = @TokenHash;
        END
        ELSE IF @FIDOOperation = 'GetRefreshToken'
        BEGIN
            SELECT TOP 1 Id, UserId, TokenHash, ExpiresAt, IsRevoked, CreatedAt, RevokedAt, IpAddress, UserAgent, Location, LastUsedAt
            FROM dbo.RefreshTokens
            WHERE TokenHash = @TokenHash
              AND UserId = @UserId;
        END
        ELSE IF @FIDOOperation = 'GetRefreshTokenById'
        BEGIN
            SELECT TOP 1 Id, UserId, TokenHash, ExpiresAt, IsRevoked, CreatedAt, RevokedAt, IpAddress, UserAgent, Location, LastUsedAt
            FROM dbo.RefreshTokens
            WHERE Id = @SessionId
              AND UserId = @UserId;
        END
        ELSE IF @FIDOOperation = 'RevokeRefreshToken'
        BEGIN
            UPDATE dbo.RefreshTokens
            SET IsRevoked = 1,
                RevokedAt = @Now
            WHERE TokenHash = @TokenHash;

            SELECT @@ROWCOUNT AS RowsAffected;
        END
        ELSE IF @FIDOOperation = 'RevokeAllForUser'
        BEGIN
            UPDATE dbo.RefreshTokens
            SET IsRevoked = 1,
                RevokedAt = @Now
            WHERE UserId = @UserId
              AND IsRevoked = 0
              AND ExpiresAt > @Now;

            SELECT @@ROWCOUNT AS RowsAffected;
        END
        ELSE IF @FIDOOperation = 'GetActiveTokensForUser'
        BEGIN
            SELECT Id, UserId, TokenHash, ExpiresAt, IsRevoked, CreatedAt, RevokedAt, IpAddress, UserAgent, Location, LastUsedAt
            FROM dbo.RefreshTokens
            WHERE UserId = @UserId
              AND IsRevoked = 0
              AND ExpiresAt > @Now
            ORDER BY CreatedAt ASC;
        END
        ELSE IF @FIDOOperation = 'UpdateLastUsed'
        BEGIN
            UPDATE dbo.RefreshTokens
            SET LastUsedAt = @Now
            WHERE TokenHash = @TokenHash;

            SELECT @@ROWCOUNT AS RowsAffected;
        END
    END
    ELSE IF @AuthType = 'AuditLog'
    BEGIN
        IF @FIDOOperation = 'Create'
        BEGIN
            INSERT INTO dbo.AuditLogs (UserId, Action, EntityType, EntityId, OldValue, NewValue, IpAddress, UserAgent)
            VALUES (@UserId, @Action, @EntityType, @EntityId, @OldValue, @NewValue, @IpAddress, @UserAgent);

            SELECT SCOPE_IDENTITY() AS Id;
        END
        ELSE IF @FIDOOperation = 'GetByUser'
        BEGIN
            SELECT Id, UserId, Action, EntityType, EntityId, OldValue, NewValue, IpAddress, UserAgent, CreatedAt
            FROM dbo.AuditLogs
            WHERE UserId = @UserId
            ORDER BY CreatedAt DESC;
        END
    END
    ELSE IF @AuthType = 'GetProfile'
    BEGIN
        SELECT 
            Id AS UserId,
            Username,
            Email,
            Phone,
            Bio,
            AvatarUrl,
            CreatedAt AS DateJoined,
            UpdatedAt AS LastActive,
            AccountStatus
        FROM dbo.Users
        WHERE Id = @UserId;
    END
    ELSE IF @AuthType = 'UpdateProfile'
    BEGIN
        UPDATE dbo.Users
        SET Username = @Username,
            Email = @Email,
            Phone = @Phone,
            Bio = @Bio,
            UpdatedAt = @Now
        WHERE Id = @UserId;

        SELECT 
            Id AS UserId,
            Username,
            Email,
            Phone,
            Bio,
            AvatarUrl,
            CreatedAt AS DateJoined,
            UpdatedAt AS LastActive,
            AccountStatus
        FROM dbo.Users
        WHERE Id = @UserId;
    END
    ELSE IF @AuthType = 'GetSettings'
    BEGIN
        SELECT 
            Username,
            Email,
            DisplayName = Username,
            EmailPreferences,
            Timezone,
            Language,
            EmailNotifications,
            PushNotifications,
            SmsAlerts,
            MarketingEmails
        FROM dbo.Users
        WHERE Id = @UserId;
    END
    ELSE IF @AuthType = 'UpdateSettings'
    BEGIN
        UPDATE dbo.Users
        SET Username = @Username,
            Email = @Email,
            EmailPreferences = @EmailPreferences,
            Timezone = @Timezone,
            Language = @Language,
            EmailNotifications = @EmailNotifications,
            PushNotifications = @PushNotifications,
            SmsAlerts = @SmsAlerts,
            MarketingEmails = @MarketingEmails,
            UpdatedAt = @Now
        WHERE Id = @UserId;

        SELECT 
            Username,
            Email,
            DisplayName = Username,
            EmailPreferences,
            Timezone,
            Language,
            EmailNotifications,
            PushNotifications,
            SmsAlerts,
            MarketingEmails
        FROM dbo.Users
        WHERE Id = @UserId;
    END
    ELSE IF @AuthType = 'GetPrivacy'
    BEGIN
        SELECT 
            ProfileVisibility,
            DataSharing,
            ThirdPartyConnections,
            CookiePreferences
        FROM dbo.Users
        WHERE Id = @UserId;
    END
    ELSE IF @AuthType = 'UpdatePrivacy'
    BEGIN
        UPDATE dbo.Users
        SET ProfileVisibility = @ProfileVisibility,
            DataSharing = @DataSharing,
            ThirdPartyConnections = @ThirdPartyConnections,
            CookiePreferences = @CookiePreferences,
            UpdatedAt = @Now
        WHERE Id = @UserId;

        SELECT 
            ProfileVisibility,
            DataSharing,
            ThirdPartyConnections,
            CookiePreferences
        FROM dbo.Users
        WHERE Id = @UserId;
    END
    ELSE IF @AuthType = 'ResetPassword'
    BEGIN
        UPDATE dbo.Users
        SET PasswordHash = @PasswordHash,
            UpdatedAt = @Now
        WHERE Id = @UserId;

        SELECT @@ROWCOUNT AS RowsAffected;
    END
    ELSE IF @AuthType = 'DeleteAccount'
    BEGIN
        UPDATE dbo.Users
        SET AccountStatus = 'deleted',
            UpdatedAt = @Now
        WHERE Id = @UserId;

        SELECT @@ROWCOUNT AS RowsAffected;
    END
    ELSE IF @AuthType = 'GetSecurity'
    BEGIN
        SELECT 
            LastPasswordChange = UpdatedAt,
            TwoFactorEnabled,
            TwoFactorMethod,
            AlertOnNewDevice,
            RequirePasswordForSensitive
        FROM dbo.Users
        WHERE Id = @UserId;
    END
    ELSE IF @AuthType = 'UpdateSecurity'
    BEGIN
        UPDATE dbo.Users
        SET AlertOnNewDevice = @AlertOnNewDevice,
            RequirePasswordForSensitive = @RequirePasswordForSensitive,
            UpdatedAt = @Now
        WHERE Id = @UserId;

        SELECT 
            LastPasswordChange = UpdatedAt,
            TwoFactorEnabled,
            TwoFactorMethod,
            AlertOnNewDevice,
            RequirePasswordForSensitive
        FROM dbo.Users
        WHERE Id = @UserId;
    END
    ELSE IF @AuthType = 'ChangePassword'
    BEGIN
        UPDATE dbo.Users
        SET PasswordHash = @PasswordHash,
            UpdatedAt = @Now
        WHERE Id = @UserId;

        SELECT @@ROWCOUNT AS RowsAffected;
    END
    ELSE IF @AuthType = 'Enable2FA'
    BEGIN
        UPDATE dbo.Users
        SET TwoFactorEnabled = 1,
            UpdatedAt = @Now
        WHERE Id = @UserId;

        SELECT 
            LastPasswordChange = UpdatedAt,
            TwoFactorEnabled = 1,
            TwoFactorMethod,
            AlertOnNewDevice,
            RequirePasswordForSensitive
        FROM dbo.Users
        WHERE Id = @UserId;
    END
    ELSE IF @AuthType = 'Disable2FA'
    BEGIN
        UPDATE dbo.Users
        SET TwoFactorEnabled = 0,
            UpdatedAt = @Now
        WHERE Id = @UserId;

        SELECT 
            LastPasswordChange = UpdatedAt,
            TwoFactorEnabled = 0,
            TwoFactorMethod,
            AlertOnNewDevice,
            RequirePasswordForSensitive
        FROM dbo.Users
        WHERE Id = @UserId;
    END
END;
GO
