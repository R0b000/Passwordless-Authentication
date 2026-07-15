using Auth.UI.Shared.Components.Toaster;
using Auth.UI.Shared.Http;
using Auth.UI.Shared.Utility;
using Auth.UI.Shared.Model.Toast;
using Auth.UI.Shared.Manager.Implementation;
using Auth.UI.Shared.Manager.Interface; // Add this using statement if not present

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// 1. Register the Named HttpClient "ApiGateway"
builder.Services.AddHttpClient("ApiGateway", client =>
{
    var baseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5000";
    client.BaseAddress = new Uri(baseUrl);
});

// 2. Register ToastService (THIS WAS MISSING)
// This allows HttpServices to inject it successfully
builder.Services.AddScoped<ToastService>();

// 3. Register your other services
builder.Services.AddScoped<ITokenHelper, TokenHelper>();
builder.Services.AddScoped<ITokenStore, TokenStore>();
builder.Services.AddScoped<IHttpServices, HttpServices>();
builder.Services.AddScoped<IAuthManager, AuthManager>();
builder.Services.AddScoped<IAccountManager, AccountManager>();
builder.Services.AddScoped<ISecurityManager, SecurityManager>();

// Note: You might also have a UI ToasterService in Components.UI.Toaster
// If you use that one elsewhere, keep registering it too:
builder.Services.AddScoped<ToasterService>();

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