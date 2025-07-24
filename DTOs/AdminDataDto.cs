namespace ECS_Logistics.DTOs;

public class AdminDataDto
{
    public required string Id { get; set; }
    public required string AdminUsername { get; set; }
    public string? AdminName { get; set; }
    public string AdminPassword { get; set; }
    public required Role AdminRole { get; set; }
    
}