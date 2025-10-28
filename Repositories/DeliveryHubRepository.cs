using AutoMapper;
using ECS_Logistics.Data;
using ECS_Logistics.DbContexts;
using ECS_Logistics.DTOs;
using ECS_Logistics.Filters;
using ECS_Logistics.Models;
using ECS_Logistics.Utils;
using Microsoft.EntityFrameworkCore;

namespace ECS_Logistics.Repositories;

public class DeliveryHubRepository(MySqlDbContext context, ILogger<DeliveryHubRepository> logger, IMapper mapper)
    : IDeliveryHubRepository
{
    public async Task<IEnumerable<DeliveryHub>> GetAllAsync(DeliveryHubFilters? filters)
    {
        var query = await GetDeliveryHubQuery(filters);
        return await query.ToListAsync();
    }

    public async Task<PagedResult<DeliveryHubEnrichedDto>> GetAllByPaginationAsync(int currentPage, int offset,
        DeliveryHubFilters? filters)
    {
        var query = await GetDeliveryHubQuery(filters);
        var results = mapper.Map<List<DeliveryHubEnrichedDto>>(await query.ToListAsync());
        if (filters != null)
        {
            results = results.Where((dh) => ( string.IsNullOrEmpty(filters.SearchValue) ||
                                          (dh.DeliveryHubAddress != null &&
                                           (
                                               (dh.DeliveryHubAddress.City != null &&
                                                dh.DeliveryHubAddress.City.Contains(filters.SearchValue,
                                                    StringComparison.CurrentCultureIgnoreCase)
                                               ) ||
                                               (dh.DeliveryHubAddress.State != null &&
                                                dh.DeliveryHubAddress.State.Contains(filters.SearchValue,
                                                    StringComparison.CurrentCultureIgnoreCase)
                                               ) ||
                                               (dh.DeliveryHubAddress.Street != null &&
                                                dh.DeliveryHubAddress.Street.Contains(filters.SearchValue,
                                                    StringComparison.CurrentCultureIgnoreCase)
                                               ) ||
                                               (dh.DeliveryHubAddress.Country != null &&
                                                dh.DeliveryHubAddress.Country.Contains(filters.SearchValue,
                                                    StringComparison.CurrentCultureIgnoreCase)
                                               ) ||
                                               (dh.DeliveryHubAddress.Zip != null &&
                                                dh.DeliveryHubAddress.Zip.Contains(filters.SearchValue,
                                                    StringComparison.CurrentCultureIgnoreCase)
                                               )
                                           )))
                                          && ( string.IsNullOrEmpty(filters.Address) || dh.DeliveryHubAddress != null &&
                                              filters.Address.Equals(dh.DeliveryHubAddress.State, 
                                                  StringComparison.CurrentCultureIgnoreCase) )
            ).ToList();
        }
        var totalCount = results.Count;
        var items = results.OrderByDescending(dh => dh.DeliveryHubId)
            .Skip(currentPage * offset)
            .Take(offset)
            .ToList();

        return new PagedResult<DeliveryHubEnrichedDto>
        {
            Items = items,
            TotalCount = totalCount,
            CurrentPage = currentPage,
            Offset = offset
        };
    }

    public async Task<DeliveryHub?> GetByIdAsync(int id)
    {
        return await context.DeliveryHubs.FindAsync(id);
    }

    public async Task<DeliveryHub> CreateAsync(DeliveryHub hub)
    {
        hub.DateAdded = DateTime.UtcNow.AddTicks(-DateTime.UtcNow.Ticks % TimeSpan.TicksPerSecond);
        hub.DateModified = DateTime.UtcNow.AddTicks(-DateTime.UtcNow.Ticks % TimeSpan.TicksPerSecond);
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

    private async Task<IQueryable<DeliveryHub>> GetDeliveryHubQuery(DeliveryHubFilters? filters)
    {
        var query = context.DeliveryHubs.AsQueryable();
        if (filters == null)
        {
            return query;
        }

        if (filters.DeliveryHubName != null && filters.DeliveryHubName.Trim() != "")
        {
            query = query.Where(a =>
                a.DeliveryHubName.ToLower().Contains(filters.DeliveryHubName.ToLower()));
        }

        // if (filters.SearchValue != null && filters.SearchValue.Trim() != "")
        // {
        //     query = query.Where((dh) =>
        //         dh.DeliveryHubName.ToLower().Contains(filters.SearchValue.ToLower()));
        // }

        /* Applied all possible filters before retrieving from database to reduce the load */
        return query;
    }
}