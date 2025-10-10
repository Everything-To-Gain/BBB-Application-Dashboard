using BBB_ApplicationDashboard.Application.DTOs.PaginatedDtos;
using BBB_ApplicationDashboard.Application.DTOs.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BBB_ApplicationDashboard.Api.Controllers;

// [Authorize(Policy = "Internal")]
public class UserController(IUserService userService) : CustomControllerBase
{
    [HttpGet("csv-sync")]
    public async Task<IActionResult> GetCSVUsers()
    {
        var csvUsers = await userService.GetAdminDashboardCSVUsers();
        return SuccessResponseWithData(csvUsers);
    }

    [HttpGet("admin-dashboard")]
    public async Task<IActionResult> GetAdminDashboardUsers(
        [FromQuery] UserPaginationRequest request
    )
    {
        var users = await userService.GetAdminDashboardUsers(request);
        return SuccessResponseWithData(users);
    }

    [HttpDelete("admin-dashboard/{id}")]
    public async Task<IActionResult> DeleteAdminDashboardUser(Guid id)
    {
        await userService.DeleteUser(id);
        return SuccessResponse();
    }

    [HttpPost("admin-dashboard")]
    public async Task<IActionResult> CreateAdminDashboardUser(
        AdminDashboardCreateUserRequest request
    )
    {
        await userService.CreateAdminDashboardUser(request);
        return SuccessResponse();
    }

    [HttpPost("admin-dashboard/batch")]
    public async Task<IActionResult> CreateAdminDashboardUsers(
        AdminDashboardUpdateUsersRequest request
    )
    {
        await userService.CreateAdminDashboardUsers(request.UsersCsv);
        return SuccessResponse();
    }

    [HttpPatch("admin-dashboard/{id}")]
    public async Task<IActionResult> UpdateAdminDashboardUser(
        Guid id,
        AdminDashboardUpdateUserRequest request
    )
    {
        await userService.UpdateAdminDashboardUser(id, request);
        return SuccessResponse();
    }
}
