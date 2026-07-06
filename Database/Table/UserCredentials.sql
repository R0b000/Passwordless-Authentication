CREATE TABLE [dbo].[UserCredentials] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [CredentialId] NVARCHAR(300) NOT NULL,
    [PublicKey] NVARCHAR(MAX) NOT NULL,
    [SignCount] BIGINT NOT NULL CONSTRAINT [DF_UserCredentials_SignCount] DEFAULT (0),
    [Transports] NVARCHAR(500) NULL,
    [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_UserCredentials_CreatedAt] DEFAULT (SYSUTCDATETIME()),
    [UpdatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_UserCredentials_UpdatedAt] DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT [PK_UserCredentials] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_UserCredentials_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([Id]) ON DELETE CASCADE
);
GO

CREATE UNIQUE INDEX [IX_UserCredentials_UserId_CredentialId] ON [dbo].[UserCredentials] ([UserId], [CredentialId]);
GO

CREATE UNIQUE INDEX [IX_UserCredentials_CredentialId] ON [dbo].[UserCredentials] ([CredentialId]);
GO 