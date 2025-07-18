using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECS_Logistics.Models;

public class DeliveryAgent
{
    [Key]
    [Column("delivery_agent_id")]
    public int DeliveryAgentId { get; set; }

    [Required]
    [Column("delivery_agent_name")]
    [StringLength(100)]
    public required string DeliveryAgentName { get; set; }

    [Required]
    [Column("contact_number")]
    [Phone]
    [StringLength(15)]
    public required string ContactNumber { get; set; }

    [Required]
    [Column("email")]
    [EmailAddress]
    [StringLength(255)]
    public required string Email { get; set; }

    [Required]
    [Column("password")]
    [PasswordPropertyText]
    [StringLength(100)]
    public required string Password { get; set; }

    [Required]
    [Column("availability_status")]
    public required int AvailabilityStatus { get; set; }

    [Column("rating")]
    public float? Rating { get; set; }

    [Required]
    [Column("total_deliveries")]
    public required int TotalDeliveries { get; set; }

    [Required]
    [Column("serving_area")]
    [StringLength(255)]
    public required string ServingArea { get; set; }

    [Required]
    [Column("date_added")]
    public required DateTime DateAdded { get; set; }

    [Required]
    [Column("date_modified")]
    public required DateTime DateModified { get; set; }
}