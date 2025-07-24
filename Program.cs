using System.Text;
using ECS_Logistics.Configs;
using ECS_Logistics.Data;
using ECS_Logistics.DTOs;
using ECS_Logistics.FeignClients;
using ECS_Logistics.Mappings;
using ECS_Logistics.Repositories;
using ECS_Logistics.Services;
using ECS_Logistics.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Serilog;
using Steeltoe.Discovery.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddLogging(logging =>
{
    logging.AddSerilog(new LoggerConfiguration().WriteTo.Console().CreateLogger());
});
builder.Services.AddDbContext<MySqlDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("MySql"),
        new MySqlServerVersion(new Version(8, 0, 29))
        ).EnableSensitiveDataLogging()
    );
builder.Services.AddSingleton<IMongoClient>(new MongoClient(
    builder.Configuration.GetConnectionString("MongoDB"))
);
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<FeignClient>();
builder.Services.AddSingleton<CustomerService>();
builder.Services.AddSingleton<AddressResolver>();
builder.Services.AddSingleton<MongoDbContext>();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddDiscoveryClient(builder.Configuration);
builder.Services.AddHttpClient();
builder.Services.AddScoped<IDeliveryAgentRepository, DeliveryAgentRepository>();
builder.Services.AddScoped<IDeliveryAgentService, DeliveryAgentService>();
builder.Services.AddScoped<IDeliveryHubRepository, DeliveryHubRepository>();
builder.Services.AddScoped<IDeliveryHubService, DeliveryHubService>();
builder.Services.AddScoped<IJwtTokenValidation, JwtTokenValidation>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(
                builder.Configuration["Jwt:Key"] ?? 
                throw new InvalidOperationException("JWT Key not configured"))),
            RequireSignedTokens = true,
            ValidAlgorithms = new List<string> { "HS512" }
        };
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var jwtTokenValidation = context.HttpContext.RequestServices.
                    GetRequiredService<IJwtTokenValidation>();
                var authHeader = context.Request.Headers.Authorization.FirstOrDefault();
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer ", 
                        StringComparison.OrdinalIgnoreCase))
                {
                    context.Fail("Invalid or missing Authorization header");
                    return;
                }

                var token = authHeader["Bearer ".Length..].Trim();
                var response = await jwtTokenValidation.ValidateTokenAsync(token);
                if (response is StatusCodesEnum)
                {
                    
                    context.Fail("Authorization Failed!");
                    return;
                }
                AdminDataDto userDetails = (dynamic) response;
                var claims = new List<System.Security.Claims.Claim>
                {
                    new System.Security.Claims.Claim(
                        System.Security.Claims.ClaimTypes.Name, userDetails.AdminUsername),
                    new System.Security.Claims.Claim(
                        System.Security.Claims.ClaimTypes.Role, 
                        "ROLE_" + userDetails.AdminRole.SubRole.ToUpper() + "_" + 
                        userDetails.AdminRole.RoleName.ToUpper())
                };
                var identity = new System.Security.Claims.ClaimsIdentity(
                    claims, JwtBearerDefaults.AuthenticationScheme);
                context.Principal = new System.Security.Claims.ClaimsPrincipal(identity);
            },
            OnAuthenticationFailed = context =>
            {
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                var message = context.Exception switch
                {
                    SecurityTokenExpiredException => "Token has expired",
                    SecurityTokenSignatureKeyNotFoundException => "Token signature key not found",
                    SecurityTokenInvalidSignatureException => "Invalid token signature",
                    SecurityTokenException => $"Token validation failed: {context.Exception.Message}",
                    _ => $"Authentication failed: {context.Exception.Message}"
                };
                context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new
                {
                    Message = message
                }));
                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddAuthorization();
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    IdentityModelEventSource.ShowPII = true;
}
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();