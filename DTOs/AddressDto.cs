namespace ECS_Logistics.DTOs;

public class AddressDto
{
    public int? AddressId { get; set; }
    public string? UserId { get; set; }
    public string? Name { get; set; }
    public string? Contact { get; set; }
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Zip { get; set; }
    public string? Country { get; set; }
}