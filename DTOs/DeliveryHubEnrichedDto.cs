namespace ECS_Logistics.DTOs;

public class DeliveryHubEnrichedDto
{
    public int DeliveryHubId { get; set; }
    public required string DeliveryHubName { get; set; }
    public AddressDto? DeliveryHubAddress { get; set; }
    public required DateTime DateAdded { get; set; }
    public required DateTime DateModified { get; set; }
}