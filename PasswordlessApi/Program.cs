using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using System.Text.Json;
using API.Shared.Utility.Jwt;
using API.Shared.Utility.PasswordHash;
using API.Shared.Middleware;
using API.Shared.Configuration;
using API.Shared.Authorization;
using API.Shared.Service.Implementation.Auth;
using API.Shared.Service.Implementation.Rbac;
using API.Shared.Service.Implementation.Security;
using API.Shared.Service.Interface.Auth;
using API.Shared.Service.Interface.Rbac;
using API.Shared.Service.Interface.Repository;
using API.Shared.Service.Interface.Security;
using PasswordlessApi.Api.Service.Implementation.Repository;

var builder = WebApplication.CreateBuilder(args);

var configuredUrls = builder.Configuration["ASPNETCORE_URLS"] ?? builder.Configuration["Urls"];
if (string.IsNullOrWhiteSpace(configuredUrls))
{
    builder.WebHost.UseUrls("http://localhost:5000");
}
else
{
    builder.WebHost.UseUrls(configuredUrls);
}

var jwtSecret = builder.Configuration["JwtSettings:SecretKey"] ?? Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
if (string.IsNullOrWhiteSpace(jwtSecret) || jwtSecret is "fake_jwt_token" or "fake_local_key")
{
    if (builder.Environment.IsProduction())
    {
        throw new InvalidOperationException("JWT signing secret is not configured. Set JwtSettings:SecretKey or JWT_SECRET_KEY.");
    }

    jwtSecret = Guid.NewGuid().ToString("N");
}

builder.Configuration["JwtSettings:SecretKey"] = jwtSecret;

var key = Encoding.UTF8.GetBytes(jwtSecret);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ManageUsers", policy => policy.Requirements.Add(new PermissionRequirement("users.write")));
    options.AddPolicy("ManageRoles", policy => policy.Requirements.Add(new PermissionRequirement("roles.write")));
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"] ?? "PasswordlessApi",
            ValidateAudience = true,
            ValidAudience = builder.Configuration["JwtSettings:Audience"] ?? "PasswordlessApiUsers",
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection(ApiSettings.SectionName));
builder.Services.Configure<SecuritySettings>(builder.Configuration.GetSection(nameof(SecuritySettings)));

builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCorsPolicy", policy =>
    {
        var apiSettings = builder.Configuration.GetSection(ApiSettings.SectionName).Get<ApiSettings>();
        if (apiSettings == null || !apiSettings.AllowedOrigins.Any())
        {
            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
            return;
        }

        if (apiSettings.IsWildcardOrigin())
        {
            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
        }
        else
        {
            policy.WithOrigins(apiSettings.GetAllowedOrigins())
                  .AllowAnyHeader()
                  .AllowAnyMethod();

            if (apiSettings.AllowCredentials)
            {
                policy.AllowCredentials();
            }
        }
    });
});

builder.Services.AddHttpClient<ILocationResolver, IpApiLocationResolver>();
builder.Services.AddScoped<DapperContext>();
builder.Services.AddScoped<IDapperRepository, DapperRepository>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IPasswordHash, PasswordHash>();
builder.Services.AddScoped<IJwtHelper, JwtHelper>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IFido2Service, Fido2Service>();
builder.Services.AddScoped<IOtpService, OtpService>();
builder.Services.AddScoped<IUserCredentialService, UserCredentialService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IUserRoleService, UserRoleService>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();
builder.Services.AddScoped<IEmailService, LoggingEmailService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
builder.Services.Configure<RateLimitSettings>(builder.Configuration.GetSection(RateLimitSettings.SectionName));
var rateLimitSettings = builder.Configuration.GetSection(RateLimitSettings.SectionName).Get<RateLimitSettings>() ?? new RateLimitSettings();
builder.Services.AddSecurityRateLimiting(rateLimitSettings);

var app = builder.Build();

app.UseExceptionHandler(appError =>
{
    appError.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = "An internal server error occurred." }));
    });
});

// Update RemoteIpAddress / scheme from the proxy so rate limiting and audit
// logs capture the real client, not the proxy itself.
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseMiddleware<SecurityHeadersMiddleware>();
    app.UseHttpsRedirection();
    app.UseHsts();
}

app.UseRateLimiter();

app.UseCors("DefaultCorsPolicy");
app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var permissionService = scope.ServiceProvider.GetRequiredService<IPermissionService>();
    await permissionService.SeedDefaultPermissionsAsync();
}

app.MapControllers();

app.Run();
