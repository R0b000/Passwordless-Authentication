using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using PasswordlessApi.Api.Configuration;
using PasswordlessApi.Api.Middleware;
using PasswordlessApi.Api.Service.Implementation.Auth;
using PasswordlessApi.Api.Service.Implementation.Repository;
using PasswordlessApi.Api.Service.Implementation.Rbac;
using PasswordlessApi.Api.Service.Interface.Auth;
using PasswordlessApi.Api.Service.Interface.Repository;
using PasswordlessApi.Api.Service.Interface.Rbac;
using PasswordlessApi.Api.Utility.Jwt;
using PasswordlessApi.Api.Utility.PasswordHash;
using PasswordlessApi.Api.Authorization;
using Microsoft.AspNetCore.Authorization;

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

var key = Encoding.UTF8.GetBytes(jwtSecret);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddAuthorization();
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

builder.Services.AddHttpClient<PasswordlessApi.Api.Service.Implementation.Security.IpApiLocationResolver>();

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
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ManageRoles", policy => policy.Requirements.Add(new PermissionRequirement("roles.write")));
    options.AddPolicy("ReadRoles", policy => policy.Requirements.Add(new PermissionRequirement("roles.read")));
    options.AddPolicy("ManageUsers", policy => policy.Requirements.Add(new PermissionRequirement("users.write")));
    options.AddPolicy("ReadUsers", policy => policy.Requirements.Add(new PermissionRequirement("users.read")));
});

            if (apiSettings.AllowCredentials)
            {
                builderPolicy.AllowCredentials();
            }
        }
    });
});

builder.Services.AddSecurityRateLimiting();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
    app.UseHsts();
}

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