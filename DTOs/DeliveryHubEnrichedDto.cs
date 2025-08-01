namespace ECS_Logistics.DTOs;

public class DeliveryHubEnrichedDto
{
    public int DeliveryHubId { get; set; }
    public required string DeliveryHubName { get; set; }
    public AddressDto? DeliveryHubAddress { get; set; }
    public required DateTimeOffset DateAdded { get; set; }
    public required DateTimeOffset DateModified { get; set; }
}