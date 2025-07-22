using ECS_Logistics.DTOs;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ECS_Logistics.Utils;

public class HelperFunctions
{
    public static Task<IActionResult> GetFinalHttpResponse(object response)
    {
        if (response is StatusCodesEnum statusCode)
        {
            switch (statusCode)
            {
                case StatusCodesEnum.InvalidEmail:
                    return Task.FromResult<IActionResult>(new BadRequestObjectResult(new
                    {
                        Message = "Invalid Email!",
                        StatusCode = 400
                    }));
                    break;
                case StatusCodesEnum.InvalidPassword:
                    return Task.FromResult<IActionResult>(new BadRequestObjectResult(new
                    {
                        Message = "Invalid password!",
                        StatusCode = 400
                    }));
                    break;
                case StatusCodesEnum.DeliveryAgentNotFound:
                    return Task.FromResult<IActionResult>(new NotFoundObjectResult(new
                    {
                        Message = "Delivery Agent not found!",
                        StatusCode = 400
                    }));
                    break;
                case StatusCodesEnum.EmailAlreadyExists:
                    return Task.FromResult<IActionResult>(new BadRequestObjectResult(new
                    {
                        Message = "Email already exists!",
                        StatusCode = 409
                    }));
                    break;
                default:
                    return Task.FromResult<IActionResult>(new BadRequestObjectResult(new
                    {
                        Message = "Unknown error!",
                        StatusCode = 400
                    }));
                    break;
            }
        }

        if (response is bool)
        {
            return Task.FromResult<IActionResult>(new NoContentResult());
        }

        return Task.FromResult<IActionResult>(new OkObjectResult(response));
    }
}