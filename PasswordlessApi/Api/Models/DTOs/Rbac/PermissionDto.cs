namespace PasswordlessApi.Api.Models.DTOs.Rbac
{
    public class PermissionDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Module { get; set; }
    }
}
