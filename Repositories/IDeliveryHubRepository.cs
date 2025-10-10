using ECS_Logistics.DTOs;
using ECS_Logistics.Filters;
using ECS_Logistics.Models;

namespace ECS_Logistics.Repositories;

public interface IDeliveryHubRepository
{
    Task<IEnumerable<DeliveryHub>> GetAllAsync(DeliveryHubFilters? filters);
    Task<PagedResult<DeliveryHub>> GetAllByPaginationAsync(int currentPage, int offset, DeliveryHubFilters? filters);
    Task<DeliveryHub?> GetByIdAsync(int id);
    Task<DeliveryHub> CreateAsync(DeliveryHub agent);
    Task<DeliveryHub> UpdateAsync(DeliveryHub agent);
    Task<bool> DeleteAsync(int id);
}