using Auth.UI.src.Manager.Security;
using Auth.UI.src.Manager.Service.Implementation;
using Auth.UI.src.Manager.Service.Interface;
using Auth.UI.src.Shared.Http;
using Auth.UI.src.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthorizationCore(options =>
{
    // Require authentication for every route by default. Public pages
    // (sign-in, registration, password recovery, showcase) opt out with [AllowAnonymous].
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.AddScoped<AuthenticationStateProvider, TokenStoreAuthenticationStateProvider>();
builder.Services.AddScoped<TokenStoreAuthenticationStateProvider>();

builder.Services.AddScoped(sp =>
{
    var baseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5000";
    var handler = new HttpClientHandler { ServerCertificateCustomValidationCallback = (_, _, _, _) => true };
    return new HttpClient(handler) { BaseAddress = new Uri(baseUrl) };
});

builder.Services.AddScoped<IHttpService, HttpService>();
builder.Services.AddScoped(typeof(GenericHttpRepository<>));
builder.Services.AddScoped<ITokenStore, TokenStore>();
builder.Services.AddScoped<IAuthManager, AuthManager>();
builder.Services.AddScoped<IAccountManager, AccountManager>();
builder.Services.AddScoped<ISecurityManager, SecurityManager>();
builder.Services.AddScoped<Auth.UI.Components.UI.Toaster.ToasterService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<Auth.UI.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
