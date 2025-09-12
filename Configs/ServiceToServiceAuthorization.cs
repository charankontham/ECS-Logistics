using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ECS_Logistics.Configs;

public class ServiceToServiceAuthorization
{
    private readonly SymmetricSecurityKey _signingKey;
    private readonly string _serviceClientId;
    private readonly Lock _lock = new Lock();

    private string? _cachedToken;
    private long _expiryMillis = 0L;
    private readonly long _serviceTtlMillis;

    public ServiceToServiceAuthorization(IConfiguration configuration)
    {
        string? secret = configuration["Jwt:Key"];
        var keyBytes = Convert.FromBase64String(secret ?? throw new InvalidOperationException("JWT Key not configured"));
        _signingKey = new SymmetricSecurityKey(keyBytes);
        _serviceClientId = configuration["ServiceClientId"] ?? string.Empty;
        _serviceTtlMillis = long.Parse(configuration["ServiceTokenTTL"] ?? "");
    }
    
    public string GetToken()
    {
        lock (_lock)
        {
            long now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            if (!string.IsNullOrEmpty(_cachedToken) && now < _expiryMillis - 500)
            {
                return _cachedToken;
            }
            
            DateTime expires = DateTime.UtcNow.AddSeconds(_serviceTtlMillis);

            var claims = new List<Claim>
            {
                new Claim("type", "service"),
                new Claim(JwtRegisteredClaimNames.Sub, _serviceClientId),
                new Claim(JwtRegisteredClaimNames.Iat, 
                    new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(), 
                    ClaimValueTypes.Integer64)
            };

            var creds = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha512);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            _cachedToken = new JwtSecurityTokenHandler().WriteToken(token);
            _expiryMillis = new DateTimeOffset(expires).ToUnixTimeMilliseconds();

            return _cachedToken;
        }
    }

    public void Invalidate()
    {
        lock (_lock)
        {
            _cachedToken = null;
            _expiryMillis = 0L;
        }
    }
}