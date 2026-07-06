CREATE TABLE [dbo].[AuthChallenges] (
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [Challenge] NVARCHAR(500) NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_AuthChallenges_CreatedAt] DEFAULT (SYSUTCDATETIME()),
    [ExpiresAt] DATETIME2 NOT NULL,
    [UsedAt] DATETIME2 NULL,
    CONSTRAINT [PK_AuthChallenges] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AuthChallenges_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_AuthChallenges_UserId_ExpiresAt] ON [dbo].[AuthChallenges] ([UserId], [ExpiresAt]);
GO