using AutoMapper;
using ECS_Logistics.DTOs;
using ECS_Logistics.Filters;
using ECS_Logistics.Models;
using ECS_Logistics.Repositories;

namespace ECS_Logistics.Services;

public class DeliveryAgentService(IDeliveryAgentRepository repository, IMapper mapper): IDeliveryAgentService
{
    public async Task<IEnumerable<DeliveryAgentDto>> GetAllAgentsAsync(DeliveryAgentFilters filters)
        {
            var agents = await repository.GetAllAsync(filters);
            return mapper.Map<IEnumerable<DeliveryAgentDto>>(agents);
        }

        public async Task<DeliveryAgentDto> GetAgentByIdAsync(int id)
        {
            var agent = await repository.GetByIdAsync(id);
            if (agent == null) throw new Exception("Agent not found!");
            return mapper.Map<DeliveryAgentDto>(agent);
        }

        public async Task<DeliveryAgentDto> CreateAgentAsync(DeliveryAgentDto dto)
        {
            var agent = mapper.Map<DeliveryAgent>(dto);
            agent.DateAdded = DateTime.Now;
            agent.DateModified = DateTime.Now;
            var createdAgent = await repository.CreateAsync(agent);
            return mapper.Map<DeliveryAgentDto>(createdAgent);
        }

        public async Task<DeliveryAgentDto> UpdateAgentAsync(int id, DeliveryAgentDto dto)
        {
            var agent = await repository.GetByIdAsync(id);
            if (agent == null) throw new Exception("Agent not found!");
            mapper.Map(dto, agent);
            agent.DateModified = DateTime.Now;
            var updatedAgent =  await repository.UpdateAsync(agent);
            return mapper.Map<DeliveryAgentDto>(updatedAgent);
        }

        public async Task<bool> DeleteAgentAsync(int id)
        {
            var agent = await repository.GetByIdAsync(id);
            if (agent == null) throw new Exception("Agent not found!");
            return await repository.DeleteAsync(id);
        }
}