using ECS_Logistics.DTOs;
using ECS_Logistics.Filters;

namespace ECS_Logistics.Services;

public interface IOrderReturnService
{
    Task<IEnumerable<OrderReturnEnrichedDto>> GetAllAsync(OrderReturnFilters? filters);
    Task<object> GetByIdAsync(int id);
    Task<IEnumerable<OrderTrackingEnrichedDto>> GetAllByCustomerIdAsync(int customerId);
    Task<OrderTrackingEnrichedDto> CreateAsync(OrderTrackingDto orderTrackingDto);
    Task<bool> DeleteOrderReturnAsync(int id);
    
}