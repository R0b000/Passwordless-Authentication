-- UserCredentials: stores WebAuthn/FIDO2 credential data.

CREATE TABLE IF NOT EXISTS dbo.UserCredentials (
    Id BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_UserCredentials PRIMARY KEY,
    UserId UNIQUEIDENTIFIER NOT NULL CONSTRAINT FK_UserCredentials_Users FOREIGN KEY REFERENCES dbo.Users(Id) ON DELETE CASCADE,

    -- WebAuthn credential identifier (base64url or hex stored as string)
    CredentialId NVARCHAR(300) NOT NULL,

    -- COSE public key (stored as JSON string for simplicity)
    PublicKey NVARCHAR(MAX) NOT NULL,

    -- WebAuthn signature counter (signCount)
    SignCount BIGINT NOT NULL CONSTRAINT DF_UserCredentials_SignCount DEFAULT (0),

    -- optional transports (json/string)
    Transports NVARCHAR(500) NULL,

    CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_UserCredentials_CreatedAt DEFAULT (SYSUTCDATETIME()),
    UpdatedAt DATETIME2 NOT NULL CONSTRAINT DF_UserCredentials_UpdatedAt DEFAULT (SYSUTCDATETIME())
);

CREATE UNIQUE INDEX IF NOT EXISTS IX_UserCredentials_UserId_CredentialId ON dbo.UserCredentials(UserId, CredentialId);
CREATE UNIQUE INDEX IF NOT EXISTS IX_UserCredentials_CredentialId ON dbo.UserCredentials(CredentialId);

