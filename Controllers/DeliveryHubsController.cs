using ECS_Logistics.DTOs;
using ECS_Logistics.Filters;
using ECS_Logistics.Services;
using ECS_Logistics.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ECS_Logistics.Controllers;
[Route("api/deliveryHubs")]
[ApiController]
[Authorize(Roles = "ROLE_LOGISTICS_ADMIN")]
public class DeliveryHubsController(IDeliveryHubService service) : ControllerBase
{
    [HttpGet("getAll")]
    public async Task<IActionResult> GetAll([FromBody] DeliveryHubFilters? filters)
    {
        var hubs = await service.GetAllHubsAsync(filters);
        return await HelperFunctions.GetFinalHttpResponse(hubs);
    }
    
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var hub = await service.GetHubByIdAsync(id);
            if (hub is StatusCodesEnum.DeliveryHubNotFound)
            {
                return await HelperFunctions.GetFinalHttpResponse(hub);
            }
            return Ok(hub);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
        
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] DeliveryHubDto hubDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var response = await HelperFunctions.GetFinalHttpResponse(await service.CreateHubAsync(hubDto));
        return response is OkObjectResult { Value: DeliveryHubEnrichedDto createdHubDto } ? 
            CreatedAtAction(nameof(GetById), new { id = createdHubDto.DeliveryHubId }, createdHubDto) : 
            response;
    }
        
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] DeliveryHubDto hubDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        return await HelperFunctions.GetFinalHttpResponse(await service.UpdateHubAsync(hubDto));
    }

    
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        return await HelperFunctions.GetFinalHttpResponse(await service.DeleteHubAsync(id));
    }
}