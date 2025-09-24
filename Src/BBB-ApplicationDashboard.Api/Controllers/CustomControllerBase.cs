using BBB_ApplicationDashboard.Application.DTOs.Common;
using Microsoft.AspNetCore.Mvc;

namespace BBB_ApplicationDashboard.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomControllerBase : ControllerBase
{
    protected IActionResult SucessResponseWithData<T>(
        T? data,
        string? message = "The operation done successfully"
    )
    {
        return Ok(
            new APIResponse<T>
            {
                Success = true,
                Message = message,
                Data = data,
            }
        );
    }

    protected IActionResult ErrorResponseWithData<T>(
        T data,
        string? message = "Something went wrong during this operation"
    )
    {
        return Ok(
            new APIResponse<T>
            {
                Success = false,
                Message = message,
                Data = data,
            }
        );
    }

    protected IActionResult SucessResponse(string? message = "The operation done successfully")
    {
        return Ok(
            new APIResponse<object>
            {
                Success = true,
                Message = message,
                Data = null,
            }
        );
    }

    protected IActionResult ErrorResponse(
        string? message = "Something went wrong during this operation"
    )
    {
        return Ok(
            new APIResponse<object>
            {
                Success = false,
                Message = message,
                Data = null,
            }
        );
    }
}
