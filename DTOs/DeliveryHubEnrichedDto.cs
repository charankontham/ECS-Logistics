namespace ECS_Logistics.DTOs;

public class DeliveryHubEnrichedDto
{
    public int DeliveryHubId { get; set; }
    public required string DeliveryHubName { get; set; }
    public required AddressDto DeliveryHubAddress { get; set; }
}