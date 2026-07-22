CREATE TABLE [dbo].[UserPermissions] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [UserId] INT NOT NULL,
    [PermissionId] INT NOT NULL,
    [IsGranted] BIT NOT NULL CONSTRAINT [DF_UserPermissions_IsGranted] DEFAULT (1),
    [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_UserPermissions_CreatedAt] DEFAULT (SYSUTCDATETIME()),
    [ExpiresAt] DATETIME2 NULL,
    CONSTRAINT [PK_UserPermissions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_UserPermissions_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserPermissions_Permissions] FOREIGN KEY ([PermissionId]) REFERENCES [dbo].[Permissions]([Id]) ON DELETE CASCADE
);
GO

CREATE UNIQUE INDEX [IX_UserPermissions_UserId_PermissionId] ON [dbo].[UserPermissions] ([UserId], [PermissionId]) WHERE [ExpiresAt] IS NULL OR [ExpiresAt] > SYSUTCDATETIME();
GO

CREATE INDEX [IX_UserPermissions_UserId] ON [dbo].[UserPermissions] ([UserId]);
GO

CREATE INDEX [IX_UserPermissions_PermissionId] ON [dbo].[UserPermissions] ([PermissionId]);
GO
