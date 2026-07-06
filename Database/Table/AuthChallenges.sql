-- AuthChallenges: stores server-generated WebAuthn challenges.

CREATE TABLE IF NOT EXISTS dbo.AuthChallenges (
    Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_AuthChallenges PRIMARY KEY,
    UserId UNIQUEIDENTIFIER NOT NULL CONSTRAINT FK_AuthChallenges_Users FOREIGN KEY REFERENCES dbo.Users(Id) ON DELETE CASCADE,

    -- Store as string (base64url)
    Challenge NVARCHAR(500) NOT NULL,

    CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_AuthChallenges_CreatedAt DEFAULT (SYSUTCDATETIME()),
    ExpiresAt DATETIME2 NOT NULL,

    -- when consumed to prevent replay
    UsedAt DATETIME2 NULL
);

CREATE INDEX IF NOT EXISTS IX_AuthChallenges_UserId_ExpiresAt ON dbo.AuthChallenges(UserId, ExpiresAt);