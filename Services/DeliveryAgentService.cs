using AutoMapper;
using ECS_Logistics.Data;
using ECS_Logistics.DbContexts;
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

    public async Task<PagedResult<DeliveryAgentDto>> GetAllByPaginationAsync(int currentPage, int offset, DeliveryAgentFilters? filters)
    {
        var pagedAgents = await repository.GetAllByPaginationAsync(currentPage, offset, filters);
        return new PagedResult<DeliveryAgentDto>
        {
            Items = mapper.Map<List<DeliveryAgentDto>>(pagedAgents.Items),
            TotalCount = pagedAgents.TotalCount,
            CurrentPage = pagedAgents.CurrentPage,
            Offset = pagedAgents.Offset
        };
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
            agentDto.DateAdded = DateTime.UtcNow.AddTicks(-DateTime.UtcNow.Ticks % TimeSpan.TicksPerSecond);
            agentDto.DateModified = DateTime.UtcNow.AddTicks(-DateTime.UtcNow.Ticks % TimeSpan.TicksPerSecond);
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
            agentDto.DateModified = DateTime.UtcNow.AddTicks(-DateTime.UtcNow.Ticks % TimeSpan.TicksPerSecond);
            var updatedAgent =  await repository.UpdateAsync(mapper.Map<DeliveryAgent>(agentDto));
            return mapper.Map<DeliveryAgentDto>(updatedAgent);
        }

        public async Task<DeliveryAgentDto> UpdateAgentDeliveries(int agentId)
        {
            DeliveryAgent? agent = await repository.GetByIdAsync(agentId);
            agent!.TotalDeliveries = agent.TotalDeliveries + 1;
            agent.DateModified = DateTime.UtcNow.AddTicks(-DateTime.UtcNow.Ticks % TimeSpan.TicksPerSecond);
            agent = await repository.UpdateAsync(agent);
            return mapper.Map<DeliveryAgentDto>(agent);
        }

        public async Task<object> UpdateAgentPassword(int agentId, string oldPassword, string newPassword)
        {
            if (context.DeliveryAgents.Any(x => x.DeliveryAgentId == agentId))
            {
                var agent = await repository.GetByIdAsync(agentId);
                if (agent != null && !PasswordHasher.VerifyPassword(newPassword, agent.Password) &&
                    PasswordHasher.VerifyPassword(oldPassword, agent.Password))
                {
                    agent.Password = PasswordHasher.HashPassword(newPassword);
                    agent.DateModified = DateTime.UtcNow.AddTicks(-DateTime.UtcNow.Ticks % TimeSpan.TicksPerSecond);
                    agent = await repository.UpdateAsync(agent);
                    return mapper.Map<DeliveryAgentDto>(agent);
                }
                else
                {
                    return StatusCodesEnum.ValidationFailed;
                }
            }
            else
            {
                return StatusCodesEnum.DeliveryAgentNotFound;
            }
        }

        public async Task<bool> DeleteAgentAsync(int id)
        {
            return await repository.DeleteAsync(id);
        }
}