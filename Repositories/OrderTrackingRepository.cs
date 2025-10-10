using ECS_Logistics.Data;
using ECS_Logistics.DTOs;
using ECS_Logistics.Filters;
using ECS_Logistics.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ECS_Logistics.Repositories;

public class OrderTrackingRepository(
    IMongoDatabase database, 
    ILogger<OrderReturnRepository> logger
    ) : IOrderTrackingRepository
{
    private readonly IMongoCollection<OrderTracking> _orderTracking = 
        database.GetCollection<OrderTracking>("order_tracking");

    public async Task<IEnumerable<OrderTracking>> GetAllByAgentIdAsync(int agentId)
    {
        return await _orderTracking.Find(ot => ot.DeliveryAgentId == agentId).ToListAsync();
    }

    public async Task<IEnumerable<OrderTracking>> GetAllByOrderItemIdAsync(int orderItemId)
    {
        try
        {
            return await _orderTracking.Find(ot => ot.OrderItemId == orderItemId).ToListAsync();
        }
        catch(MongoAuthenticationException e)
        {
            logger.LogError(e.Message, " MongoDB authentication failed");
            throw;
        }
    }

    public async Task<PagedResult<OrderTracking>> GetAllByPagination(int currentPage, int offset,
        OrderTrackingFilters? filters)
    {
        try
        {
            var filterBuilder = Builders<OrderTracking>.Filter;
            var filter = filterBuilder.Empty;

            if (filters != null)
            {
                var filtersList = new List<FilterDefinition<OrderTracking>>();

                if (filters.EstimatedDeliveryDate is not null)
                    filtersList.Add(filterBuilder.Eq(ot => ot.EstimatedDeliveryDate,
                        filters.EstimatedDeliveryDate.Value));

                if (filters.OrderTrackingStatusId is not null)
                    filtersList.Add(filterBuilder.Eq(ot => ot.OrderTrackingStatusId,
                        filters.OrderTrackingStatusId.Value));

                if (filters.DeliveryAgentId is not null)
                    filtersList.Add(filterBuilder.Eq(ot => ot.DeliveryAgentId, filters.DeliveryAgentId.Value));

                if (filters.OrderTrackingType is not null)
                    filtersList.Add(filterBuilder.Eq(ot => ot.OrderTrackingType, filters.OrderTrackingType));

                if (filtersList.Count != 0)
                    filter = filterBuilder.And(filtersList);
            }

            var totalCount = await _orderTracking.CountDocumentsAsync(filter);
            IEnumerable<OrderTracking> otItems = await _orderTracking.Find(filter)
                .SortBy(ot => ot.OrderTrackingId)
                .Skip(currentPage * offset)
                .Limit(offset)
                .ToListAsync();
            return new PagedResult<OrderTracking>
            {
                Items = otItems.ToList(),
                TotalCount = (int)totalCount,
                CurrentPage = currentPage,
                Offset = offset
            };
        }
        catch (Exception e)
        {
            logger.LogError(e.Message, " Database error");
            throw e;
        }
    }

    public async Task<OrderTracking?> GetByIdAsync(ObjectId orderTrackingId)
    {
        return await _orderTracking.Find(ot => ot.OrderTrackingId == orderTrackingId).FirstOrDefaultAsync();
    }

    public async Task<OrderTracking> CreateAsync(OrderTracking orderTracking)
    {
        await _orderTracking.InsertOneAsync(orderTracking);
        return orderTracking;
    }

    public async Task<OrderTracking?> UpdateAsync(OrderTracking orderTracking)
    {
        var result = await _orderTracking.ReplaceOneAsync(
            t => t.OrderTrackingId == orderTracking.OrderTrackingId,
            orderTracking,
            new ReplaceOptions { IsUpsert = false }
        );
        return result.ModifiedCount > 0 ? orderTracking : null;
    }
}