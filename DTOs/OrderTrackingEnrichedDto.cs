namespace ECS_Logistics.DTOs;

public class OrderTrackingEnrichedDto
{
    public required string OrderTrackingId { get; set; }
    public required ProductFinalDto OrderItem { get; set; }
    public DeliveryAgentDto? DeliveryAgent { get; set; }
    public DeliveryHubDto? NearestHub { get; set; }
    public required int OrderTrackingStatusId { get; set; }
    public DateTime EstimatedDeliveryTime { get; set; }
    public DateTime? ActualDeliveryTime { get; set; }
    public required AddressDto CustomerAddress { get; set; }
    public string? CustomerInstructions { get; set; }
    public required int OrderTrackingType { get; set; }
}