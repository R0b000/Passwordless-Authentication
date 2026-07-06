-- Users: supports initial password-based login and later FIDO2 login.

CREATE TABLE IF NOT EXISTS dbo.Users (
    Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_Users PRIMARY KEY,
    Username NVARCHAR(200) NOT NULL,
    PasswordHash NVARCHAR(200) NULL,
    CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Users_CreatedAt DEFAULT (SYSUTCDATETIME()),
    UpdatedAt DATETIME2 NOT NULL CONSTRAINT DF_Users_UpdatedAt DEFAULT (SYSUTCDATETIME())
);

CREATE UNIQUE INDEX IF NOT EXISTS IX_Users_Username ON dbo.Users(Username);

