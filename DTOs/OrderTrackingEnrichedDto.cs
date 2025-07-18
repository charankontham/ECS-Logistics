namespace ECS_Logistics.DTOs;

public class OrderTrackingEnrichedDto
{
    public required string OrderTrackingId { get; set; }
    public required ProductFinalDto OrderItem { get; set; }
    public DeliveryAgentDto? DeliveryAgent { get; set; }
    public DeliveryHubDto? NearestHub { get; set; }
    public int OrderTrackingStatus { get; set; }
    public DateTime EstimatedDeliveryTime { get; set; }
    public DateTime? ActualDeliveryTime { get; set; }
    public required AddressDto DeliveryAddress { get; set; }
    public string? DeliveryInstructions { get; set; }
}