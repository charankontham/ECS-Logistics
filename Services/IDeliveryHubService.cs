using ECS_Logistics.DTOs;
using ECS_Logistics.Filters;
using ECS_Logistics.Models;

namespace ECS_Logistics.Services;

public interface IDeliveryHubService
{
    Task<IEnumerable<DeliveryHubEnrichedDto>> GetAllHubsAsync(DeliveryHubFilters? filters);
    Task<object> GetHubByIdAsync(int id);
    Task<object> CreateHubAsync(DeliveryHubDto hubDto);
    Task<object> UpdateHubAsync(DeliveryHubDto hubDto);
    Task<bool> DeleteHubAsync(int id);
}