using Auth.UI.Shared.Components.Menu;
using Auth.UI.Shared.Model.Account;
using Auth.UI.Shared.Utility;
using Microsoft.AspNetCore.Components;

namespace Auth.UI.Components.Layout
{
    public partial class AccountLayout : LayoutComponentBase
    {
        [Inject] private IAccountManager AccountController { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;
        [Inject] private ITokenStore TokenStore { get; set; } = default!;

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

        protected override async Task OnInitializedAsync()
        {
            if (TokenStore.GetToken() is null)
            {
                _redirectToLogin = true;
                return;
            }

            var result = await AccountController.GetProfileAsync();
            Profile = result.Succeeded ? result.Data : null;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (_redirectToLogin)
            {
                Navigation.NavigateTo("/login", replace: true);
            }
        }

        protected void ToggleAccountMenu() => AccountMenuOpen = !AccountMenuOpen;

        protected void CloseAccountMenu() => AccountMenuOpen = false;

        protected void OnMenuAction(MenuActionItem item)
        {
            if (item.Key == "logout")
            {
                Logout();
            }
        }

        protected void Logout()
        {
            TokenStore.Clear();
            AccountMenuOpen = false;
            Navigation.NavigateTo("/");
        }
    }
}
