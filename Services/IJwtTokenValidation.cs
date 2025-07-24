namespace ECS_Logistics.Services;

public interface IJwtTokenValidation
{
    Task<object> ValidateTokenAsync(string token);
}