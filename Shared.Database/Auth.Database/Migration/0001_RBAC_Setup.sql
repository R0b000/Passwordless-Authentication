SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

IF OBJECT_ID('dbo.Roles', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Roles] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Name] NVARCHAR(100) NOT NULL,
        [Description] NVARCHAR(500) NULL,
        [IsSystemRole] BIT NOT NULL CONSTRAINT [DF_Roles_IsSystemRole] DEFAULT (0),
        [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_Roles_CreatedAt] DEFAULT (SYSUTCDATETIME()),
        [UpdatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_Roles_UpdatedAt] DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT [PK_Roles] PRIMARY KEY ([Id])
    );
    CREATE UNIQUE INDEX [IX_Roles_Name] ON [dbo].[Roles] ([Name]);
END
GO

IF OBJECT_ID('dbo.Permissions', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Permissions] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Name] NVARCHAR(100) NOT NULL,
        [Description] NVARCHAR(500) NULL,
        [Module] NVARCHAR(100) NULL,
        [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_Permissions_CreatedAt] DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT [PK_Permissions] PRIMARY KEY ([Id])
    );
    CREATE UNIQUE INDEX [IX_Permissions_Name] ON [dbo].[Permissions] ([Name]);
    CREATE INDEX [IX_Permissions_Module] ON [dbo].[Permissions] ([Module]);
END
GO

IF OBJECT_ID('dbo.UserRoles', 'U') IS NULL
BEGIN
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
    CREATE UNIQUE INDEX [IX_UserRoles_UserId_RoleId] ON [dbo].[UserRoles] ([UserId], [RoleId]) WHERE [RevokedAt] IS NULL;
    CREATE INDEX [IX_UserRoles_UserId] ON [dbo].[UserRoles] ([UserId]);
    CREATE INDEX [IX_UserRoles_RoleId] ON [dbo].[UserRoles] ([RoleId]);
END
GO

IF OBJECT_ID('dbo.RolePermissions', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[RolePermissions] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [RoleId] INT NOT NULL,
        [PermissionId] INT NOT NULL,
        [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_RolePermissions_CreatedAt] DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT [PK_RolePermissions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_RolePermissions_Roles] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles]([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_RolePermissions_Permissions] FOREIGN KEY ([PermissionId]) REFERENCES [dbo].[Permissions]([Id]) ON DELETE CASCADE
    );
    CREATE UNIQUE INDEX [IX_RolePermissions_RoleId_PermissionId] ON [dbo].[RolePermissions] ([RoleId], [PermissionId]);
    CREATE INDEX [IX_RolePermissions_PermissionId] ON [dbo].[RolePermissions] ([PermissionId]);
END
GO

IF OBJECT_ID('dbo.UserPermissions', 'U') IS NULL
BEGIN
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
    CREATE UNIQUE INDEX [IX_UserPermissions_UserId_PermissionId] ON [dbo].[UserPermissions] ([UserId], [PermissionId]) WHERE [ExpiresAt] IS NULL OR [ExpiresAt] > SYSUTCDATETIME();
    CREATE INDEX [IX_UserPermissions_UserId] ON [dbo].[UserPermissions] ([UserId]);
    CREATE INDEX [IX_UserPermissions_PermissionId] ON [dbo].[UserPermissions] ([PermissionId]);
END
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] WHERE [Name] = 'users.read')
    INSERT INTO [dbo].[Permissions] ([Name], [Description], [Module]) VALUES ('users.read', 'View users', 'Users');
IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] WHERE [Name] = 'users.write')
    INSERT INTO [dbo].[Permissions] ([Name], [Description], [Module]) VALUES ('users.write', 'Create/update users', 'Users');
IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] WHERE [Name] = 'users.delete')
    INSERT INTO [dbo].[Permissions] ([Name], [Description], [Module]) VALUES ('users.delete', 'Delete users', 'Users');
IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] WHERE [Name] = 'roles.read')
    INSERT INTO [dbo].[Permissions] ([Name], [Description], [Module]) VALUES ('roles.read', 'View roles', 'Roles');
IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] WHERE [Name] = 'roles.write')
    INSERT INTO [dbo].[Permissions] ([Name], [Description], [Module]) VALUES ('roles.write', 'Create/update roles', 'Roles');
IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] WHERE [Name] = 'roles.delete')
    INSERT INTO [dbo].[Permissions] ([Name], [Description], [Module]) VALUES ('roles.delete', 'Delete roles', 'Roles');
IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] WHERE [Name] = 'auth.read')
    INSERT INTO [dbo].[Permissions] ([Name], [Description], [Module]) VALUES ('auth.read', 'View auth settings', 'Auth');
IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] WHERE [Name] = 'auth.write')
    INSERT INTO [dbo].[Permissions] ([Name], [Description], [Module]) VALUES ('auth.write', 'Modify auth settings', 'Auth');
IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] WHERE [Name] = 'dashboard.view')
    INSERT INTO [dbo].[Permissions] ([Name], [Description], [Module]) VALUES ('dashboard.view', 'Access dashboard', 'Dashboard');
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[Roles] WHERE [Name] = 'Admin')
    INSERT INTO [dbo].[Roles] ([Name], [Description], [IsSystemRole]) VALUES ('Admin', 'System administrator role with full permissions', 1);
IF NOT EXISTS (SELECT 1 FROM [dbo].[Roles] WHERE [Name] = 'User')
    INSERT INTO [dbo].[Roles] ([Name], [Description], [IsSystemRole]) VALUES ('User', 'Standard user role with basic permissions', 1);
GO

DECLARE @AdminRoleId INT = (SELECT Id FROM dbo.Roles WHERE Name = 'Admin');
DECLARE @UserRoleId INT = (SELECT Id FROM dbo.Roles WHERE Name = 'User');

DECLARE @UsersRead INT = (SELECT Id FROM dbo.Permissions WHERE Name = 'users.read');
DECLARE @UsersWrite INT = (SELECT Id FROM dbo.Permissions WHERE Name = 'users.write');
DECLARE @UsersDelete INT = (SELECT Id FROM dbo.Permissions WHERE Name = 'users.delete');
DECLARE @RolesRead INT = (SELECT Id FROM dbo.Permissions WHERE Name = 'roles.read');
DECLARE @RolesWrite INT = (SELECT Id FROM dbo.Permissions WHERE Name = 'roles.write');
DECLARE @RolesDelete INT = (SELECT Id FROM dbo.Permissions WHERE Name = 'roles.delete');
DECLARE @AuthRead INT = (SELECT Id FROM dbo.Permissions WHERE Name = 'auth.read');
DECLARE @AuthWrite INT = (SELECT Id FROM dbo.Permissions WHERE Name = 'auth.write');
DECLARE @DashboardView INT = (SELECT Id FROM dbo.Permissions WHERE Name = 'dashboard.view');

IF @UsersRead IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.RolePermissions WHERE RoleId = @AdminRoleId AND PermissionId = @UsersRead)
    INSERT INTO dbo.RolePermissions (RoleId, PermissionId) VALUES (@AdminRoleId, @UsersRead);
IF @UsersWrite IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.RolePermissions WHERE RoleId = @AdminRoleId AND PermissionId = @UsersWrite)
    INSERT INTO dbo.RolePermissions (RoleId, PermissionId) VALUES (@AdminRoleId, @UsersWrite);
IF @UsersDelete IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.RolePermissions WHERE RoleId = @AdminRoleId AND PermissionId = @UsersDelete)
    INSERT INTO dbo.RolePermissions (RoleId, PermissionId) VALUES (@AdminRoleId, @UsersDelete);
IF @RolesRead IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.RolePermissions WHERE RoleId = @AdminRoleId AND PermissionId = @RolesRead)
    INSERT INTO dbo.RolePermissions (RoleId, PermissionId) VALUES (@AdminRoleId, @RolesRead);
IF @RolesWrite IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.RolePermissions WHERE RoleId = @AdminRoleId AND PermissionId = @RolesWrite)
    INSERT INTO dbo.RolePermissions (RoleId, PermissionId) VALUES (@AdminRoleId, @RolesWrite);
IF @RolesDelete IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.RolePermissions WHERE RoleId = @AdminRoleId AND PermissionId = @RolesDelete)
    INSERT INTO dbo.RolePermissions (RoleId, PermissionId) VALUES (@AdminRoleId, @RolesDelete);
IF @AuthRead IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.RolePermissions WHERE RoleId = @AdminRoleId AND PermissionId = @AuthRead)
    INSERT INTO dbo.RolePermissions (RoleId, PermissionId) VALUES (@AdminRoleId, @AuthRead);
IF @AuthWrite IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.RolePermissions WHERE RoleId = @AdminRoleId AND PermissionId = @AuthWrite)
    INSERT INTO dbo.RolePermissions (RoleId, PermissionId) VALUES (@AdminRoleId, @AuthWrite);
IF @DashboardView IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.RolePermissions WHERE RoleId = @AdminRoleId AND PermissionId = @DashboardView)
    INSERT INTO dbo.RolePermissions (RoleId, PermissionId) VALUES (@AdminRoleId, @DashboardView);

IF @UsersRead IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.RolePermissions WHERE RoleId = @UserRoleId AND PermissionId = @UsersRead)
    INSERT INTO dbo.RolePermissions (RoleId, PermissionId) VALUES (@UserRoleId, @UsersRead);
IF @DashboardView IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.RolePermissions WHERE RoleId = @UserRoleId AND PermissionId = @DashboardView)
    INSERT INTO dbo.RolePermissions (RoleId, PermissionId) VALUES (@UserRoleId, @DashboardView);
GO
