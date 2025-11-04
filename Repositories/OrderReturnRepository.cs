using AutoMapper;
using ECS_Logistics.Data;
using ECS_Logistics.DbContexts;
using ECS_Logistics.DTOs;
using ECS_Logistics.Filters;
using ECS_Logistics.Models;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;

namespace ECS_Logistics.Repositories;

public class OrderReturnRepository(MySqlDbContext context, IMapper mapper) : IOrderReturnRepository
{
    public async Task<IEnumerable<OrderReturn>> GetAllAsync(OrderReturnFilters? filters)
    {
        var query = await GetOrderReturnQuery(filters);
        return await query.ToListAsync();
    }

    public async Task<PagedResult<OrderReturnEnrichedDto>> GetAllByPaginationAsync(int currentPage, int offset, OrderReturnFilters? filters)
    {
        var query = await GetOrderReturnQuery(filters);
        var results = mapper.Map<List<OrderReturnEnrichedDto>>(await query.ToListAsync());
        if (filters != null)
        {
            results = results.Where(orderReturn =>
                (filters.OrderStatusId != null &&
                orderReturn.OrderTracking?.OrderTrackingStatusId == filters.OrderStatusId) ||
                (filters.DeliveryAgentId !=null && 
                 orderReturn.OrderTracking?.DeliveryAgent?.DeliveryAgentId == filters.DeliveryAgentId) ||
                (filters.DeliveryHubId !=null &&
                 orderReturn.OrderTracking?.NearestHub?.DeliveryHubId == filters.DeliveryHubId) ||
                (filters.SearchValue is {Length: > 0} &&
                 (orderReturn.ReturnReason!.Contains(filters.SearchValue) || 
                  orderReturn.OrderTracking!.Product!.ProductName.Contains(filters.SearchValue) ||
                  orderReturn.OrderTracking!.Product!.ProductSubCategory.SubCategoryName.Contains(filters.SearchValue) ||
                  orderReturn.OrderTracking!.Product!.ProductSubCategory.ProductCategory.CategoryName.Contains(filters.SearchValue) ||
                  (int.TryParse(filters.SearchValue, out int id) && 
                   (orderReturn.CustomerId == id || orderReturn.OrderReturnId == id))
                  ))
                ).ToList();
        }
        var totalCount = results.Count;
        var items = results.OrderByDescending(or => or.OrderReturnId)
            .Skip(currentPage * offset)
            .Take(offset)
            .ToList();
        
        return new PagedResult<OrderReturnEnrichedDto>
        {
            Items = items,
            TotalCount = totalCount,
            CurrentPage = currentPage,
            Offset = offset
        };
    }

    public async Task<OrderReturn?> GetByIdAsync(int id)
    {
        return await context.OrderReturns.FindAsync(id);
    }

    public async Task<IEnumerable<OrderReturn>> GetAllByCustomerIdAsync(int customerId)
    {
        return await context.OrderReturns.Where(a => a.CustomerId == customerId).ToListAsync();
    }

    public async Task<OrderReturn> CreateAsync(OrderReturn orderReturn)
    {
        context.OrderReturns.Add(orderReturn);
        await context.SaveChangesAsync();
        return orderReturn;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var orderReturn = await context.OrderReturns.FindAsync(id);
        if (orderReturn == null)
            return false;
        context.OrderReturns.Remove(orderReturn);
        await context.SaveChangesAsync();
        return true;
    }

    private async Task<IQueryable<OrderReturn>> GetOrderReturnQuery(OrderReturnFilters? filters)
    {
        IQueryable<OrderReturn> query = context.OrderReturns.AsQueryable();
        if (filters == null)
        {
            return query;
        }
        if (filters is { FromDate: not null, ToDate: not null } && filters.FromDate < DateTime.UtcNow)
        {
            query = query.Where(a => a.DateAdded >= filters.FromDate && a.DateAdded <= filters.ToDate);
        }
        if (filters.ReturnReasonCategoryId != null)
        {
            query = query.Where(a => a.ReturnReasonCategoryId == filters.ReturnReasonCategoryId);
        }
        if (filters.ProductId != null)
        {
            query = query.Where(a => a.ProductId == filters.ProductId);
        }
        if (filters.CategoryId != null)
        {
            query = query.Where(a => a.CategoryId == filters.CategoryId);
        }
        if (filters.SubCategoryId != null)
        {
            query = query.Where(a => a.SubCategoryId == filters.SubCategoryId);
        }
        if (filters.BrandId != null)
        {
            query = query.Where(a => a.BrandId == filters.BrandId);
        }
        
        /* Applied all possible filters before retrieving from database to reduce the load */
        return query;
    } 
}