using ECS_Logistics.DTOs;
using ECS_Logistics.Filters;
using ECS_Logistics.Models;
using MongoDB.Bson;

namespace ECS_Logistics.Services;

public interface IOrderTrackingService
{
    Task<object> GetAllByAgentIdAsync(int agentId);
    Task<object> GetAllByPaginationAsync(int currentPage, int offset, OrderTrackingFilters? filters);
    Task<object> GetByIdAsync(string orderTrackingId);
    Task<object> GetByOrderIdAndProductIdAsync(int orderId, int productId);
    Task<object> CreateAsync(OrderTrackingDto orderTrackingDto);
    Task<object> UpdateAsync(OrderTrackingDto orderTrackingDto);
    Task<object> UpdateStatusAsync(string orderTrackingId, int statusId);
    
}