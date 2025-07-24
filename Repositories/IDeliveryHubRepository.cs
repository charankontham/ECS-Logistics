using ECS_Logistics.Filters;
using ECS_Logistics.Models;

namespace ECS_Logistics.Repositories;

public interface IDeliveryHubRepository
{
    Task<IEnumerable<DeliveryHub>> GetAllAsync(DeliveryHubFilters? filters);
    Task<DeliveryHub?> GetByIdAsync(int id);
    Task<DeliveryHub> CreateAsync(DeliveryHub agent);
    Task<DeliveryHub> UpdateAsync(DeliveryHub agent);
    Task<bool> DeleteAsync(int id);
}