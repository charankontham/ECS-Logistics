using ECS_Logistics.DTOs;
using ECS_Logistics.Filters;
using ECS_Logistics.Services;
using ECS_Logistics.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECS_Logistics.Controllers;
[Route("api/DeliveryAgents")]
[ApiController]
[Authorize(Roles = "ROLE_INVENTORY_ADMIN")]
public class DeliveryAgentsController(IDeliveryAgentService service) : ControllerBase
{
        [HttpGet("getAll")]
        public async Task<IActionResult> GetAll([FromBody] DeliveryAgentFilters? filters)
        {
            var agents = await service.GetAllAgentsAsync(filters);
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

    
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            return await HelperFunctions.GetFinalHttpResponse(await service.DeleteAgentAsync(id));
        }
}