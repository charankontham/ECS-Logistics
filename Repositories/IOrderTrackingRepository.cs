using ECS_Logistics.DTOs;
using ECS_Logistics.Models;
using MongoDB.Bson;

namespace ECS_Logistics.Repositories;

public interface IOrderTrackingRepository
{
    Task<OrderTracking?> GetByIdAsync(ObjectId orderTrackingId);
    Task<IEnumerable<OrderTracking>> GetAllByAgentIdAsync(int agentId);
    Task<IEnumerable<OrderTracking>> GetAllByOrderItemIdAsync(int orderItemId);
    Task<OrderTracking> CreateAsync(OrderTracking orderTracking);
    Task<OrderTracking?> UpdateAsync(OrderTracking orderTracking);
}