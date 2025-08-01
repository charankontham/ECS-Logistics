using ECS_Logistics.Data;
using ECS_Logistics.DbContexts;
using ECS_Logistics.Filters;
using ECS_Logistics.Models;
using ECS_Logistics.Utils;
using Microsoft.EntityFrameworkCore;

namespace ECS_Logistics.Repositories;

public class DeliveryHubRepository(MySqlDbContext context, ILogger<DeliveryHubRepository> logger) : IDeliveryHubRepository
{
    public async Task<IEnumerable<DeliveryHub>> GetAllAsync(DeliveryHubFilters? filters)
    {
        var query = context.DeliveryHubs.AsQueryable();
        if (filters == null)
        {
            return await query.ToListAsync();
        }
        if (filters.DeliveryHubName != null && filters.DeliveryHubName.Trim() != "")
        {
            query = query.Where(a =>
                a.DeliveryHubName.Contains(filters.DeliveryHubName, StringComparison.CurrentCultureIgnoreCase));
        }
        /* Applied all possible filters before retrieving from database to reduce the load */
        return await query.ToListAsync();
    }

    public async Task<DeliveryHub?> GetByIdAsync(int id)
    {
        return await context.DeliveryHubs.FindAsync(id);
    }

    public async Task<DeliveryHub> CreateAsync(DeliveryHub hub)
    {
        hub.DateAdded = DateTimeOffset.UtcNow;
        hub.DateModified = DateTimeOffset.UtcNow;
        context.DeliveryHubs.Add(hub);
        await context.SaveChangesAsync();
        return hub;
    }

    public async Task<DeliveryHub> UpdateAsync(DeliveryHub hub)
    {
        var existingHub = await context.DeliveryHubs.FindAsync(hub.DeliveryHubId);
        if (existingHub != null)
        {
            hub.DateAdded = existingHub.DateAdded;
            context.Entry(existingHub).State = EntityState.Detached;
            context.DeliveryHubs.Update(hub);
            await context.SaveChangesAsync();
            return hub;
        }
        else
        {
            logger.LogInformation("DeliveryHub not found!");
            throw new Exception("DeliveryHub not found");
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var hub = await context.DeliveryHubs.FindAsync(id);
        if (hub == null) return false;
        context.DeliveryHubs.Remove(hub);
        await context.SaveChangesAsync();
        return true;
    }
}