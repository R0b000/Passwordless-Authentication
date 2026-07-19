CREATE TABLE [dbo].[UserRoles] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [UserId] INT NOT NULL,
    [RoleId] INT NOT NULL,
    [AssignedAt] DATETIME2 NOT NULL CONSTRAINT [DF_UserRoles_AssignedAt] DEFAULT (SYSUTCDATETIME()),
    [RevokedAt] DATETIME2 NULL,
    CONSTRAINT [PK_UserRoles] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_UserRoles_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserRoles_Roles] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles]([Id]) ON DELETE CASCADE
);
GO

CREATE UNIQUE INDEX [IX_UserRoles_UserId_RoleId] ON [dbo].[UserRoles] ([UserId], [RoleId]) WHERE [RevokedAt] IS NULL;
GO

CREATE INDEX [IX_UserRoles_UserId] ON [dbo].[UserRoles] ([UserId]);
GO

CREATE INDEX [IX_UserRoles_RoleId] ON [dbo].[UserRoles] ([RoleId]);
GO
