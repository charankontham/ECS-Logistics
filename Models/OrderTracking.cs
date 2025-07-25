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
    [BsonElement("order_tracking_status_id")]
    public required int OrderTrackingStatusId { get; set; }

    [BsonElement("estimated_delivery_time")]
    public DateTime? EstimatedDeliveryTime { get; set; }

    [BsonElement("actual_delivery_time")]
    public DateTime? ActualDeliveryTime { get; set; }
    
    [BsonRequired]
    [BsonElement("customer_address_id")]
    public int CustomerAddressId { get; set; }

    [BsonElement("customer_instructions")]
    public string? CustomerInstructions { get; set; }
    
    [BsonRequired]
    [BsonElement("order_tracking_type")]
    public required int OrderTrackingType { get; set; }
}