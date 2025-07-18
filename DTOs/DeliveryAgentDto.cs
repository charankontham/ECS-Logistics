namespace ECS_Logistics.DTOs;

public class DeliveryAgentDto
{
    public int DeliveryAgentId { get; set; }
    public required string DeliveryAgentName { get; set; }
    public required string ContactNumber { get; set; }
    public required string Email { get; set; }
    public string? Password { get; set; }
    public int AvailabilityStatus { get; set; }
    public float? Rating { get; set; }
    public int TotalDeliveries { get; set; }
    public required string ServingArea { get; set; }
    public DateTime DateAdded { get; set; }
    public DateTime DateModified { get; set; }
}