namespace ECS_Logistics.DTOs;

public class OrderReturnDto
{
    public int OrderReturnId { get; set; }
    public int OrderId { get; set; }
    public int OrderTrackingId { get; set; }
    public int DeliveryAgentId { get; set; }
    public int OrderItemId { get; set; }
    public int ReturnReasonCategoryId { get; set; }
    public string? ReturnReason { get; set; }
    public DateTime DateAdded { get; set; }
    public DateTime DateModified { get; set; }
    public int ReturnPaymentSourceId { get; set; }
}