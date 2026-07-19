namespace API.Shared.Common
{
    public static class DbConstants
    {
        public static class Procedures
        {
            public const string Users = "sp_Users";
            public const string Rbac = "sp_RBAC";
        }

        public static class AuthTypes
        {
            public const string Login = "Login";
            public const string Register = "Register";
            public const string Fido = "FIDO";
            public const string RefreshToken = "RefreshToken";
            public const string AuditLog = "AuditLog";
            public const string EmailOtp = "EmailOtp";
            public const string GetProfile = "GetProfile";
            public const string UpdateProfile = "UpdateProfile";
            public const string GetSettings = "GetSettings";
            public const string UpdateSettings = "UpdateSettings";
            public const string GetPrivacy = "GetPrivacy";
            public const string UpdatePrivacy = "UpdatePrivacy";
            public const string ResetPassword = "ResetPassword";
            public const string DeleteAccount = "DeleteAccount";
            public const string GetSecurity = "GetSecurity";
            public const string UpdateSecurity = "UpdateSecurity";
            public const string ChangePassword = "ChangePassword";
            public const string Enable2Fa = "Enable2FA";
            public const string Disable2Fa = "Disable2FA";
        }

        public static class FidoOperations
        {
            public const string GetCredentialsByUserId = "GetCredentialsByUserId";
            public const string CreateChallenge = "CreateChallenge";
            public const string GetUserChallenge = "GetUserChallenge";
            public const string GetCredential = "GetCredential";
            public const string UpsertCredential = "UpsertCredential";
            public const string ConsumeChallenge = "ConsumeChallenge";
            public const string GetRefreshToken = "GetRefreshToken";
            public const string CreateRefreshToken = "CreateRefreshToken";
            public const string RevokeRefreshToken = "RevokeRefreshToken";
            public const string RevokeAllForUser = "RevokeAllForUser";
            public const string GetRefreshTokenById = "GetRefreshTokenById";
            public const string GetActiveTokensForUser = "GetActiveTokensForUser";
        }

        public static class RbacActions
        {
            public const string CreateRole = "CreateRole";
            public const string GetAllRoles = "GetAllRoles";
            public const string GetRoleByName = "GetRoleByName";
            public const string GetRoleById = "GetRoleById";
            public const string GetRoleWithPermissions = "GetRoleWithPermissions";
            public const string AssignPermissionToRole = "AssignPermissionToRole";
            public const string RemovePermissionFromRole = "RemovePermissionFromRole";
            public const string DeleteRole = "DeleteRole";
            public const string AssignRoleToUser = "AssignRoleToUser";
            public const string RemoveRoleFromUser = "RemoveRoleFromUser";
            public const string GetUserRoles = "GetUserRoles";
            public const string GetUserRoleNames = "GetUserRoleNames";
            public const string GetUserPermissionNames = "GetUserPermissionNames";
            public const string GetUserWithRolesAndPermissions = "GetUserWithRolesAndPermissions";
            public const string HasPermission = "HasPermission";
            public const string IsInRole = "IsInRole";
            public const string GetUsersInRole = "GetUsersInRole";
            public const string GetAllUsersWithRoles = "GetAllUsersWithRoles";
            public const string CreatePermission = "CreatePermission";
            public const string GetAllPermissions = "GetAllPermissions";
            public const string GetPermissionsByNames = "GetPermissionsByNames";
            public const string GetPermissionByName = "GetPermissionByName";
            public const string GetPermissionNamesByRoleId = "GetPermissionNamesByRoleId";
        }

        public static class Email
        {
            public const string PasswordResetSubject = "Password Reset Request";
            public const string PasswordResetBodyTemplate = "Use the following token to reset your password: {0}";
        }
    }
}
