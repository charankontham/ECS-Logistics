using ECS_Logistics.Data;
using ECS_Logistics.Filters;
using ECS_Logistics.Models;
using ECS_Logistics.Utils;
using Microsoft.EntityFrameworkCore;

namespace ECS_Logistics.Repositories;

public class DeliveryAgentRepository(MySqlDbContext context) : IDeliveryAgentRepository
{
    public async Task<IEnumerable<DeliveryAgent>> GetAllAsync(DeliveryAgentFilters? filters)
    {
        var query = context.DeliveryAgents.AsQueryable();
        if (filters == null)
        {
            return await query.ToListAsync();
        }
        if (filters.Availability != null)
        {
            query = query.Where(a => a.AvailabilityStatus == filters.Availability);
        }
        if (filters.ServingArea != null)
        {
            query = query.Where(a => a.ServingArea == filters.ServingArea);
        }
        if (filters.DeliveryAgentName != null)
        {
            query = query.Where(a => a.DeliveryAgentName == filters.DeliveryAgentName);
        }
        return await query.ToListAsync();
    }

    public async Task<DeliveryAgent?> GetByIdAsync(int id)
    {
        return await context.DeliveryAgents.FindAsync(id);
    }

    public async Task<DeliveryAgent?> GetByEmailAsync(string email)
    {
        return await context.DeliveryAgents.FirstOrDefaultAsync(agent => agent.Email == email);
    }

    public async Task<DeliveryAgent> CreateAsync(DeliveryAgent agent)
    {
        agent.Password = PasswordHasher.HashPassword(agent.Password);
        context.DeliveryAgents.Add(agent);
        await context.SaveChangesAsync();
        return agent;
    }

    public async Task<DeliveryAgent> UpdateAsync(DeliveryAgent agent)
    {
        var existingAgent = await context.DeliveryAgents.FindAsync(agent.DeliveryAgentId);
        if (existingAgent != null)
        {
            agent.Password = existingAgent.Password;
            agent.DateAdded = existingAgent.DateAdded;
            context.Entry(existingAgent).State = EntityState.Detached;
            context.DeliveryAgents.Update(agent);
            await context.SaveChangesAsync();
            return agent;
        }
        else
        {
            throw new Exception("Agent not found");
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var agent = await context.DeliveryAgents.FindAsync(id);
        if (agent == null) return false;
        context.DeliveryAgents.Remove(agent);
        await context.SaveChangesAsync();
        return true;
    }
}