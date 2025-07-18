using ECS_Logistics.DTOs;
using ECS_Logistics.Filters;

namespace ECS_Logistics.Services;

public interface IDeliveryAgentService
{
    Task<IEnumerable<DeliveryAgentDto>> GetAllAgentsAsync(DeliveryAgentFilters filters);
    Task<DeliveryAgentDto> GetAgentByIdAsync(int id);
    Task<DeliveryAgentDto> CreateAgentAsync(DeliveryAgentDto dto);
    Task<DeliveryAgentDto> UpdateAgentAsync(int id, DeliveryAgentDto dto);
    Task<bool> DeleteAgentAsync(int id);
}