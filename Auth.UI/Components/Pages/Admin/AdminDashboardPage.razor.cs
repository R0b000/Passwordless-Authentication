using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Shared.Data.Wrapper;
using Shared.UI.Components.Tabs;
using Shared.UI.Components.Table;
using Shared.UI.Components.Modal;
using Shared.UI.Components.Button;
using Shared.UI.Components.Toaster;
using Auth.Model.Models.Rbac;
using Auth.UI.Manager.Interface.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Auth.UI.Components.Pages.Admin
{
    public partial class AdminDashboardPage : ComponentBase
    {
        [Inject] public IRbacManager RbacManager { get; set; } = default!;

        protected bool IsLoading { get; set; } = true;
        protected List<TabItem> TabItems { get; set; } = new();
        protected TabItem ActiveTab { get; set; } = new();

        // Data Lists
        protected List<UserRoleResponse> Users { get; set; } = new();
        protected List<RoleDto> Roles { get; set; } = new();
        protected List<PermissionDto> Permissions { get; set; } = new();

        // Table Columns
        protected List<TableColumn<UserRoleResponse>> UserColumns { get; set; } = new();
        protected List<TableColumn<RoleDto>> RoleColumns { get; set; } = new();
        protected List<TableColumn<PermissionDto>> PermissionColumns { get; set; } = new();

        // Modal/Editing State
        protected Modal RoleModal { get; set; } = default!;
        protected UserRoleResponse? SelectedUser { get; set; }
        protected int TargetRoleId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            SetupTabs();
            SetupColumns();
            await LoadDataAsync();
        }

        private void SetupTabs()
        {
            TabItems = new List<TabItem>
            {
                new TabItem { Title = "Users", Icon = "user" },
                new TabItem { Title = "Roles & Permissions", Icon = "shield" },
                new TabItem { Title = "Permissions", Icon = "lock" }
            };
            ActiveTab = TabItems.First();
        }

        private void SetupColumns()
        {
            UserColumns = new List<TableColumn<UserRoleResponse>>
            {
                new TableColumn<UserRoleResponse> { Title = "User ID", Property = nameof(UserRoleResponse.UserId), Sortable = true },
                new TableColumn<UserRoleResponse> { Title = "Username", Property = nameof(UserRoleResponse.Username), Sortable = true },
                new TableColumn<UserRoleResponse> { Title = "Email", Property = nameof(UserRoleResponse.Email), Sortable = true },
                new TableColumn<UserRoleResponse> { 
                    Title = "Active Role", 
                    Property = nameof(UserRoleResponse.Role), 
                    Sortable = true,
                    Template = user => (RenderTreeBuilder builder) =>
                    {
                        var roleName = string.IsNullOrEmpty(user.Role) ? "None" : user.Role;
                        var badgeClass = roleName == "Admin" ? "bg-danger" : (roleName == "Seller" ? "bg-success" : "bg-primary");
                        builder.OpenElement(0, "span");
                        builder.AddAttribute(1, "class", $"badge {badgeClass}");
                        builder.AddContent(2, roleName);
                        builder.CloseElement();
                    }
                },
                new TableColumn<UserRoleResponse>
                {
                    Title = "Actions",
                    Sortable = false,
                    Template = user => (RenderTreeBuilder builder) =>
                    {
                        builder.OpenComponent<Button>(0);
                        builder.AddAttribute(1, "Variant", Button.ButtonVariant.Primary);
                        builder.AddAttribute(2, "Size", Button.ButtonSize.Small);
                        builder.AddAttribute(3, "IconName", "edit");
                        builder.AddAttribute(4, "ChildContent", (RenderFragment)(__builder => __builder.AddContent(5, "Manage Role")));
                        builder.AddAttribute(6, "OnClick", EventCallback.Factory.Create(this, () => OpenManageRole(user)));
                        builder.CloseComponent();
                    }
                }
            };

            RoleColumns = new List<TableColumn<RoleDto>>
            {
                new TableColumn<RoleDto> { Title = "Role ID", Property = nameof(RoleDto.Id), Sortable = true },
                new TableColumn<RoleDto> { Title = "Role Name", Property = nameof(RoleDto.Name), Sortable = true },
                new TableColumn<RoleDto> { Title = "Description", Property = nameof(RoleDto.Description), Sortable = true },
                new TableColumn<RoleDto> { 
                    Title = "System Role", 
                    Property = nameof(RoleDto.IsSystemRole), 
                    Sortable = true,
                    Template = role => (RenderTreeBuilder builder) =>
                    {
                        builder.OpenElement(0, "span");
                        builder.AddAttribute(1, "class", role.IsSystemRole ? "text-success font-semibold" : "text-muted");
                        builder.AddContent(2, role.IsSystemRole ? "Yes" : "No");
                        builder.CloseElement();
                    }
                },
                new TableColumn<RoleDto>
                {
                    Title = "Assigned Permissions",
                    Sortable = false,
                    Template = role => (RenderTreeBuilder builder) =>
                    {
                        builder.OpenElement(0, "div");
                        builder.AddAttribute(1, "class", "d-flex flex-wrap gap-1");
                        if (role.Permissions != null && role.Permissions.Any())
                        {
                            foreach (var perm in role.Permissions)
                            {
                                builder.OpenElement(2, "span");
                                builder.AddAttribute(3, "class", "badge bg-dark-subtle text-light border border-secondary");
                                builder.AddContent(4, perm);
                                builder.CloseElement();
                            }
                        }
                        else
                        {
                            builder.OpenElement(5, "span");
                            builder.AddAttribute(6, "class", "text-muted small italic");
                            builder.AddContent(7, "No permissions");
                            builder.CloseElement();
                        }
                        builder.CloseElement();
                    }
                }
            };

            PermissionColumns = new List<TableColumn<PermissionDto>>
            {
                new TableColumn<PermissionDto> { Title = "Permission ID", Property = nameof(PermissionDto.Id), Sortable = true },
                new TableColumn<PermissionDto> { Title = "Name", Property = nameof(PermissionDto.Name), Sortable = true },
                new TableColumn<PermissionDto> { Title = "Module", Property = nameof(PermissionDto.Module), Sortable = true },
                new TableColumn<PermissionDto> { Title = "Description", Property = nameof(PermissionDto.Description), Sortable = true }
            };
        }

        private async Task LoadDataAsync()
        {
            IsLoading = true;
            try
            {
                // Fetch large batch size to let Client-Side Table paginate
                var usersResponse = await RbacManager.GetUsersWithRolesAsync(1, 1000);
                if (usersResponse.Succeeded && usersResponse.Data?.Data != null)
                {
                    Users = usersResponse.Data.Data.ToList();
                }

                var rolesResponse = await RbacManager.GetRolesAsync(1, 1000);
                if (rolesResponse.Succeeded && rolesResponse.Data?.Data != null)
                {
                    Roles = rolesResponse.Data.Data.ToList();
                }

                var permissionsResponse = await RbacManager.GetPermissionsAsync(1, 1000);
                if (permissionsResponse.Succeeded && permissionsResponse.Data?.Data != null)
                {
                    Permissions = permissionsResponse.Data.Data.ToList();
                }
            }
            catch (Exception ex)
            {
                Toaster.ShowDanger($"Failed to load dashboard data: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        protected Task OnTabChanged(TabItem tab)
        {
            ActiveTab = tab;
            return Task.CompletedTask;
        }

        protected async Task OpenManageRole(UserRoleResponse user)
        {
            SelectedUser = user;
            TargetRoleId = 0;

            if (!string.IsNullOrEmpty(user.Role))
            {
                var currentRole = Roles.FirstOrDefault(r => r.Name.Equals(user.Role, StringComparison.OrdinalIgnoreCase));
                if (currentRole != null)
                {
                    TargetRoleId = currentRole.Id;
                }
            }

            await RoleModal.ShowAsync();
        }

        protected async Task SaveUserRoleAsync()
        {
            if (SelectedUser == null) return;

            try
            {
                // If user currently has a role, revoke it first
                if (!string.IsNullOrEmpty(SelectedUser.Role))
                {
                    var currentRole = Roles.FirstOrDefault(r => r.Name.Equals(SelectedUser.Role, StringComparison.OrdinalIgnoreCase));
                    if (currentRole != null)
                    {
                        var revokeResult = await RbacManager.RemoveRoleFromUserAsync(SelectedUser.UserId, currentRole.Id);
                        if (!revokeResult.Succeeded)
                        {
                            Toaster.ShowDanger($"Could not clear previous role: {revokeResult.Messages}");
                            return;
                        }
                    }
                }

                // If new role is selected, assign it
                if (TargetRoleId > 0)
                {
                    var assignResult = await RbacManager.AssignRoleToUserAsync(SelectedUser.UserId, TargetRoleId);
                    if (assignResult.Succeeded)
                    {
                        Toaster.ShowSuccess("User role updated successfully");
                    }
                    else
                    {
                        Toaster.ShowDanger($"Failed to assign role: {assignResult.Messages}");
                        return;
                    }
                }
                else
                {
                    Toaster.ShowSuccess("User role cleared successfully");
                }

                await RoleModal.HideAsync();
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                Toaster.ShowDanger($"An error occurred while saving: {ex.Message}");
            }
        }
    }
}
