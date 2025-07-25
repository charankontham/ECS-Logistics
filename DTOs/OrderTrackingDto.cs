namespace ECS_Logistics.DTOs;

public class OrderTrackingDto
{
    public string? OrderTrackingId { get; set; }
    public int OrderItemId { get; set; }
    public int? DeliveryAgentId { get; set; }
    public int? NearestHubId { get; set; }
    public int OrderTrackingStatusId { get; set; }
    public DateTime EstimatedDeliveryTime { get; set; }
    public DateTime? ActualDeliveryTime { get; set; }
    public int CustomerAddressId { get; set; }
    public string? CustomerInstructions { get; set; }
    public required int OrderTrackingType { get; set; }
}