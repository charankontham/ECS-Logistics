using ECS_Logistics.DTOs;
using ECS_Logistics.Filters;

namespace ECS_Logistics.Services;

public interface IDeliveryAgentService
{
    Task<IEnumerable<DeliveryAgentDto>> GetAllAgentsAsync(DeliveryAgentFilters? filters);
    Task<DeliveryAgentDto> GetAgentByIdAsync(int id);
    Task<object> Login(string email, string password);
    Task<object> CreateAgentAsync(DeliveryAgentDto dto);
    Task<object> UpdateAgentAsync(DeliveryAgentDto dto);
    Task<bool> DeleteAgentAsync(int id);
}