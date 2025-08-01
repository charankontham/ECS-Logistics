using ECS_Logistics.Configs;
using ECS_Logistics.DTOs;
using ECS_Logistics.Services;
using ECS_Logistics.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECS_Logistics.Controllers;
[Route("api/orderTracking")]
[ApiController]
[Authorize(Roles = "ROLE_LOGISTICS_ADMIN,ROLE_CUSTOMER")]
public class OrderTrackingController(
    IOrderTrackingService orderTrackingService, 
    KafkaProducerService kafkaProducerService) : ControllerBase
{
    [HttpGet("getAllByAgentId/{agentId:int}")]
    [Authorize(Roles = "ROLE_LOGISTICS_ADMIN")]
    public async Task<IActionResult> GetAll(int agentId)
    {
        var ordersTracking = await orderTrackingService.GetAllByAgentIdAsync(agentId);
        return await HelperFunctions.GetFinalHttpResponse(ordersTracking);
    }
    
    [HttpGet("{orderTrackingId}")]
    [Authorize(Roles = "ROLE_CUSTOMER, ROLE_LOGISTICS_ADMIN")]
    public async Task<IActionResult> GetById(string orderTrackingId)
    {
        try
        {
            var response = await orderTrackingService.GetByIdAsync(orderTrackingId);
            return await HelperFunctions.GetFinalHttpResponse(response);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Order Tracking Controller : {0}", ex.Message);
            return NotFound("Order tracking not found!");
        }
    }
    
    [HttpGet("getByOrderIdAndProductId/{orderId:int}/{productId:int}")]
    [Authorize(Roles = "ROLE_CUSTOMER, ROLE_LOGISTICS_ADMIN")]
    public async Task<IActionResult> GetByOrderIdAndProductId(int orderId, int productId)
    {
        try
        {
            var response = await orderTrackingService.GetByOrderIdAndProductIdAsync(orderId, productId);
            return await HelperFunctions.GetFinalHttpResponse(response);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Order Tracking Controller : {0}", ex.Message);
            return NotFound("Order tracking not found!");
        }
    }
        
    [HttpPost]
    [Authorize(Roles = "ROLE_CUSTOMER, ROLE_LOGISTICS_ADMIN")]
    public async Task<IActionResult> Create([FromBody] OrderTrackingDto orderTrackingDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var response = await HelperFunctions.GetFinalHttpResponse(
            await orderTrackingService.CreateAsync(orderTrackingDto));
        return response is OkObjectResult { Value: OrderTrackingEnrichedDto createdOrderTracking } ? 
            CreatedAtAction(nameof(GetById), new
            {
                orderTrackingId = createdOrderTracking.OrderTrackingId
            }, createdOrderTracking) : response;
    }
        
    [HttpPut]
    [Authorize(Roles = "ROLE_LOGISTICS_ADMIN")]
    public async Task<IActionResult> Update([FromBody] OrderTrackingDto orderTrackingDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var response = await orderTrackingService.UpdateAsync(orderTrackingDto);
        if (response is OrderTrackingEnrichedDto updatedOrderTracking)
        {
            await kafkaProducerService.SendOrderTrackingUpdateAsync(updatedOrderTracking);
        }
        return await HelperFunctions.GetFinalHttpResponse(response);
    }
}