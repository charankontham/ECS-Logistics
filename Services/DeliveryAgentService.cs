using AutoMapper;
using ECS_Logistics.Data;
using ECS_Logistics.DTOs;
using ECS_Logistics.Filters;
using ECS_Logistics.Models;
using ECS_Logistics.Repositories;
using ECS_Logistics.Utils;

namespace ECS_Logistics.Services;

public class DeliveryAgentService(IDeliveryAgentRepository repository, IMapper mapper, MySqlDbContext context): IDeliveryAgentService
{
    public async Task<IEnumerable<DeliveryAgentDto>> GetAllAgentsAsync(DeliveryAgentFilters? filters)
        {
            var agents = await repository.GetAllAsync(filters);
            return mapper.Map<IEnumerable<DeliveryAgentDto>>(agents);
        }

        public async Task<object> GetAgentByIdAsync(int id)
        {
            var agent = await repository.GetByIdAsync(id);
            if (agent == null) return StatusCodesEnum.DeliveryAgentNotFound;
            return mapper.Map<DeliveryAgentDto>(agent);
        }

        public async Task<object> Login(string email, string password)
        {
            if (email.Trim() == "")
            {
                return StatusCodesEnum.InvalidEmail;
            }
            else if(password.Trim() == "")
            {
                return StatusCodesEnum.InvalidPassword;
            }
            else
            {
                var agent = await repository.GetByEmailAsync(email);
                if (agent == null) return StatusCodesEnum.DeliveryAgentNotFound;
                if (PasswordHasher.VerifyPassword(password, agent.Password))
                {
                    return mapper.Map<DeliveryAgentDto>(agent);
                }
                else
                {
                    return StatusCodesEnum.InvalidPassword;
                }
            }
        }

        public async Task<object> CreateAgentAsync(DeliveryAgentDto agentDto)
        {
            if (context.DeliveryAgents.Any(x => x.Email == agentDto.Email))
            {
                return StatusCodesEnum.EmailAlreadyExists;
            }
            agentDto.DateAdded = DateTime.Now;
            agentDto.DateModified = DateTime.Now;
            var agent = mapper.Map<DeliveryAgent>(agentDto);
            var createdAgent = await repository.CreateAsync(agent);
            return mapper.Map<DeliveryAgentDto>(createdAgent);
        }

        public async Task<object> UpdateAgentAsync(DeliveryAgentDto agentDto)
        {
            if (agentDto.DeliveryAgentId == null)
            {
                return StatusCodesEnum.DeliveryAgentNotFound;
            }
            agentDto.DateModified = DateTime.Now;
            var updatedAgent =  await repository.UpdateAsync(mapper.Map<DeliveryAgent>(agentDto));
            return mapper.Map<DeliveryAgentDto>(updatedAgent);
        }

        public async Task<bool> DeleteAgentAsync(int id)
        {
            return await repository.DeleteAsync(id);
        }
}