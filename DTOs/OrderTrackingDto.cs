namespace ECS_Logistics.DTOs;

public class OrderTrackingDto
{
    public string? OrderTrackingId { get; set; }
    public int OrderItemId { get; set; }
    public int? DeliveryAgentId { get; set; }
    public int? NearestHubId { get; set; }
    public int OrderTrackingStatus { get; set; }
    public DateTime EstimatedDeliveryTime { get; set; }
    public DateTime? ActualDeliveryTime { get; set; }
    public int DeliveryAddressId { get; set; }
    public string? DeliveryInstructions { get; set; }
}