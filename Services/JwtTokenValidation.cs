using System.IdentityModel.Tokens.Jwt;
using System.Net;
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
        if (jwtToken.ValidTo < DateTime.UtcNow)
        {
            logger.LogWarning("Jwt token expired!");
            return StatusCodesEnum.AuthenticationFailed;
        }
        if (string.IsNullOrEmpty(email))
        {
            logger.LogWarning("JWT token missing 'sub' claim");
            return StatusCodesEnum.InvalidEmail;
        }
        try
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var customerResponse = await httpClient.GetAsync($"{ServiceUrls.CustomerService}/getByEmail/{email}");
            if (customerResponse.IsSuccessStatusCode)
            {
                var customer = await customerResponse.Content.ReadFromJsonAsync<CustomerDto>();
                if (customer != null && customer.Email.Equals(email))
                {
                    logger.LogInformation("Successfully validated User: {Email}", email);
                    return customer;
                }
                else
                {
                    logger.LogWarning("Not an authorized user role!");
                    return StatusCodesEnum.AuthenticationFailed;
                }
            }
            else if (customerResponse.StatusCode == HttpStatusCode.NotFound)
            {
                var adminResponse = await httpClient.GetAsync($"{ServiceUrls.AdminService}/getByUsername/{email}");
                if (adminResponse.IsSuccessStatusCode)
                {
                    var admin = await adminResponse.Content.ReadFromJsonAsync<AdminDataDto>();
                    if (admin != null && admin.AdminUsername.Equals(email))
                    {
                        logger.LogInformation("Successfully validated user: {Email}, role: ROLE_{SubRole}_{RoleName}",
                            email, admin.AdminRole.SubRole.ToUpper(), admin.AdminRole.RoleName.ToUpper());
                        return admin;
                    }
                    else
                    {
                        logger.LogWarning("Not an authorized admin role!");
                        return StatusCodesEnum.AuthenticationFailed;
                    }
                }
                else
                {
                    logger.LogWarning("Admin service returned {AdminResponseStatusCode} for email: {Email}", 
                        adminResponse.StatusCode, email);
                    return StatusCodesEnum.AuthenticationFailed;
                }
            }
            else
            {
                logger.LogWarning("Customer service returned {CustomerResponseStatusCode} for email: {Email}", 
                    customerResponse.StatusCode, email);
                return StatusCodesEnum.AuthenticationFailed;
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