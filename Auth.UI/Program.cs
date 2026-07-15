using Auth.UI.Shared.Components.Toaster;
using Auth.UI.Shared.Utility;
using Auth.UI.Shared.Model.Toast;
using UI.Shared.Manager.Implementation.Http;
using UI.Shared.Manager.Interface.Http;
using UI.Shared.Manager.Implementation.Auth;
using UI.Shared.Manager.Interface.Auth;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient("ApiGateway", client =>
{
    var baseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5000";
    client.BaseAddress = new Uri(baseUrl);
});


// 3. Register your services
builder.Services.AddScoped<ToastService>();
builder.Services.AddScoped<ITokenHelper, TokenHelper>();
builder.Services.AddScoped<ITokenStore, TokenStore>();
builder.Services.AddScoped<IHttpServices, HttpServices>();
builder.Services.AddScoped<IAuthManager, AuthManager>();
builder.Services.AddScoped<IAccountManager, AccountManager>();
builder.Services.AddScoped<ISecurityManager, SecurityManager>();

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