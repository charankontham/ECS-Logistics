namespace ECS_Logistics.DTOs;

public class DeliveryHubDto
{
    public int? DeliveryHubId { get; set; }
    public required string DeliveryHubName { get; set; }
    public int? DeliveryHubAddressId { get; set; }
    public DateTimeOffset DateAdded { get; set; }
    public DateTimeOffset DateModified { get; set; }
}