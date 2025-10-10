using ECS_Logistics.DTOs;
using ECS_Logistics.Filters;
using ECS_Logistics.Models;

namespace ECS_Logistics.Repositories;

public interface IOrderReturnRepository
{
    Task<IEnumerable<OrderReturn>> GetAllAsync(OrderReturnFilters? filters);
    Task<PagedResult<OrderReturn>> GetAllByPaginationAsync(int currentPage, int offset, OrderReturnFilters? filters);
    Task<OrderReturn?> GetByIdAsync(int id);
    Task<IEnumerable<OrderReturn>> GetAllByCustomerIdAsync(int customerId);
    Task<OrderReturn> CreateAsync(OrderReturn orderReturn);
    Task<bool> DeleteAsync(int id);
}