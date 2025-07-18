using ECS_Logistics.Filters;
using ECS_Logistics.Models;

namespace ECS_Logistics.Repositories;

public interface IDeliveryAgentRepository
{
    Task<IEnumerable<DeliveryAgent>> GetAllAsync(DeliveryAgentFilters filters);
    Task<DeliveryAgent?> GetByIdAsync(int id);
    Task<DeliveryAgent> CreateAsync(DeliveryAgent agent);
    Task<DeliveryAgent> UpdateAsync(DeliveryAgent agent);
    Task<bool> DeleteAsync(int id);
}