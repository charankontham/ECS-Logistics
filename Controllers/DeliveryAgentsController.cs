using ECS_Logistics.DTOs;
using ECS_Logistics.Filters;
using ECS_Logistics.Services;
using ECS_Logistics.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace ECS_Logistics.Controllers;
[Route("api/deliveryAgents")]
[ApiController]
[Authorize(Roles = "ROLE_LOGISTICS_ADMIN")]
public class DeliveryAgentsController(IDeliveryAgentService service) : ControllerBase
{
        [HttpGet]
        public async Task<IActionResult> GetAll([FromBody] DeliveryAgentFilters? filters)
        {
            var agents = await service.GetAllAgentsAsync(filters);
            return await HelperFunctions.GetFinalHttpResponse(agents);
        }
        
        [HttpGet("getAllByPagination")]
        public async Task<IActionResult> GetAllByPagination(
            [FromQuery(Name = "currentPage")] int currentPage, 
            [FromQuery(Name = "offset")] int offset, 
            [FromQuery(Name = "servingArea")] string? servingArea,
            [FromQuery(Name = "availabilityStatus")] string? availabilityStatus,
            [FromQuery(Name = "agentName")] string? agentName,
            [FromQuery(Name = "agentRating")] int? agentRating)
        {
            var filters = ((servingArea is { Length: > 0 } && servingArea.Split(",").Length > 0) ||
                           availabilityStatus is { Length: > 0 } && availabilityStatus.Split(",").Length > 0 ||
                           agentRating != null ||
                           agentName is { Length: > 0 })
                ? new DeliveryAgentFilters()
                {
                    ServingArea = servingArea is {Length: > 0} ? servingArea.Split(",").ToList() : null,
                    Availability = availabilityStatus is { Length: > 0 } ? 
                        availabilityStatus.Split(",").Select(item => int.Parse(item.Trim())).ToList() : null,
                    DeliveryAgentName = agentName is { Length: > 0 } ? agentName : null,
                    Rating = agentRating,
                }
                : null;
            var agents = await service.GetAllByPaginationAsync(currentPage, offset, filters);
            return await HelperFunctions.GetFinalHttpResponse(agents);
        }
    
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var response = await service.GetAgentByIdAsync(id);
                if (response is StatusCodesEnum.DeliveryAgentNotFound)
                {
                    return await HelperFunctions.GetFinalHttpResponse(response);
                }
                return Ok((DeliveryAgentDto)response);
            }
            catch (Exception ex)
            {
                return NotFound("Delivery Agent not found!");
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DeliveryAgentDto agentDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var response = await HelperFunctions.GetFinalHttpResponse(await service.CreateAgentAsync(agentDto));
            return response is OkObjectResult { Value: DeliveryAgentDto createdAgent } ? 
                CreatedAtAction(nameof(GetById), new { id = createdAgent.DeliveryAgentId }, createdAgent) : 
                response;
        }
        
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] DeliveryAgentDto agentDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            agentDto.Password = "";
            return await HelperFunctions.GetFinalHttpResponse(await service.UpdateAgentAsync(agentDto));
        }

        [HttpPut("updatePassword")]
        public async Task<IActionResult> UpdatePassword([FromBody] JObject passwordBlock)
        {
            var deliveryAgentId = passwordBlock["deliveryAgentId"]?.Value<int>();
            var oldPassword = passwordBlock["oldPassword"]?.ToString();
            var newPassword = passwordBlock["newPassword"]?.ToString();
            
            Console.WriteLine("passBlock : "+ passwordBlock);
            if (deliveryAgentId != null && oldPassword != null && oldPassword.Trim()!="" &&  
                newPassword != null && newPassword.Trim()!="")
            {
                return await HelperFunctions.GetFinalHttpResponse(
                    await service.UpdateAgentPassword(
                        deliveryAgentId ?? 0, 
                        oldPassword, 
                        newPassword)
                    );
            }
            return BadRequest("Schema validation failed!");
        }

    
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            return await HelperFunctions.GetFinalHttpResponse(await service.DeleteAgentAsync(id));
        }
}