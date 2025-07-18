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

    [Required]
    [Column("delivery_hub_address_id")]
    public required int DeliveryHubAddressId { get; set; }
}