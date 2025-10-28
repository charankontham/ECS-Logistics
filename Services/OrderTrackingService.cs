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
    ILogger<OrderTrackingService> logger,
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

    public async Task<object> GetAllByPaginationAsync(int currentPage, int offset, OrderTrackingFilters? filters)
    {
        try
        {
            PagedResult<OrderTracking> results =
                await orderTrackingRepository.GetAllByPagination(currentPage, offset, filters);
            List<OrderTrackingEnrichedDto> finalResults = [];
            var ordersTracking = results.Items.ToArray() ?? [];
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
                    var enrichedTrackingDto = mapper.Map<OrderTracking, OrderTrackingEnrichedDto>(orderTracking,
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
                    Console.WriteLine("Error : "+ ex.Message);
                    return StatusCodesEnum.EnrichedDtoMappingsFailed;
                }
            }

            return new PagedResult<OrderTrackingEnrichedDto>()
            {
                Items = finalResults.ToList(),
                TotalCount = results.TotalCount,
                CurrentPage = results.CurrentPage,
                Offset = results.Offset,
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message, "Order tracking error");
            throw ex;
        }
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
            var orderTrackings = ordersTracking as OrderTracking[] ?? ordersTracking.ToArray();
            if (orderTrackings.ToList().Count < 1)
            {
                logger.LogWarning("Order tracking not found!");
                return StatusCodesEnum.OrderTrackingNotFound;
            }
            
            List<OrderTrackingEnrichedDto> finalResults = [];
            foreach (var orderTracking in orderTrackings)
            {
                var enrichedData = await FetchEnrichmentFields(
                    orderTracking.OrderItemId,
                    orderTracking.ProductId,
                    orderTracking.CustomerAddressId,
                    orderTracking.DeliveryAgentId,
                    orderTracking.NearestHubId);
                var finalDto = mapper.Map<OrderTracking, OrderTrackingEnrichedDto>(orderTracking,
                    opts =>
                    {
                        opts.Items.Add("CustomerAddress", enrichedData["CustomerAddress"]);
                        opts.Items.Add("OrderItem", enrichedData["OrderItem"]);
                        opts.Items.Add("Product", enrichedData["Product"]);
                        opts.Items.Add("DeliveryAgent", enrichedData["DeliveryAgent"]);
                        opts.Items.Add("NearestHub", enrichedData["NearestHub"]);
                    });
                finalResults.Add(finalDto);
            }
            return finalResults;
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
                orderTrackingDto.OrderTrackingStatusId = (int) OrderTrackingStatusEnum.WaitingForDeliveryAgent;
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
                orderTrackingDto.OrderTrackingStatusId = (int)OrderTrackingStatusEnum.WaitingForDeliveryAgent;
            }
            var enrichedData = await FetchEnrichmentFields(
                orderTrackingDto.OrderItemId,
                orderTrackingDto.ProductId,
                orderTrackingDto.CustomerAddressId,
                orderTrackingDto.DeliveryAgentId,
                orderTrackingDto.NearestHubId
            );
            orderTrackingDto.EstimatedDeliveryDate = await CalculateEstimatedTime(
                enrichedData["CustomerAddress"] as AddressDto,
                orderTrackingDto.OrderTrackingStatusId,
                orderTrackingDto.OrderTrackingType,
                enrichedData["DeliveryAgent"] as DeliveryAgentDto,
                enrichedData["NearestHub"] as DeliveryHubEnrichedDto
            );
            if (orderTrackingDto is { NearestHubId: not null, 
                    OrderTrackingStatusId: <= (int) OrderTrackingStatusEnum.ShipmentInTransit })
            {
                orderTrackingDto.OrderTrackingStatusId = (int) OrderTrackingStatusEnum.Shipped;
            }
            if (orderTrackingDto.OrderTrackingStatusId is (int) OrderTrackingStatusEnum.Delivered or 
                (int) OrderTrackingStatusEnum.Delivered) // delivered successfully or pickup successfully
            {
                orderTrackingDto.ActualDeliveryDate = DateTime.UtcNow.AddTicks(-DateTime.UtcNow.Ticks % TimeSpan.TicksPerSecond);
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

    public async Task<object> UpdateStatusAsync(string orderTrackingId, int statusId)
    {
        try
        {
            var existingOrderTracking = await orderTrackingRepository.GetByIdAsync(ObjectId.Parse(orderTrackingId));
            if (existingOrderTracking == null)
                return StatusCodesEnum.OrderTrackingNotFound;
            existingOrderTracking.OrderTrackingStatusId = statusId;
            var updatedOrderTracking = await orderTrackingRepository.UpdateAsync(existingOrderTracking) ??
                                       existingOrderTracking;
            var enrichedData = await FetchEnrichmentFields(
                updatedOrderTracking.OrderItemId,
                updatedOrderTracking.ProductId,
                updatedOrderTracking.CustomerAddressId,
                updatedOrderTracking.DeliveryAgentId,
                updatedOrderTracking.NearestHubId
            );
            // updatedOrderTracking.EstimatedDeliveryDate = await CalculateEstimatedTime(
            //     enrichedData["CustomerAddress"] as AddressDto,
            //     statusId,
            //     existingOrderTracking.OrderTrackingType,
            //     enrichedData["DeliveryAgent"] as DeliveryAgentDto,
            //     enrichedData["NearestHub"] as DeliveryHubEnrichedDto);
            return mapper.Map<OrderTracking, OrderTrackingEnrichedDto>(updatedOrderTracking,
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

    public async Task<bool> DeleteAsync(string orderTrackingId)
    {
        try
        {
            return await orderTrackingRepository.DeleteAsync(ObjectId.Parse(orderTrackingId));
        }
        catch (Exception ex)
        {
            logger.LogWarning("Exception while deleting order tracking : "+ ex.Message);
            return false;
        }
    }

    private async Task<Dictionary<string, object>> FetchEnrichmentFields(
        int orderItemId, int productId, int addressId, int? deliveryAgentId, int? deliveryHubId)
    {
        var dictionary = new Dictionary<string, object>();
        try {
            var orderItemResult = await orderService.GetOrderItemByOrderItemId(orderItemId);
            var productResult = await productService.GetProductById(productId);
            var addressResult = await customerService.GetAddressById(addressId);
            dictionary.Add("OrderItem", orderItemResult);
            dictionary.Add("Product", productResult);
            dictionary.Add("CustomerAddress", addressResult);
        }
        catch (Exception ex)
        {
            logger.LogError("OrderItem or Product or Address not mapped : {message}", ex.Message);
            throw;
        }
        dictionary.Add("DeliveryAgent",
            deliveryAgentId == null ? null : await deliveryAgentService.GetAgentByIdAsync(deliveryAgentId!.Value));
        dictionary.Add("NearestHub", 
            deliveryHubId == null ? null : await deliveryHubService.GetHubByIdAsync(deliveryHubId!.Value));
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

            await deliveryAgentService.UpdateAgentDeliveries(id!.Value);
            return id;
        }
        else
        {
            return null;
        }

        return 1;
        // deliveryAgentRepository.GetAllAsync()
    }

    private async Task<DateTime> CalculateEstimatedTime(
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
            var rawEstimatedTime = DateTime.UtcNow + bufferTime + estimatedTravelTime + agentDelay;
            const long ticksPerSecond = TimeSpan.TicksPerSecond;
            var ceilingTicks = (long)Math.Ceiling((double)rawEstimatedTime.Ticks / ticksPerSecond) * ticksPerSecond;
            
            return new DateTime(ceilingTicks, DateTimeKind.Utc);
        }
        else
        {
            if (orderTrackingStatusId == (int) OrderTrackingStatusEnum.OrderPlaced)
            {
                return DateTime.UtcNow.AddDays(6).AddTicks(-DateTime.UtcNow.Ticks % TimeSpan.TicksPerSecond);
            }
            if (orderTrackingStatusId == (int) OrderTrackingStatusEnum.ShipmentInTransit)
            {
                return DateTime.UtcNow.AddDays(4).AddTicks(-DateTime.UtcNow.Ticks % TimeSpan.TicksPerSecond);
            }
            return DateTime.UtcNow.AddDays(2).AddTicks(-DateTime.UtcNow.Ticks % TimeSpan.TicksPerSecond);
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