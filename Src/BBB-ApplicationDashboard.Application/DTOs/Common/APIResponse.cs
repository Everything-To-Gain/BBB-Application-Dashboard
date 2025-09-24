using System;

namespace BBB_ApplicationDashboard.Application.DTOs.Common;

public class APIResponse<T>
{
    public bool Success { get; set; } // Did the request succeed?
    public string? Message { get; set; } // Optional message (success or error)
    public T? Data { get; set; } // The actual payload (flexible)
    public List<string> Errors { get; set; } = []; // Detailed error list
}
