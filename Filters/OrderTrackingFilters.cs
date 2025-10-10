namespace ECS_Logistics.Filters;

public class OrderTrackingFilters
{
    public int? DeliveryAgentId { get; set; }
    public DateTime? EstimatedDeliveryDate { get; set; }
    public int? OrderTrackingStatusId { get; set; }
    public int? OrderTrackingType { get; set; }
    // public string? SearchValue { get; set; }
}