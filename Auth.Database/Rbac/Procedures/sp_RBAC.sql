SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

CREATE OR ALTER PROCEDURE dbo.sp_RBAC
    @RoleAction NVARCHAR(30) = NULL,
    @PermissionAction NVARCHAR(30) = NULL,
    @UserRoleAction NVARCHAR(30) = NULL,

    @RoleId INT = NULL,
    @Name NVARCHAR(100) = NULL,
    @Description NVARCHAR(500) = NULL,
    @IsSystemRole BIT = 0,

    @PermissionId INT = NULL,
    @PermissionName NVARCHAR(100) = NULL,
    @Module NVARCHAR(100) = NULL,

    @UserId INT = NULL,
    @Names NVARCHAR(MAX) = NULL,

    @Now DATETIME2 = NULL,
    @RoleName NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @Now IS NULL SET @Now = SYSUTCDATETIME();

    IF @RoleAction IS NOT NULL
    BEGIN
        IF @RoleAction = 'CreateRole'
        BEGIN
            INSERT INTO dbo.Roles (Name, Description, IsSystemRole)
            VALUES (@Name, @Description, @IsSystemRole);

            SELECT SCOPE_IDENTITY() AS Id;
        END
        ELSE IF @RoleAction = 'GetAllRoles'
        BEGIN
            SELECT Id, Name, Description, IsSystemRole, CreatedAt, UpdatedAt
            FROM dbo.Roles
            ORDER BY Name;
        END
        ELSE IF @RoleAction = 'GetRoleByName'
        BEGIN
            SELECT TOP 1 Id, Name, Description, IsSystemRole, CreatedAt, UpdatedAt
            FROM dbo.Roles
            WHERE Name = @Name;
        END
        ELSE IF @RoleAction = 'GetRoleById'
        BEGIN
            SELECT TOP 1 Id, Name, Description, IsSystemRole, CreatedAt, UpdatedAt
            FROM dbo.Roles
            WHERE Id = @RoleId;
        END
        ELSE IF @RoleAction = 'GetRoleWithPermissions'
        BEGIN
            SELECT TOP 1 Id, Name, Description, IsSystemRole, CreatedAt, UpdatedAt
            FROM dbo.Roles
            WHERE Id = @RoleId;

            SELECT p.Id, p.Name, p.Description, p.Module, p.CreatedAt
            FROM dbo.RolePermissions rp
            INNER JOIN dbo.Permissions p ON p.Id = rp.PermissionId
            WHERE rp.RoleId = @RoleId;
        END
        ELSE IF @RoleAction = 'AssignPermissionToRole'
        BEGIN
            IF NOT EXISTS (
                SELECT 1 FROM dbo.RolePermissions
                WHERE RoleId = @RoleId AND PermissionId = @PermissionId
            )
            BEGIN
                INSERT INTO dbo.RolePermissions (RoleId, PermissionId)
                VALUES (@RoleId, @PermissionId);
            END

            SELECT CAST(1 AS BIT) AS Result;
        END
        ELSE IF @RoleAction = 'RemovePermissionFromRole'
        BEGIN
            DELETE FROM dbo.RolePermissions
            WHERE RoleId = @RoleId AND PermissionId = @PermissionId;

            SELECT CAST(1 AS BIT) AS Result;
        END
        ELSE IF @RoleAction = 'DeleteRole'
        BEGIN
            DELETE FROM dbo.RolePermissions WHERE RoleId = @RoleId;
            DELETE FROM dbo.UserRoles WHERE RoleId = @RoleId;
            DELETE FROM dbo.Roles WHERE Id = @RoleId;

            SELECT CAST(1 AS BIT) AS Result;
        END
    END

    IF @PermissionAction IS NOT NULL
    BEGIN
        IF @PermissionAction = 'CreatePermission'
        BEGIN
            INSERT INTO dbo.Permissions (Name, Description, Module)
            VALUES (@Name, @Description, @Module);

            SELECT SCOPE_IDENTITY() AS Id;
        END
        ELSE IF @PermissionAction = 'GetAllPermissions'
        BEGIN
            SELECT Id, Name, Description, Module, CreatedAt
            FROM dbo.Permissions
            ORDER BY Module, Name;
        END
        ELSE IF @PermissionAction = 'GetPermissionByName'
        BEGIN
            SELECT TOP 1 Id, Name, Description, Module, CreatedAt
            FROM dbo.Permissions
            WHERE Name = @Name;
        END
        ELSE IF @PermissionAction = 'GetPermissionsByNames'
        BEGIN
            SELECT Id, Name, Description, Module, CreatedAt
            FROM dbo.Permissions
            WHERE Name IN (SELECT value FROM STRING_SPLIT(@Names, ','));
        END
        ELSE IF @PermissionAction = 'GetPermissionNamesByRoleId'
        BEGIN
            SELECT p.Name
            FROM dbo.RolePermissions rp
            INNER JOIN dbo.Permissions p ON p.Id = rp.PermissionId
            WHERE rp.RoleId = @RoleId;
        END
    END

    IF @UserRoleAction IS NOT NULL
    BEGIN
        IF @UserRoleAction = 'AssignRoleToUser'
        BEGIN
            IF EXISTS (
                SELECT 1 FROM dbo.UserRoles
                WHERE UserId = @UserId AND RoleId = @RoleId AND RevokedAt IS NULL
            )
            BEGIN
                SELECT CAST(1 AS BIT) AS Result;
                RETURN;
            END

            IF EXISTS (
                SELECT 1 FROM dbo.UserRoles
                WHERE UserId = @UserId AND RoleId = @RoleId AND RevokedAt IS NOT NULL
            )
            BEGIN
                UPDATE dbo.UserRoles
                SET AssignedAt = @Now, RevokedAt = NULL
                WHERE UserId = @UserId AND RoleId = @RoleId;
            END
            ELSE
            BEGIN
                INSERT INTO dbo.UserRoles (UserId, RoleId, AssignedAt)
                VALUES (@UserId, @RoleId, @Now);
            END

            SELECT CAST(1 AS BIT) AS Result;
        END
        ELSE IF @UserRoleAction = 'RemoveRoleFromUser'
        BEGIN
            UPDATE dbo.UserRoles
            SET RevokedAt = @Now
            WHERE UserId = @UserId AND RoleId = @RoleId AND RevokedAt IS NULL;

            SELECT CAST(1 AS BIT) AS Result;
        END
        ELSE IF @UserRoleAction = 'GetUserRoles'
        BEGIN
            SELECT ur.Id, ur.UserId, ur.RoleId, ur.AssignedAt, ur.RevokedAt
            FROM dbo.UserRoles ur
            WHERE ur.UserId = @UserId AND ur.RevokedAt IS NULL;
        END
        ELSE IF @UserRoleAction = 'GetUserRoleNames'
        BEGIN
            SELECT r.Name
            FROM dbo.UserRoles ur
            INNER JOIN dbo.Roles r ON r.Id = ur.RoleId
            WHERE ur.UserId = @UserId AND ur.RevokedAt IS NULL;
        END
        ELSE IF @UserRoleAction = 'GetUserPermissionNames'
        BEGIN
            SELECT DISTINCT p.Name
            FROM dbo.UserRoles ur
            INNER JOIN dbo.RolePermissions rp ON rp.RoleId = ur.RoleId
            INNER JOIN dbo.Permissions p ON p.Id = rp.PermissionId
            WHERE ur.UserId = @UserId AND ur.RevokedAt IS NULL
            UNION
            SELECT p.Name
            FROM dbo.UserPermissions up
            INNER JOIN dbo.Permissions p ON p.Id = up.PermissionId
            WHERE up.UserId = @UserId AND up.IsGranted = 1 AND (up.ExpiresAt IS NULL OR up.ExpiresAt > @Now);
        END
        ELSE IF @UserRoleAction = 'GetUserWithRolesAndPermissions'
        BEGIN
            SELECT TOP 1 Id, Username, Email, PasswordHash, CreatedAt, UpdatedAt
            FROM dbo.Users
            WHERE Id = @UserId;

            SELECT r.Name
            FROM dbo.UserRoles ur
            INNER JOIN dbo.Roles r ON r.Id = ur.RoleId
            WHERE ur.UserId = @UserId AND ur.RevokedAt IS NULL;

            SELECT DISTINCT p.Name AS PermissionName
            FROM dbo.UserRoles ur
            INNER JOIN dbo.RolePermissions rp ON rp.RoleId = ur.RoleId
            INNER JOIN dbo.Permissions p ON p.Id = rp.PermissionId
            WHERE ur.UserId = @UserId AND ur.RevokedAt IS NULL
            UNION
            SELECT p.Name AS PermissionName
            FROM dbo.UserPermissions up
            INNER JOIN dbo.Permissions p ON p.Id = up.PermissionId
            WHERE up.UserId = @UserId AND up.IsGranted = 1 AND (up.ExpiresAt IS NULL OR up.ExpiresAt > @Now);
        END
        ELSE IF @UserRoleAction = 'HasPermission'
        BEGIN
            IF EXISTS (
                SELECT 1
                FROM dbo.UserRoles ur
                INNER JOIN dbo.RolePermissions rp ON rp.RoleId = ur.RoleId
                INNER JOIN dbo.Permissions p ON p.Id = rp.PermissionId
                WHERE ur.UserId = @UserId
                  AND ur.RevokedAt IS NULL
                  AND p.Name = @PermissionName
            )
            BEGIN
                SELECT CAST(1 AS BIT) AS Result;
                RETURN;
            END

            IF EXISTS (
                SELECT 1
                FROM dbo.UserPermissions up
                INNER JOIN dbo.Permissions p ON p.Id = up.PermissionId
                WHERE up.UserId = @UserId
                  AND up.IsGranted = 1
                  AND (up.ExpiresAt IS NULL OR up.ExpiresAt > @Now)
                  AND p.Name = @PermissionName
            )
            BEGIN
                SELECT CAST(1 AS BIT) AS Result;
                RETURN;
            END

            SELECT CAST(0 AS BIT) AS Result;
        END
        ELSE IF @UserRoleAction = 'IsInRole'
        BEGIN
            SELECT CASE WHEN EXISTS (
                SELECT 1 FROM dbo.UserRoles
                WHERE UserId = @UserId AND RevokedAt IS NULL
            ) THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS Result;
        END
        ELSE IF @UserRoleAction = 'GetUsersInRole'
        BEGIN
            SELECT u.Id, u.Username, u.Email, u.PasswordHash, u.CreatedAt, u.UpdatedAt
            FROM dbo.Users u
            INNER JOIN dbo.UserRoles ur ON ur.UserId = u.Id
            WHERE ur.RevokedAt IS NULL AND ur.RoleId = (
                SELECT Id FROM dbo.Roles WHERE Name = @RoleName
            );
        END
        ELSE IF @UserRoleAction = 'GetAllUsersWithRoles'
        BEGIN
            SELECT DISTINCT u.Id, u.Username, u.Email, u.PasswordHash, u.CreatedAt, u.UpdatedAt
            FROM dbo.Users u
            INNER JOIN dbo.UserRoles ur ON ur.UserId = u.Id
            WHERE ur.RevokedAt IS NULL
            ORDER BY u.Username;
        END
    END
END;
GO
