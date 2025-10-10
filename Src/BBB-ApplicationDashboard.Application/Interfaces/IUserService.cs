using System;
using BBB_ApplicationDashboard.Application.DTOs.PaginatedDtos;
using BBB_ApplicationDashboard.Application.DTOs.User;
using BBB_ApplicationDashboard.Domain.Entities;

namespace BBB_ApplicationDashboard.Application.Interfaces;

public interface IUserService
{
    Task<User?> FindUser(string email);
    Task CreateUser(User user);

    Task<PaginatedResponse<AdminDashboardUserResponse>> GetAdminDashboardUsers(
        UserPaginationRequest request
    );

    Task DeleteUser(Guid id);
    Task CreateAdminDashboardUser(AdminDashboardCreateUserRequest request);
    Task CreateAdminDashboardUsers(string usersCsv);
    Task UpdateAdminDashboardUser(Guid id, AdminDashboardUpdateUserRequest request);
    Task<List<string>> GetAdminDashboardCsvUsers();
}