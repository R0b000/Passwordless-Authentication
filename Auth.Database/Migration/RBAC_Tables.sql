CREATE TABLE [dbo].[Roles] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Name] NVARCHAR(100) NOT NULL,
    [Description] NVARCHAR(500) NULL,
    [IsSystemRole] BIT NOT NULL CONSTRAINT [DF_Roles_IsSystemRole] DEFAULT (0),
    [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_Roles_CreatedAt] DEFAULT (SYSUTCDATETIME()),
    [UpdatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_Roles_UpdatedAt] DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT [PK_Roles] PRIMARY KEY ([Id])
);
GO

CREATE UNIQUE INDEX [IX_Roles_Name] ON [dbo].[Roles] ([Name]);
GO

CREATE TABLE [dbo].[Permissions] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Name] NVARCHAR(100) NOT NULL,
    [Description] NVARCHAR(500) NULL,
    [Module] NVARCHAR(100) NULL,
    [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_Permissions_CreatedAt] DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT [PK_Permissions] PRIMARY KEY ([Id])
);
GO

CREATE UNIQUE INDEX [IX_Permissions_Name] ON [dbo].[Permissions] ([Name]);
GO

CREATE INDEX [IX_Permissions_Module] ON [dbo].[Permissions] ([Module]);
GO

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

CREATE TABLE [dbo].[RolePermissions] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [RoleId] INT NOT NULL,
    [PermissionId] INT NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_RolePermissions_CreatedAt] DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT [PK_RolePermissions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_RolePermissions_Roles] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_RolePermissions_Permissions] FOREIGN KEY ([PermissionId]) REFERENCES [dbo].[Permissions]([Id]) ON DELETE CASCADE
);
GO

CREATE UNIQUE INDEX [IX_RolePermissions_RoleId_PermissionId] ON [dbo].[RolePermissions] ([RoleId], [PermissionId]);
GO

CREATE INDEX [IX_RolePermissions_PermissionId] ON [dbo].[RolePermissions] ([PermissionId]);
GO

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
