using AutoMapper;
using ECS_Logistics.DTOs;
using ECS_Logistics.FeignClients;
using ECS_Logistics.Models;
using ECS_Logistics.Repositories;
using ECS_Logistics.Utils;
using MongoDB.Bson;

namespace ECS_Logistics.Services;

public class OrderTrackingService(
    IOrderTrackingRepository orderTrackingRepository, 
    IMapper mapper,
    ILogger<OrderReturnService> logger,
    ProductService productService,
    CustomerService customerService,
    IDeliveryHubRepository deliveryHubRepository,
    IDeliveryAgentRepository deliveryAgentRepository,
    DistanceService distanceService) : IOrderTrackingService
{
    public async Task<object> GetAllByAgentIdAsync(int agentId)
    {
        var results = await orderTrackingRepository.GetAllByAgentIdAsync(agentId);
        List<OrderTrackingEnrichedDto> finalResults = [];
        var ordersTracking = results as OrderTracking[] ?? results.ToArray();
        foreach (var orderTracking in ordersTracking)
        {
            try
            {
                var enrichedData = await FetchEnrichedDtoRequiredFields(
                    orderTracking.OrderItemId,
                    orderTracking.CustomerAddressId,
                    orderTracking.DeliveryAgentId,
                    orderTracking.NearestHubId);
                var enrichedTrackingDto =  mapper.Map<OrderTracking, OrderTrackingEnrichedDto>(orderTracking, 
                opts =>
                {
                    opts.Items.Add("CustomerAddress", enrichedData["CustomerAddress"]);
                    opts.Items.Add("OrderItem", enrichedData["OrderItem"]);
                    opts.Items.Add("DeliveryAgent", enrichedData["DeliveryAgent"]);
                    opts.Items.Add("NearestHub", enrichedData["NearestHub"]);
                });
                finalResults.Add(enrichedTrackingDto);
            }
            catch (Exception ex)
            {
                return StatusCodesEnum.EnrichedDtoMappingsFailed;
            }
        }
        return finalResults.AsEnumerable();
    }

    public async Task<object> GetByIdAsync(string orderTrackingId)
    {
        try
        {
            var orderTracking = await orderTrackingRepository.GetByIdAsync(ObjectId.Parse(orderTrackingId));
            if (orderTracking == null)
            {
                return StatusCodesEnum.OrderTrackingNotFound;
            }
            var enrichedData = await FetchEnrichedDtoRequiredFields(
                orderTracking.OrderItemId,
                orderTracking.CustomerAddressId,
                orderTracking.DeliveryAgentId,
                orderTracking.NearestHubId);
            return mapper.Map<OrderTracking, OrderTrackingEnrichedDto>(orderTracking,
                opts =>
                {
                    opts.Items.Add("CustomerAddress", enrichedData["CustomerAddress"]);
                    opts.Items.Add("OrderItem", enrichedData["OrderItem"]);
                    opts.Items.Add("DeliveryAgent", enrichedData["DeliveryAgent"]);
                    opts.Items.Add("NearestHub", enrichedData["NearestHub"]);
                });
        }
        catch (Exception ex)
        {
            return StatusCodesEnum.EnrichedDtoMappingsFailed;
        }
    }

    public async Task<object> CreateAsync(OrderTrackingDto orderTrackingDto)
    {
        try
        {
            var enrichedData = await FetchEnrichedDtoRequiredFields(
                orderTrackingDto.OrderItemId,
                orderTrackingDto.CustomerAddressId,
                orderTrackingDto.DeliveryAgentId,
                orderTrackingDto.NearestHubId);
            orderTrackingDto.EstimatedDeliveryTime = await CalculateEstimatedTime(
                enrichedData["CustomerAddress"] as AddressDto,
                orderTrackingDto.OrderTrackingStatusId,
                orderTrackingDto.OrderTrackingType,
                enrichedData["DeliveryAgent"] as DeliveryAgentDto,
                enrichedData["NearestHub"] as DeliveryHubEnrichedDto);
            if (orderTrackingDto.DeliveryAgentId == null && orderTrackingDto.NearestHubId != null)
            {
                orderTrackingDto.DeliveryAgentId = await AssignAvailableNearestDeliveryAgent(
                    orderTrackingDto.CustomerAddressId, orderTrackingDto.NearestHubId ?? 0, orderTrackingDto.OrderTrackingType);
            }
            var orderTracking = await orderTrackingRepository.CreateAsync(
                mapper.Map<OrderTrackingDto, OrderTracking>(orderTrackingDto));
            
            return mapper.Map<OrderTracking, OrderTrackingEnrichedDto>(orderTracking,
                opts =>
                {
                    opts.Items.Add("CustomerAddress", enrichedData["CustomerAddress"]);
                    opts.Items.Add("OrderItem", enrichedData["OrderItem"]);
                    opts.Items.Add("DeliveryAgent", enrichedData["DeliveryAgent"]);
                    opts.Items.Add("NearestHub", enrichedData["NearestHub"]);
                });
            
        }
        catch(Exception ex)
        {
            return StatusCodesEnum.EnrichedDtoMappingsFailed;
        }
    }

    public async Task<object> UpdateAsync(OrderTrackingDto orderTrackingDto)
    {
        if (orderTrackingDto.OrderTrackingId == null)
        {
            return StatusCodesEnum.OrderTrackingNotFound;
        }
        try
        {
            var existingOrderTracking = await orderTrackingRepository.GetByIdAsync(ObjectId.Parse(orderTrackingDto.OrderTrackingId));
            if (existingOrderTracking == null)
                return StatusCodesEnum.OrderTrackingNotFound;
            if (!IsOrderTrackingSpecificFieldsUnchanged(orderTrackingDto, existingOrderTracking))
            {
                return StatusCodesEnum.ValidationFailed;
            }
            if (orderTrackingDto.DeliveryAgentId == null && orderTrackingDto.NearestHubId != null)
            {
                orderTrackingDto.DeliveryAgentId = await AssignAvailableNearestDeliveryAgent(
                    orderTrackingDto.CustomerAddressId, orderTrackingDto.NearestHubId ?? 0, orderTrackingDto.OrderTrackingType);
            }
            var enrichedData = await FetchEnrichedDtoRequiredFields(
                orderTrackingDto.OrderItemId,
                orderTrackingDto.CustomerAddressId,
                orderTrackingDto.DeliveryAgentId,
                orderTrackingDto.NearestHubId);
            orderTrackingDto.EstimatedDeliveryTime = await CalculateEstimatedTime(
                enrichedData["CustomerAddress"] as AddressDto,
                orderTrackingDto.OrderTrackingStatusId,
                orderTrackingDto.OrderTrackingType,
                enrichedData["DeliveryAgent"] as DeliveryAgentDto,
                enrichedData["NearestHub"] as DeliveryHubEnrichedDto);
            var orderTracking = await orderTrackingRepository.UpdateAsync(
                mapper.Map<OrderTrackingDto, OrderTracking>(orderTrackingDto));

            if (orderTracking == null)
            {
                return StatusCodesEnum.FailedToUpdateOrderTracking;
            }
            
            return mapper.Map<OrderTracking, OrderTrackingEnrichedDto>(orderTracking,
                opts =>
                {
                    opts.Items.Add("CustomerAddress", enrichedData["CustomerAddress"]);
                    opts.Items.Add("OrderItem", enrichedData["OrderItem"]);
                    opts.Items.Add("DeliveryAgent", enrichedData["DeliveryAgent"]);
                    opts.Items.Add("NearestHub", enrichedData["NearestHub"]);
                });
        }
        catch(Exception ex)
        {
            return StatusCodesEnum.EnrichedDtoMappingsFailed;
        }
    }

    private async Task<Dictionary<string, object>> FetchEnrichedDtoRequiredFields(
        int orderItemId, int addressId, int? deliveryAgentId, int? deliveryHubId)
    {
        var dictionary = new Dictionary<string, object>();
        try
        {
            var productResult = await productService.GetProductById(orderItemId);
            var addressResult = await customerService.GetAddressById(addressId);
            dictionary.Add("OrderItem", productResult);
            dictionary.Add("CustomerAddress", addressResult);
        }
        catch (Exception ex)
        {
            logger.LogError("OrderItems or Address not mapped : {message}", ex.Message);
            throw;
        }
        dictionary.Add("DeliveryAgent",
            deliveryAgentId == null ? null : await deliveryAgentRepository.GetByIdAsync(deliveryAgentId ?? 0));
        dictionary.Add("NearestHub", 
            deliveryHubId == null ? null : await deliveryHubRepository.GetByIdAsync(deliveryHubId ?? 0));
        return dictionary;
    }
    
    private static async Task<int> AssignAvailableNearestDeliveryAgent(int customerAddressId, int nearestHubId, int orderTrackingTypeId)
    {
        throw new NotImplementedException();
    }

    private async Task<DateTime> CalculateEstimatedTime(
        AddressDto? customerAddress, int orderTrackingStatusId, int orderTrackingTypeId, 
        DeliveryAgentDto? deliveryAgent, DeliveryHubEnrichedDto? nearestHub)
    {
        if (orderTrackingStatusId >= 3 && customerAddress != null && nearestHub?.DeliveryHubAddress != null)
        {
            var (distanceKm, durationText) =
                await distanceService.GetDistanceAsync(customerAddress.ToString(),
                    nearestHub.DeliveryHubAddress.ToString());
            TimeSpan estimatedTravelTime = HelperFunctions.ParseGoogleDuration(durationText);
            
            TimeSpan bufferTime = TimeSpan.FromMinutes(15);
            
            TimeSpan agentDelay = TimeSpan.Zero;
            if (deliveryAgent != null)
            {
                if (deliveryAgent.AvailabilityStatus != 1 && orderTrackingTypeId == 1)
                {
                    agentDelay = TimeSpan.FromDays(1.5);
                }
                if (deliveryAgent.AvailabilityStatus != 1 && orderTrackingTypeId == 2)
                {
                    agentDelay = TimeSpan.FromDays(1);
                }
            }
            else
            {
                agentDelay = TimeSpan.FromDays(2);
            }
            
            return DateTime.Now + bufferTime + estimatedTravelTime + agentDelay;
        }
        else
        {
            if (orderTrackingStatusId == 1)
            {
                return DateTime.Now.AddDays(6);
            }
            if (orderTrackingStatusId == 2)
            {
                return DateTime.Now.AddDays(4);
            }
            return DateTime.Now.AddDays(2);
        }
    }

    private static bool IsOrderTrackingSpecificFieldsUnchanged(OrderTrackingDto orderTrackingDto,
        OrderTracking orderTracking)
    {
        return orderTrackingDto.OrderItemId == orderTracking.OrderItemId &&
               orderTrackingDto.CustomerAddressId == orderTracking.CustomerAddressId &&
               orderTrackingDto.OrderTrackingStatusId >= orderTracking.OrderTrackingStatusId &&
               orderTrackingDto.OrderTrackingType == orderTracking.OrderTrackingType &&
               orderTrackingDto.CustomerInstructions == orderTracking.CustomerInstructions;
    }
}