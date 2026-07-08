CREATE TABLE [dbo].[EmailOtps] (
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [UserId] INT NOT NULL,
    [Otp] NVARCHAR(10) NOT NULL,
    [ExpiresAt] DATETIME2 NOT NULL,
    [UsedAt] DATETIME2 NULL,
    [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_EmailOtps_CreatedAt] DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT [PK_EmailOtps] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_EmailOtps_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_EmailOtps_UserId_ExpiresAt] ON [dbo].[EmailOtps] ([UserId], [ExpiresAt]);
GO
