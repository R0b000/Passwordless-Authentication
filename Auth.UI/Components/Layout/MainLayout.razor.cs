using global::Shared.UI.Components.Menu;
using global::Auth.Model.Token;
using global::Auth.UI.Manager.Interface.Auth;
using Auth.Model.Models.Account;
using Microsoft.AspNetCore.Components;

namespace Auth.UI.Components.Layout
{
    public partial class MainLayout : LayoutComponentBase
    {

        protected UserProfile? Profile { get; set; }
        protected bool AccountMenuOpen { get; set; }
        protected bool _redirectToLogin;

        protected string AccountInitial =>
            string.IsNullOrEmpty(Profile?.DisplayName) ? "?" : Profile!.DisplayName[0].ToString().ToUpperInvariant();

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
            }
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
