using ECS_Logistics.DTOs;
using ECS_Logistics.Filters;

namespace ECS_Logistics.Services;

public interface IDeliveryAgentService
{
    Task<IEnumerable<DeliveryAgentDto>> GetAllAgentsAsync(DeliveryAgentFilters? filters);
    Task<PagedResult<DeliveryAgentDto>> GetAllByPaginationAsync(int currentPage, int offset, DeliveryAgentFilters? filters);
    Task<object> GetAgentByIdAsync(int id);
    Task<object> CreateAgentAsync(DeliveryAgentDto agentDto);
    Task<object> UpdateAgentAsync(DeliveryAgentDto agentDto);
    Task<bool> DeleteAgentAsync(int id);
}