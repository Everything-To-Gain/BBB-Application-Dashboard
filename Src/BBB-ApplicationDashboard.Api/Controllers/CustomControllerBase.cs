using Microsoft.AspNetCore.Mvc;

namespace BBB_ApplicationDashboard.Api.Controllers;

public class ApiResponse<T>
{
    public bool Success { get; init; }
    public string? Message { get; init; }
    public T? Data { get; init; }
    public object? Errors { get; init; }
}

[ApiController]
[Route("api/[controller]")]
public class CustomControllerBase : ControllerBase
{
    protected IActionResult SucessResponse<T>(
        T? data,
        string? message = "The operation done successfully"
    )
    {
        return Ok(
            new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data,
            }
        );
    }

    protected IActionResult ErrorResponse<T>(
        T data,
        string? message = "Something went wrong during this operation"
    )
    {
        return Ok(
            new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Data = data,
            }
        );
    }
}
