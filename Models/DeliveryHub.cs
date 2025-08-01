using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECS_Logistics.Models;

public class DeliveryHub
{
    [Key]
    [Column("delivery_hub_id")]
    public required int DeliveryHubId { get; set; }

    [Required]
    [Column("delivery_hub_name")]
    [StringLength(500)]
    public required string DeliveryHubName { get; set; }
    
    [Column("delivery_hub_address_id")]
    public int? DeliveryHubAddressId { get; set; }
    
    [Required]
    [Column("date_added")]
    public required DateTimeOffset DateAdded { get; set; }
    
    [Required]
    [Column("date_modified")]
    public required DateTimeOffset DateModified { get; set; }
}