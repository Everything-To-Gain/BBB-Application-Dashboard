using System;
using BBB_ApplicationDashboard.Domain.Entities;

namespace BBB_ApplicationDashboard.Application.Interfaces;

public interface IUserService
{
    Task<User?> FindUser(string email);
    Task CreateUser(User user);
}
