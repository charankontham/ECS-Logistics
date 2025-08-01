using AutoMapper;
using ECS_Logistics.DTOs;
using ECS_Logistics.FeignClients;
using ECS_Logistics.Filters;
using ECS_Logistics.Models;
using ECS_Logistics.Repositories;
using ECS_Logistics.Utils;
using MongoDB.Bson;

namespace ECS_Logistics.Services;

public class OrderTrackingService(
    IOrderTrackingRepository orderTrackingRepository, 
    IMapper mapper,
    ILogger<OrderReturnService> logger,
    OrderService orderService,
    ProductService productService,
    CustomerService customerService,
    IDeliveryHubService deliveryHubService,
    IDeliveryAgentService deliveryAgentService,
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
                var enrichedData = await FetchEnrichmentFields(
                    orderTracking.OrderItemId,
                    orderTracking.ProductId,
                    orderTracking.CustomerAddressId,
                    orderTracking.DeliveryAgentId,
                    orderTracking.NearestHubId);
                var enrichedTrackingDto =  mapper.Map<OrderTracking, OrderTrackingEnrichedDto>(orderTracking, 
                opts =>
                {
                    opts.Items.Add("CustomerAddress", enrichedData["CustomerAddress"]);
                    opts.Items.Add("OrderItem", enrichedData["OrderItem"]);
                    opts.Items.Add("Product", enrichedData["Product"]);
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
                logger.LogWarning("Order tracking not found!");
                return StatusCodesEnum.OrderTrackingNotFound;
            }
            var enrichedData = await FetchEnrichmentFields(
                orderTracking.OrderItemId,
                orderTracking.ProductId,
                orderTracking.CustomerAddressId,
                orderTracking.DeliveryAgentId,
                orderTracking.NearestHubId);
            return mapper.Map<OrderTracking, OrderTrackingEnrichedDto>(orderTracking,
                opts =>
                {
                    opts.Items.Add("CustomerAddress", enrichedData["CustomerAddress"]);
                    opts.Items.Add("OrderItem", enrichedData["OrderItem"]);
                    opts.Items.Add("Product", enrichedData["Product"]);
                    opts.Items.Add("DeliveryAgent", enrichedData["DeliveryAgent"]);
                    opts.Items.Add("NearestHub", enrichedData["NearestHub"]);
                });
        }
        catch (Exception ex)
        {
            return StatusCodesEnum.EnrichedDtoMappingsFailed;
        }
    }

    public async Task<object> GetByOrderIdAndProductIdAsync(int orderId, int productId)
    {
        try
        {
            var orderItemDto = await orderService.GetOrderItemByOrderIdAndProductId(orderId, productId);
            if (orderItemDto == null)
            {
                logger.LogWarning("OrderItem not found!");
                return StatusCodesEnum.OrderTrackingNotFound;
            }
            var ordersTracking = await orderTrackingRepository.
                GetAllByOrderItemIdAsync(orderItemDto.OrderItemId);
            if (ordersTracking.ToList().Count < 1)
            {
                logger.LogWarning("Order tracking not found!");
                return StatusCodesEnum.OrderTrackingNotFound;
            }
            var orderTracking = ordersTracking.First();
            var enrichedData = await FetchEnrichmentFields(
                orderTracking.OrderItemId,
                orderTracking.ProductId,
                orderTracking.CustomerAddressId,
                orderTracking.DeliveryAgentId,
                orderTracking.NearestHubId);
            return mapper.Map<OrderTracking, OrderTrackingEnrichedDto>(orderTracking,
                opts =>
                {
                    opts.Items.Add("CustomerAddress", enrichedData["CustomerAddress"]);
                    opts.Items.Add("OrderItem", enrichedData["OrderItem"]);
                    opts.Items.Add("Product", enrichedData["Product"]);
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
            var enrichedData = await FetchEnrichmentFields(
                orderTrackingDto.OrderItemId,
                orderTrackingDto.ProductId,
                orderTrackingDto.CustomerAddressId,
                orderTrackingDto.DeliveryAgentId,
                orderTrackingDto.NearestHubId);
            orderTrackingDto.EstimatedDeliveryDate = await CalculateEstimatedTime(
                enrichedData["CustomerAddress"] as AddressDto,
                orderTrackingDto.OrderTrackingStatusId,
                orderTrackingDto.OrderTrackingType,
                enrichedData["DeliveryAgent"] as DeliveryAgentDto,
                enrichedData["NearestHub"] as DeliveryHubEnrichedDto);
            if (orderTrackingDto.DeliveryAgentId == null && orderTrackingDto.NearestHubId != null)
            {
                orderTrackingDto.DeliveryAgentId = await AssignAvailableNearestDeliveryAgent(orderTrackingDto.NearestHubId ?? 0);
            }
            var currentOrdersTracking = 
                await orderTrackingRepository.GetAllByOrderItemIdAsync(orderTrackingDto.OrderItemId);
            var alreadyOrderTrackingExists = currentOrdersTracking.ToList().Find(ot =>
                ot.ProductId == orderTrackingDto.ProductId &&
                ot.OrderItemId == orderTrackingDto.OrderItemId &&
                ot.OrderTrackingType == orderTrackingDto.OrderTrackingType
            );
            if (alreadyOrderTrackingExists != null)
            {
                return StatusCodesEnum.DuplicateOrderTracking;
            }
            var orderTracking = await orderTrackingRepository.CreateAsync(
                mapper.Map<OrderTrackingDto, OrderTracking>(orderTrackingDto));
            logger.LogInformation("Order tracking created with Id : {0}", orderTracking.OrderTrackingId.ToString());
            return mapper.Map<OrderTracking, OrderTrackingEnrichedDto>(orderTracking,
                opts =>
                {
                    opts.Items.Add("CustomerAddress", enrichedData["CustomerAddress"]);
                    opts.Items.Add("OrderItem", enrichedData["OrderItem"]);
                    opts.Items.Add("Product", enrichedData["Product"]);
                    opts.Items.Add("DeliveryAgent", enrichedData["DeliveryAgent"]);
                    opts.Items.Add("NearestHub", enrichedData["NearestHub"]);
                });
        }
        catch(Exception ex)
        {
            logger.LogWarning("Exception while processing order tracking : "+ ex.Message);
            return StatusCodesEnum.ValidationFailed;
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
                    orderTrackingDto.NearestHubId ?? 0);
            }
            var enrichedData = await FetchEnrichmentFields(
                orderTrackingDto.OrderItemId,
                orderTrackingDto.ProductId,
                orderTrackingDto.CustomerAddressId,
                orderTrackingDto.DeliveryAgentId,
                orderTrackingDto.NearestHubId);
            orderTrackingDto.EstimatedDeliveryDate = await CalculateEstimatedTime(
                enrichedData["CustomerAddress"] as AddressDto,
                orderTrackingDto.OrderTrackingStatusId,
                orderTrackingDto.OrderTrackingType,
                enrichedData["DeliveryAgent"] as DeliveryAgentDto,
                enrichedData["NearestHub"] as DeliveryHubEnrichedDto);
            if (orderTrackingDto.OrderTrackingStatusId is (int) OrderTrackingStatusEnum.Delivered or 
                (int) OrderTrackingStatusEnum.Delivered) // delivered successfully or pickup successfully
            {
                orderTrackingDto.ActualDeliveryDate = DateTimeOffset.Now;
            }
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
                    opts.Items.Add("Product", enrichedData["Product"]);
                    opts.Items.Add("DeliveryAgent", enrichedData["DeliveryAgent"]);
                    opts.Items.Add("NearestHub", enrichedData["NearestHub"]);
                });
        }
        catch(Exception ex)
        {
            return StatusCodesEnum.EnrichedDtoMappingsFailed;
        }
    }

    private async Task<Dictionary<string, object>> FetchEnrichmentFields(
        int orderItemId, int productId, int addressId, int? deliveryAgentId, int? deliveryHubId)
    {
        var dictionary = new Dictionary<string, object>();
        try
        {
            var orderItemResult = await orderService.GetOrderItemByOrderItemId(orderItemId);
            var productResult = await productService.GetProductById(productId);
            var addressResult = await customerService.GetAddressById(addressId);
            dictionary.Add("OrderItem", orderItemResult);
            dictionary.Add("Product", productResult);
            dictionary.Add("CustomerAddress", addressResult);
        }
        catch (Exception ex)
        {
            logger.LogError("OrderItems or Address not mapped : {message}", ex.Message);
            throw;
        }
        dictionary.Add("DeliveryAgent",
            deliveryAgentId == null ? null : await deliveryAgentService.GetAgentByIdAsync(deliveryAgentId ?? 0));
        dictionary.Add("NearestHub", 
            deliveryHubId == null ? null : await deliveryHubService.GetHubByIdAsync(deliveryHubId ?? 0));
        return dictionary;
    }
    
    private async Task<int?> AssignAvailableNearestDeliveryAgent(int nearestHubId)
    {
        var deliveryHubResponse = await deliveryHubService.GetHubByIdAsync(nearestHubId);
        if (deliveryHubResponse is DeliveryHubEnrichedDto dto)
        {
            var nearestHubCityName = dto.DeliveryHubAddress?.City;
            var deliveryAgentFilters = new DeliveryAgentFilters
            {
                ServingArea = [nearestHubCityName?.ToUpper() ?? "", nearestHubCityName?.ToLower() ?? ""],
                Availability = [1,2]
            };
            var deliveryAgents = await deliveryAgentService.GetAllAgentsAsync(deliveryAgentFilters);
            var id = deliveryAgents.ToList().FirstOrDefault(d => 
                d.AvailabilityStatus == 1 && d.ServingArea.Equals(nearestHubCityName)
                )?.DeliveryAgentId;
            if (id == null)
            {
                return deliveryAgents.ToList().FirstOrDefault(d => 
                    d.AvailabilityStatus == 2 && d.ServingArea.Equals(nearestHubCityName)
                    )?.DeliveryAgentId;
            }
            return id;
        }
        else
        {
            return null;
        }

        return 1;
        // deliveryAgentRepository.GetAllAsync()
    }

    private async Task<DateTimeOffset> CalculateEstimatedTime(
        AddressDto? customerAddress, int orderTrackingStatusId, int orderTrackingTypeId, 
        DeliveryAgentDto? deliveryAgent, DeliveryHubEnrichedDto? nearestHub)
    {
        if (orderTrackingStatusId >= (int) OrderTrackingStatusEnum.Shipped && customerAddress != null && nearestHub?.DeliveryHubAddress != null)
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
            
            return DateTimeOffset.Now + bufferTime + estimatedTravelTime + agentDelay;
        }
        else
        {
            if (orderTrackingStatusId == (int) OrderTrackingStatusEnum.OrderPlaced)
            {
                return DateTimeOffset.Now.AddDays(6);
            }
            if (orderTrackingStatusId == (int) OrderTrackingStatusEnum.ShipmentInTransit)
            {
                return DateTime.Now.AddDays(4);
            }
            return DateTime.Now.AddDays(2);
        }
    }

    private static bool IsOrderTrackingSpecificFieldsUnchanged(OrderTrackingDto orderTrackingDto,
        OrderTracking orderTracking)
    {
        return orderTrackingDto.ProductId == orderTracking.ProductId &&
               orderTrackingDto.OrderItemId == orderTracking.OrderItemId &&
               orderTrackingDto.CustomerAddressId == orderTracking.CustomerAddressId &&
               orderTrackingDto.OrderTrackingStatusId >= orderTracking.OrderTrackingStatusId &&
               orderTrackingDto.OrderTrackingType == orderTracking.OrderTrackingType &&
               orderTrackingDto.CustomerInstructions == orderTracking.CustomerInstructions;
    }
}