using ECS_Logistics.DTOs;
using ECS_Logistics.Filters;

namespace ECS_Logistics.Services;

public interface IOrderReturnService
{
    Task<IEnumerable<OrderReturnEnrichedDto>> GetAllReturnsAsync(OrderReturnFilters? filters);
    Task<object> GetOrderReturnByIdAsync(int id);
    Task<object> GetAllByCustomerIdAsync(int customerId);
    Task<object> CreateReturnAsync(OrderReturnDto orderReturnDto);
    Task<bool> DeleteOrderReturnAsync(int orderReturnId);
    
}