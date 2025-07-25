using ECS_Logistics.Data;
using ECS_Logistics.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ECS_Logistics.Repositories;

public class OrderTrackingRepository(IMongoCollection<OrderTracking> orderTracking) : IOrderTrackingRepository
{
    private readonly IMongoCollection<OrderTracking> _orderTracking = orderTracking;

    public OrderTrackingRepository(IMongoDatabase database, IMongoCollection<OrderTracking> orderTracking) : this(orderTracking)
    {
        _orderTracking = database.GetCollection<OrderTracking>("OrderTracking");
    }
    
    public async Task<IEnumerable<OrderTracking>> GetAllByAgentIdAsync(int agentId)
    {
        return await _orderTracking.Find(ot => ot.DeliveryAgentId == agentId).ToListAsync();
    }

    public async Task<OrderTracking?> GetByIdAsync(ObjectId orderTrackingId)
    {
        return await _orderTracking.Find(ot => ot.OrderTrackingId == orderTrackingId).FirstOrDefaultAsync();
    }

    public async Task<OrderTracking> CreateAsync(OrderTracking orderTracking)
    {
        await _orderTracking.InsertOneAsync(orderTracking);
        return orderTracking;
    }

    public async Task<OrderTracking?> UpdateAsync(OrderTracking orderTracking)
    {
        var result = await _orderTracking.ReplaceOneAsync(
            t => t.OrderTrackingId == orderTracking.OrderTrackingId,
            orderTracking,
            new ReplaceOptions { IsUpsert = false });

        return result.ModifiedCount > 0 ? orderTracking : null;
    }
}