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
            var failedResponse = new BadRequestObjectResult(new
            {
                Message = "",
                StatusCode = 400
            });
            switch (statusCode)
            {
                case StatusCodesEnum.InvalidEmail:
                    failedResponse.Value = "Invalid email!";
                    failedResponse.StatusCode = 400;
                    return Task.FromResult<IActionResult>(failedResponse);
                    break;
                case StatusCodesEnum.InvalidPassword:
                    failedResponse.Value = "Invalid password!";
                    failedResponse.StatusCode = 400;
                    return Task.FromResult<IActionResult>(new BadRequestObjectResult(new
                    {
                        Message = "Invalid password!",
                        StatusCode = 400
                    }));
                    break;
                case StatusCodesEnum.DeliveryAgentNotFound:
                    failedResponse.Value = "Delivery Agent not found!";
                    failedResponse.StatusCode = 404;
                    return Task.FromResult<IActionResult>(failedResponse);
                    break;
                case StatusCodesEnum.EmailAlreadyExists:
                    failedResponse.Value = "Email already exists!";
                    failedResponse.StatusCode = 409;
                    return Task.FromResult<IActionResult>(failedResponse);
                    break;
                case StatusCodesEnum.DeliveryHubNotFound:
                    failedResponse.Value = "Delivery Hub not found!";
                    failedResponse.StatusCode = 404;
                    return Task.FromResult<IActionResult>(failedResponse);
                    break;
                case StatusCodesEnum.DeliveryHubNameAlreadyExists:
                    failedResponse.Value = "DeliveryHub Name already exists!";
                    failedResponse.StatusCode = 409;
                    return Task.FromResult<IActionResult>(failedResponse);
                    break;
                case StatusCodesEnum.AddressNotFound:
                    failedResponse.Value = "Address not found!";
                    failedResponse.StatusCode = 404;
                    return Task.FromResult<IActionResult>(failedResponse);
                    break;
                case StatusCodesEnum.AuthenticationFailed:
                    failedResponse.Value = "Authentication failed!";
                    failedResponse.StatusCode = 401;
                    return Task.FromResult<IActionResult>(failedResponse);
                    break;
                default:
                    failedResponse.Value = "Unknown error! check logs";
                    failedResponse.StatusCode = 500;
                    return Task.FromResult<IActionResult>(failedResponse);
                    break;
            }
        }

        if (response is bool)
        {
            return Task.FromResult<IActionResult>(new NoContentResult());
        }

        return Task.FromResult<IActionResult>(new OkObjectResult(response));
    }
    
    public static TimeSpan ParseGoogleDuration(string durationText)
    {
        int hours = 0;
        int minutes = 0;
        var parts = durationText.Split(' ');
        for (int i = 0; i < parts.Length; i++)
        {
            if (parts[i].Contains("hour"))
                hours = int.Parse(parts[i - 1]);
            else if (parts[i].Contains("min"))
                minutes = int.Parse(parts[i - 1]);
        }
        return new TimeSpan(hours, minutes, 0);
    }
}