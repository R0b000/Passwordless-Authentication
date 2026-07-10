SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[Roles] WHERE [Name] = 'Admin')
BEGIN
    INSERT INTO [dbo].[Roles] ([Name], [Description], [IsSystemRole])
    VALUES ('Admin', 'System administrator role with full permissions', 1);
END
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[Roles] WHERE [Name] = 'User')
BEGIN
    INSERT INTO [dbo].[Roles] ([Name], [Description], [IsSystemRole])
    VALUES ('User', 'Standard user role with basic permissions', 1);
END
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
