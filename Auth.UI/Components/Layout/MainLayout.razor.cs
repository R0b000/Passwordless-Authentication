using global::Shared.UI.Components.Menu;
using global::Auth.Model.Token;
using global::Auth.UI.Manager.Interface.Auth;
using Auth.Model.Models.Account;
using Microsoft.AspNetCore.Components;
using System.Text;
using System.Text.Json;

namespace Auth.UI.Components.Layout
{
    public partial class MainLayout : LayoutComponentBase
    {
        [Inject] public IRbacManager RbacManager { get; set; } = default!;

        protected UserProfile? Profile { get; set; }
        protected bool AccountMenuOpen { get; set; }
        protected bool _redirectToLogin;
        protected bool IsAdmin { get; set; }

        protected string AccountInitial =>
            string.IsNullOrEmpty(Profile?.DisplayName) ? "?" : Profile!.DisplayName[0].ToString().ToUpperInvariant();

        protected bool IsAuthPage
        {
            get
            {
                var path = Navigation.Uri.Replace(Navigation.BaseUri, "/").Split('?')[0].ToLowerInvariant();
                return path is "/" or "/login" or "/signup" or "/forgot-password" or "/reset-password" or "/verify-device";
            }
        }

        protected string Title
        {
            get
            {
                var path = Navigation.Uri.Replace(Navigation.BaseUri, "/").Split('?')[0];
                return path switch
                {
                    "/profile" => "Profile",
                    "/account/settings" => "Account Settings",
                    "/account/security" => "Security",
                    "/account/sessions" => "Active Sessions & Devices",
                    "/account/activity" => "Security Activity",
                    "/account/privacy" => "Privacy",
                    "/admin/dashboard" => "Admin Dashboard",
                    _ => "Account"
                };
            }
        }

        protected List<object> MenuItems { get; set; } = new()
        {
            new MenuHeaderItem { Text = "Account" },
            new MenuLinkItem { Text = "Profile", Url = "/profile", Icon = "user" },
            new MenuLinkItem { Text = "Settings", Url = "/account/settings", Icon = "sliders" },
            new MenuLinkItem { Text = "Security", Url = "/account/security", Icon = "shield" },
            new MenuLinkItem { Text = "Sessions & Devices", Url = "/account/sessions", Icon = "usb" },
            new MenuLinkItem { Text = "Activity Log", Url = "/account/activity", Icon = "eye" },
            new MenuLinkItem { Text = "Privacy", Url = "/account/privacy", Icon = "lock" },
            new MenuDivider(),
            new MenuActionItem { Text = "Sign out", Icon = "x", Key = "logout" }
        };

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                if (!await TokenStore.IsAvailableAsync())
                {
                    return;
                }

                var token = await TokenStore.GetToken();
                if (token is null)
                {
                    Navigation.NavigateTo("/login", replace: true);
                    return;
                }

                var result = await AccountManager.GetProfileAsync();
                Profile = result.Succeeded ? result.Data : null;

                // Extract userId from JWT payload (Base64URL decode — no external package needed)
                var userId = GetUserIdFromJwt(token);
                if (userId.HasValue)
                {
                    var rolesResult = await RbacManager.GetUserRolesAsync(userId.Value);
                    if (rolesResult.Succeeded && rolesResult.Data != null)
                    {
                        IsAdmin = rolesResult.Data.Any(r => r.Equals("Admin", StringComparison.OrdinalIgnoreCase));

                        if (IsAdmin)
                        {
                            // Insert Admin section before the existing divider
                            var dividerIndex = MenuItems.FindIndex(i => i is MenuDivider);
                            if (dividerIndex >= 0)
                            {
                                // Guard against double-injection on re-render
                                var alreadyAdded = MenuItems.OfType<MenuLinkItem>()
                                    .Any(i => i.Url == "/admin/dashboard");
                                if (!alreadyAdded)
                                {
                                    MenuItems.Insert(dividerIndex, new MenuDivider());
                                    MenuItems.Insert(dividerIndex, new MenuLinkItem
                                    {
                                        Text = "Admin Dashboard",
                                        Url = "/admin/dashboard",
                                        Icon = "settings"
                                    });
                                }
                            }
                        }
                    }
                }

                StateHasChanged();
            }
        }

        /// <summary>
        /// Reads the 'nameid' claim from the JWT payload using Base64URL decoding.
        /// Does not validate the token signature — only used for UI-side role display.
        /// </summary>
        private static int? GetUserIdFromJwt(string jwt)
        {
            try
            {
                var parts = jwt.Split('.');
                if (parts.Length != 3) return null;

                // Base64URL decode: replace chars and add padding
                var payload = parts[1]
                    .Replace('-', '+')
                    .Replace('_', '/');

                switch (payload.Length % 4)
                {
                    case 2: payload += "=="; break;
                    case 3: payload += "="; break;
                }

                var bytes = Convert.FromBase64String(payload);
                var json = Encoding.UTF8.GetString(bytes);

                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                // Try standard 'nameid' claim (what ClaimTypes.NameIdentifier maps to in JWT)
                if (root.TryGetProperty("nameid", out var nameIdEl) && nameIdEl.TryGetInt32(out var id))
                    return id;

                // Fallback: try full claim URI form
                if (root.TryGetProperty("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", out var fullEl) && fullEl.TryGetInt32(out var id2))
                    return id2;
            }
            catch
            {
                // Swallow parsing errors — menu will not show admin link
            }

            return null;
        }

        protected void ToggleAccountMenu() => AccountMenuOpen = !AccountMenuOpen;

        protected void CloseAccountMenu() => AccountMenuOpen = false;

        protected async Task OnMenuAction(MenuActionItem item)
        {
            if (item.Key == "logout")
            {
                await Logout();
            }
        }

        protected async Task Logout()
        {
            await TokenStore.Clear();
            AccountMenuOpen = false;
            Navigation.NavigateTo("/login", replace: true, forceLoad: true);
        }
    }
}
