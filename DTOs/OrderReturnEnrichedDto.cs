namespace ECS_Logistics.DTOs;

public class OrderReturnEnrichedDto
{
    public int OrderReturnId { get; set; }
    public required int OrderItemId { get; set; }
    public required int ProductQuantity { get; set; }
    public required int CustomerId { get; set; }
    public required OrderTrackingEnrichedDto? OrderTracking { get; set; }
    public required int ReturnReasonCategoryId { get; set; }
    public string? ReturnReason { get; set; }
    public DateTime DateAdded { get; set; }
    public DateTime DateModified { get; set; }
    public required int ReturnPaymentSourceId { get; set; }
}