using ECS_Logistics.DTOs;
using ECS_Logistics.Filters;
using ECS_Logistics.Services;
using ECS_Logistics.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECS_Logistics.Controllers;
[Route("api/orderReturns")]
[ApiController]
[Authorize(Roles = "ROLE_LOGISTICS_ADMIN, ROLE_CUSTOMER")]
public class OrderReturnsController(IOrderReturnService orderReturnService) : ControllerBase
{
    [HttpGet("getAll")]
    public async Task<IActionResult> GetAllAsync(OrderReturnFilters? orderReturnFilters)
    {
        var orderReturns = await orderReturnService.GetAllReturnsAsync(orderReturnFilters);
        return await HelperFunctions.GetFinalHttpResponse(orderReturns);
    }
    
    [HttpGet("getAllByPagination")]
    public async Task<IActionResult> GetAllByPaginationAsync(
        [FromQuery(Name = "currentPage")] int currentPage, 
        [FromQuery(Name = "offset")] int offset, 
        [FromQuery(Name = "fromDate")] DateTime? fromDate,
        [FromQuery(Name = "toDate")] DateTime? toDate, 
        [FromQuery(Name = "productId")] int? productId, 
        [FromQuery(Name = "categoryId")] int? categoryId,
        [FromQuery(Name = "subCategoryId")] int? subCategoryId,
        [FromQuery(Name = "brandId")] int? brandId,
        [FromQuery(Name = "returnReasonCategoryId")] int? returnReasonCategoryId)
    {
        var filters = (fromDate != null || toDate != null || productId != null || categoryId != null ||
                       subCategoryId != null || brandId != null || returnReasonCategoryId != null)
            ? new OrderReturnFilters
            {
                FromDate = fromDate,
                ToDate = toDate,
                ProductId = productId,
                CategoryId = categoryId,
                SubCategoryId = subCategoryId,
                BrandId = brandId,
                ReturnReasonCategoryId = returnReasonCategoryId
            }
            : null;
        var orderReturns = await orderReturnService.GetAllByPaginationAsync(currentPage, offset, filters);
        return await HelperFunctions.GetFinalHttpResponse(orderReturns);
        
    }
    
    [HttpGet("getAllOrderReturnsByCustomerId/{agentId:int}")]
    public async Task<IActionResult> GetAll(int customerId)
    {
        var orderReturns = await orderReturnService.GetAllByCustomerIdAsync(customerId);
        return await HelperFunctions.GetFinalHttpResponse(orderReturns);
    }
    
    [HttpGet("{orderReturnId:int}")]
    public async Task<IActionResult> GetById(int orderReturnId)
    {
        try
        {
            var response = await orderReturnService.GetOrderReturnByIdAsync(orderReturnId);
            return await HelperFunctions.GetFinalHttpResponse(response);
        }
        catch (Exception ex)
        {
            return NotFound("OrderReturn not found!");
        }
    }
        
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] OrderReturnDto orderReturnDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var response = await HelperFunctions.GetFinalHttpResponse(
            await orderReturnService.CreateReturnAsync(orderReturnDto));
        return response is OkObjectResult { Value: OrderReturnEnrichedDto createdOrderReturn } ? 
            CreatedAtAction(nameof(GetById), new
            {
                id = createdOrderReturn.OrderReturnId
            }, createdOrderReturn) : response;
    }
        
    [HttpDelete("{orderReturnId:int}")]
    public async Task<IActionResult> Delete(int orderReturnId)
    {
        return await HelperFunctions.GetFinalHttpResponse(await orderReturnService.DeleteOrderReturnAsync(orderReturnId));
    }
}