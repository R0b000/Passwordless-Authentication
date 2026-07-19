CREATE TABLE [dbo].[RefreshTokens] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [UserId] INT NOT NULL,
    [TokenHash] NVARCHAR(500) NOT NULL,
    [ExpiresAt] DATETIME2 NOT NULL,
    [IsRevoked] BIT NOT NULL CONSTRAINT [DF_RefreshTokens_IsRevoked] DEFAULT (0),
    [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_RefreshTokens_CreatedAt] DEFAULT (SYSUTCDATETIME()),
    [RevokedAt] DATETIME2 NULL,
    [IpAddress] NVARCHAR(45) NULL,
    [UserAgent] NVARCHAR(500) NULL,
    [Location] NVARCHAR(200) NULL,
    [LastUsedAt] DATETIME2 NULL,
    CONSTRAINT [PK_RefreshTokens] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_RefreshTokens_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([Id]) ON DELETE CASCADE
);
GO

CREATE UNIQUE INDEX [IX_RefreshTokens_TokenHash] ON [dbo].[RefreshTokens] ([TokenHash]);
GO

CREATE INDEX [IX_RefreshTokens_UserId] ON [dbo].[RefreshTokens] ([UserId]);
GO

CREATE INDEX [IX_RefreshTokens_ExpiresAt] ON [dbo].[RefreshTokens] ([ExpiresAt]);
GO
