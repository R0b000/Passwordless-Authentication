using global::Shared.UI.Components.Toaster;
using global::Auth.Model.Token;
using global::Auth.API.Utility.Auth;
using global::Shared.UI.Http;
using global::Auth.UI.Manager.Implementation.Auth;
using global::Auth.UI.Manager.Interface.Auth;
using global::Auth.UI.Utility;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient("ApiGateway", client =>
{
    var baseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5000";
    client.BaseAddress = new Uri(baseUrl);
});


// 3. Register your services
builder.Services.AddScoped<ITokenHelper, TokenHelper>();
builder.Services.AddScoped<ITokenStore, ProtectedSessionTokenStore>();
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

app.MapRazorComponents<UI.Components.App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(Auth.UI.Components.Layout.MainLayout).Assembly);

app.Run();
