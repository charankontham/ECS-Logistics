using ECS_Logistics.DTOs;
using ECS_Logistics.Filters;
using ECS_Logistics.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECS_Logistics.Controllers;

public class DeliveryAgentsController(IDeliveryAgentService service) : ControllerBase
{
        [HttpGet]
        public async Task<IActionResult> GetAll([FromBody] DeliveryAgentFilters filters)
        {
            var agents = await service.GetAllAgentsAsync(filters);
            return Ok(agents);
        }
    
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var agent = await service.GetAgentByIdAsync(id);
                return Ok(agent);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DeliveryAgentDto agentDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var createdAgent = await service.CreateAgentAsync(agentDto);
            return CreatedAtAction(nameof(GetById), new { id = createdAgent.DeliveryAgentId }, createdAgent);
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] DeliveryAgentDto agentDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != agentDto.DeliveryAgentId) return BadRequest("ID mismatch");
            try
            {
                var updatedAgentDto = await service.UpdateAgentAsync(id, agentDto);
                return Ok(updatedAgentDto);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

    
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await service.DeleteAgentAsync(id);
                if (response) return NoContent();
                else return NotFound();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
}