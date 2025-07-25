using AutoMapper;
using ECS_Logistics.DTOs;
using ECS_Logistics.Filters;
using ECS_Logistics.Repositories;
using ECS_Logistics.Utils;

namespace ECS_Logistics.Services;

public class OrderReturnService(
    IOrderReturnRepository repository, 
    ILogger<OrderReturnService> logger,
    IMapper mapper) : IOrderReturnService
{
    public async Task<IEnumerable<OrderReturnEnrichedDto>> GetAllAsync(OrderReturnFilters? filters)
    {
        var orderReturns = await repository.GetAllAsync(filters);
        return mapper.Map<IEnumerable<OrderReturnEnrichedDto>>(orderReturns);
    }

    public async Task<object> GetByIdAsync(int id)
    {
        var orderReturn = await repository.GetByIdAsync(id);
        return orderReturn != null ? orderReturn : StatusCodesEnum.OrderReturnNotFound;
    }

    public async Task<IEnumerable<OrderTrackingEnrichedDto>> GetAllByCustomerIdAsync(int customerId)
    {
        var orderReturns =  await repository.GetAllByCustomerIdAsync(customerId);
        return mapper.Map<IEnumerable<OrderTrackingEnrichedDto>>(orderReturns);
    }

    public async Task<OrderTrackingEnrichedDto> CreateAsync(OrderTrackingDto orderTrackingDto)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteOrderReturnAsync(int id)
    {
        throw new NotImplementedException();
    }
}