using ECS_Logistics.Data;
using ECS_Logistics.DbContexts;
using ECS_Logistics.Filters;
using ECS_Logistics.Models;
using Microsoft.EntityFrameworkCore;

namespace ECS_Logistics.Repositories;

public class OrderReturnRepository(MySqlDbContext context) : IOrderReturnRepository
{
    public async Task<IEnumerable<OrderReturn>> GetAllAsync(OrderReturnFilters? filters)
    {
        var query = context.OrderReturns.AsQueryable();
        if (filters == null)
        {
            return await query.ToListAsync();
        }
        if (filters is { FromDate: not null, ToDate: not null } && filters.FromDate < DateTimeOffset.Now)
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
        return await query.ToListAsync();
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
}