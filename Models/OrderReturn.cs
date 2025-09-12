using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECS_Logistics.Models;

public class OrderReturn
{
    [Key]
    [Column("order_return_id")]
    public int OrderReturnId { get; set; }
    
    [Required]
    [Column("order_item_id")]
    public int OrderItemId { get; set; }
    
    [Required]
    [Column("product_quantity")]
    public int ProductQuantity { get; set; }
    
    [Required]
    [Column("customer_id")]
    public required int CustomerId { get; set; }

    [Required]
    [Column("order_tracking_id")]
    [StringLength(50)]
    public required string OrderTrackingId { get; set; }
    
    [Column("product_id")]
    public int? ProductId { get; set; }
    
    [Column("category_id")]
    public int? CategoryId { get; set; }
    
    [Column("subcategory_id")]
    public int? SubCategoryId { get; set; }
    
    [Column("brand_id")]
    public int? BrandId { get; set; }
    
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