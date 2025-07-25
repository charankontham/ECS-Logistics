using ECS_Logistics.DTOs;
using ECS_Logistics.Models;
using MongoDB.Bson;

namespace ECS_Logistics.Services;

public interface IOrderTrackingService
{
    Task<object> GetAllByAgentIdAsync(int agentId);
    Task<object> GetByIdAsync(string orderTrackingId);
    Task<object> CreateAsync(OrderTrackingDto orderTrackingDto);
    Task<object> UpdateAsync(OrderTrackingDto orderTrackingDto);
    
}