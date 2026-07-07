using Auth.UI.src.Manager.Controller;
using Auth.UI.src.Manager.Service.Implementation;
using Auth.UI.src.Manager.Service.Interface;
using Auth.UI.src.Shared.Http;
using Auth.UI.src.Utility;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped(sp =>
{
    var baseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "https://didactic-goggles-px4974rp7jg2v7-5001.app.github.dev/";
    var handler = new HttpClientHandler { ServerCertificateCustomValidationCallback = (_, _, _, _) => true };
    return new HttpClient(handler) { BaseAddress = new Uri(baseUrl) };
});

builder.Services.AddScoped<IHttpService, HttpService>();
builder.Services.AddScoped(typeof(GenericHttpRepository<>));
builder.Services.AddScoped<ITokenStore, TokenStore>();
builder.Services.AddScoped<IAuthManager, AuthManager>();
builder.Services.AddScoped<AuthController>();
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
