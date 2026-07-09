using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using PasswordlessApi.Api.Configuration;
using PasswordlessApi.Api.Middleware;
using PasswordlessApi.Api.Service.Implementation.Auth;
using PasswordlessApi.Api.Service.Implementation.Repository;
using PasswordlessApi.Api.Service.Interface.Auth;
using PasswordlessApi.Api.Service.Interface.Repository;
using PasswordlessApi.Api.Utility.Jwt;
using PasswordlessApi.Api.Utility.PasswordHash;

var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddScoped<PasswordlessApi.Api.Service.Interface.Security.ILocationResolver, PasswordlessApi.Api.Service.Implementation.Security.IpApiLocationResolver>();
builder.Services.AddScoped<PasswordlessApi.Api.Service.Interface.Security.IAuditLogService, PasswordlessApi.Api.Service.Implementation.Security.AuditLogService>();
builder.Services.AddScoped<PasswordlessApi.Api.Service.Interface.Security.IRateLimiter, PasswordlessApi.Api.Service.Implementation.Security.InMemoryRateLimiter>();
builder.Services.AddHttpContextAccessor();
builder.Services.Configure<Fido2Settings>(builder.Configuration.GetSection("Fido2Settings"));
builder.Services.Configure<SecuritySettings>(builder.Configuration.GetSection("SecuritySettings"));
builder.Services.Configure<CorsSettings>(builder.Configuration.GetSection("CorsSettings"));

var corsSettings = builder.Configuration.GetSection("CorsSettings").Get<CorsSettings>() ?? new CorsSettings();
var allowedOrigins = corsSettings.AllowedOrigins.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCorsPolicy", policy =>
    {
        if (allowedOrigins.Length > 0 && !(allowedOrigins.Length == 1 && allowedOrigins[0] == "*"))
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        }
        else if (allowedOrigins.Length == 1 && allowedOrigins[0] == "*")
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
        else
        {
            policy.WithOrigins("https://localhost:5001", "http://localhost:5000")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
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
app.UseMiddleware<PasswordlessApi.Api.Middleware.SecurityHeadersMiddleware>();
app.UseRateLimiter();
app.MapControllers();

app.Run();