using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECS_Logistics.Models;

public class OrderReturn
{
    [Key]
    [Column("order_return_id")]
    public int OrderReturnId { get; set; }

    [Required]
    [Column("order_id")]
    public required int OrderId { get; set; }

    [Required]
    [Column("order_tracking_id")]
    public required int OrderTrackingId { get; set; }

    [Required]
    [Column("delivery_agent_id")]
    public required int DeliveryAgentId { get; set; }

    [Required]
    [Column("order_item_id")]
    public required int OrderItemId { get; set; }
    
    [Required]
    [Column("return_reason_category_id")]
    public required int ReturnReasonCategoryId { get; set; }
    
    [Column("return_reason")]
    [StringLength(500)]
    public string? ReturnReason { get; set; }

    [Required]
    [Column("date_added")]
    public required DateTime DateAdded { get; set; }

    [Required]
    [Column("date_modified")]
    public required DateTime DateModified { get; set; }

    [Required]
    [Column("return_payment_source_id")]
    public required int ReturnPaymentSourceId { get; set; }
}