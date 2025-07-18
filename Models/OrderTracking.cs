using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ECS_Logistics.Models;

public class OrderTracking
{
    [BsonId]
    [BsonElement("order_tracking_id")]
    public ObjectId OrderTrackingId { get; set; }

    [BsonRequired]
    [BsonElement("order_item_id")]
    public required int OrderItemId { get; set; }
    
    [BsonElement("delivery_agent_id")]
    public int? DeliveryAgentId { get; set; }
    
    [BsonElement("nearest_hub_id")]
    public int? NearestHubId { get; set; }

    [BsonRequired]
    [BsonElement("order_tracking_status")]
    public required int OrderTrackingStatus { get; set; } // Stored as integer for status codes

    [BsonElement("estimated_delivery_time")]
    public DateTime EstimatedDeliveryTime { get; set; }

    [BsonElement("actual_delivery_time")]
    public DateTime? ActualDeliveryTime { get; set; }

    [BsonRequired]
    [BsonElement("delivery_address_id")]
    public int DeliveryAddressId { get; set; }

    [BsonElement("delivery_instructions")]
    public string? DeliveryInstructions { get; set; }
}