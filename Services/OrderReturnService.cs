using System.Diagnostics;
using System.Transactions;
using AutoMapper;
using ECS_Logistics.DTOs;
using ECS_Logistics.FeignClients;
using ECS_Logistics.Filters;
using ECS_Logistics.Models;
using ECS_Logistics.Repositories;
using ECS_Logistics.Utils;

namespace ECS_Logistics.Services;

public class OrderReturnService(
    IOrderReturnRepository orderReturnRepository, 
    ILogger<OrderReturnService> logger,
    IMapper mapper,
    IOrderTrackingService orderTrackingService,
    OrderService orderService) : IOrderReturnService
{
    public async Task<IEnumerable<OrderReturnEnrichedDto>> GetAllReturnsAsync(OrderReturnFilters? filters)
    {
        var orderReturns = await orderReturnRepository.GetAllAsync(filters);
        return mapper.Map<IEnumerable<OrderReturnEnrichedDto>>(orderReturns);
    }

    public async Task<object> GetOrderReturnByIdAsync(int id)
    {
        var orderReturn = await orderReturnRepository.GetByIdAsync(id);
        return orderReturn != null ? orderReturn : StatusCodesEnum.OrderReturnNotFound;
    }

    public async Task<object> GetAllByCustomerIdAsync(int customerId)
    {
        try
        {
            var orderReturns = await orderReturnRepository.GetAllByCustomerIdAsync(customerId);
            return mapper.Map<IEnumerable<OrderTrackingEnrichedDto>>(orderReturns);
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return StatusCodesEnum.EnrichedDtoMappingsFailed;
        }
    }

    public async Task<object> CreateReturnAsync(OrderReturnDto orderReturnDto)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        try
        {
            var orderItemDto = await orderService.GetOrderItemByOrderItemId(orderReturnDto.OrderItemId);
            var orderTrackingDto = new OrderTrackingDto
            {
                OrderTrackingType = 2,
                OrderItemId = orderItemDto?.OrderItemId ?? 0,
                ProductId = orderItemDto?.ProductId ?? 0,
                OrderTrackingStatusId = 1,
                CustomerAddressId = orderReturnDto.PickupAddressId
            };
            var orderTrackingResponse = await orderTrackingService.CreateAsync(orderTrackingDto);
            if (orderTrackingResponse is OrderTrackingEnrichedDto dto)
            {
                orderTrackingDto.OrderTrackingId = dto.OrderTrackingId;
            }
            else if (orderTrackingResponse is StatusCodesEnum statusCode)
            {
                logger.LogWarning("OrderTracking not successful! StatusCode : {StatusCodesEnum} ", statusCode);
                return statusCode;
            }
            else
            {
                return StatusCodesEnum.InternalServerError;
            }
            var contextDictionary = GetContextDictionary(orderItemDto?.ProductId ?? 0,
                orderTrackingResponse as OrderTrackingEnrichedDto);
            var orderReturn = mapper.Map<OrderReturnDto, OrderReturn>(orderReturnDto, opts =>
            {
                opts.Items.Add(contextDictionary.ElementAt(0).Key, contextDictionary.ElementAt(0).Value);
                opts.Items.Add(contextDictionary.ElementAt(1).Key, contextDictionary.ElementAt(1).Value);
                opts.Items.Add(contextDictionary.ElementAt(2).Key, contextDictionary.ElementAt(2).Value);
                opts.Items.Add(contextDictionary.ElementAt(3).Key, contextDictionary.ElementAt(3).Value);
            });
            var createdOrderReturn = await orderReturnRepository.CreateAsync(orderReturn);
            var finalResult = mapper.Map<OrderReturnEnrichedDto>(createdOrderReturn);
            scope.Complete();
            return finalResult;
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCodesEnum.ValidationFailed;
        }
    }

    public async Task<bool> DeleteOrderReturnAsync(int orderReturnId)
    {
        return await orderReturnRepository.DeleteAsync(orderReturnId);
    }

    private static Dictionary<string, int> GetContextDictionary(
        int productId, OrderTrackingEnrichedDto? orderTrackingDto)
    {
        var dictionary = new Dictionary<string, int>
        {
            { "ProductId", productId },
            { "CategoryId", orderTrackingDto?.Product?.ProductSubCategory.ProductCategory.CategoryId ?? 0},
            { "SubCategoryId", orderTrackingDto?.Product?.ProductSubCategory.SubCategoryId ?? 0},
            { "BrandId", orderTrackingDto?.Product?.Brand.BrandId ?? 0}
        };
        return dictionary;
    }
}