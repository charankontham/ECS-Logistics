using ECS_Logistics.Data;
using ECS_Logistics.Filters;
using ECS_Logistics.Models;
using Microsoft.EntityFrameworkCore;

namespace ECS_Logistics.Repositories;

public class DeliveryAgentRepository(MySqlDbContext context) : IDeliveryAgentRepository
{
    public async Task<IEnumerable<DeliveryAgent>> GetAllAsync(DeliveryAgentFilters filters)
    {
        var query = context.DeliveryAgents.AsQueryable();
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

    public async Task<DeliveryAgent> CreateAsync(DeliveryAgent agent)
    {
        agent.DateAdded = DateTime.Now;
        agent.DateModified = DateTime.Now;
        context.DeliveryAgents.Add(agent);
        await context.SaveChangesAsync();
        return agent;
    }

    public async Task<DeliveryAgent> UpdateAsync(DeliveryAgent agent)
    {
        agent.DateModified = DateTime.Now;
        context.DeliveryAgents.Update(agent);
        await context.SaveChangesAsync();
        return agent;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var agent = await context.DeliveryAgents.FindAsync(id);
        if (agent != null)
        {
            context.DeliveryAgents.Remove(agent);
            await context.SaveChangesAsync();
            return true;
        }
        else
        {
            return false;
        }
    }
}