using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using ECS_Logistics.DTOs;
using ECS_Logistics.Utils;
using static System.Console;

namespace ECS_Logistics.Services;

public class JwtTokenValidation(ILogger<JwtTokenValidation> logger) : IJwtTokenValidation
{
    public async Task<object> ValidateTokenAsync(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        if (!handler.CanReadToken(token))
        {
            logger.LogWarning("Invalid JWT token format");
            return StatusCodesEnum.AuthenticationFailed;
        }
        var jwtToken = handler.ReadJwtToken(token);
        var email = jwtToken.Subject;   
        if (string.IsNullOrEmpty(email))
        {
            logger.LogWarning("JWT token missing 'sub' claim");
            return StatusCodesEnum.InvalidEmail;
        }

        try
        {
            var adminClient = new HttpClient();
            adminClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            WriteLine("Admin client created : " + adminClient.ToString());
            var adminResponse = await adminClient.GetAsync($"{ServiceUrls.AdminService}/getByUsername/{email}");
            WriteLine("Admin service Response : " + adminResponse.Content.ReadAsStringAsync());
            if (adminResponse.IsSuccessStatusCode)
            {
                var admin = await adminResponse.Content.ReadFromJsonAsync<AdminDataDto>();
                // var admin = JsonSerializer.Deserialize<AdminDataDto>(
                //     adminJson,
                //     new JsonSerializerOptions
                //     {
                //         PropertyNameCaseInsensitive = true
                //     });
                if (admin is { AdminRole: { RoleName: "admin", SubRole: "inventory" } })
                {
                    logger.LogInformation("Successfully validated user: {Email}, role: ROLE_{ToUpper}_{S}", email, admin.AdminRole.SubRole.ToUpper(), admin.AdminRole.RoleName.ToUpper());
                    return admin;
                }
                else
                {
                    logger.LogWarning("Not an Authorized Admin role!");
                    return StatusCodesEnum.AuthenticationFailed;
                }
            }
            else
            {
                logger.LogWarning("Admin service returned {AdminResponseStatusCode} for email: {Email}", adminResponse.StatusCode, email);
                return StatusCodesEnum.InvalidEmail;
            }
        }
        catch (HttpRequestException ex)
        {
            logger.LogError("Failed to validate token: {ExMessage}", ex.Message);
            return StatusCodesEnum.AuthenticationFailed;
        }

        return StatusCodesEnum.AuthenticationFailed;
    }
}