namespace ECS_Logistics.Utils;

using BCrypt.Net;

public static class PasswordHasher
{
    public static string HashPassword(string password)
    {
        return BCrypt.HashPassword(password, workFactor: 10);
    }
    
    public static bool VerifyPassword(string password, string hashedPassword)
    {
        return BCrypt.Verify(password, hashedPassword);
    }
}
