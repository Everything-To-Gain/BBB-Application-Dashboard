using System;
using BBB_ApplicationDashboard.Application.DTOs;
using BBB_ApplicationDashboard.Application.Interfaces;
using BBB_ApplicationDashboard.Domain.Entities;
using BBB_ApplicationDashboard.Infrastructure.Data.Context;
using BBB_ApplicationDashboard.Infrastructure.Exceptions.User;
using Microsoft.EntityFrameworkCore;

namespace BBB_ApplicationDashboard.Infrastructure.Services.User;

public class UserService(ApplicationDbContext context) : IUserService
{
    public async Task<Domain.Entities.User?> FindUser(string email) =>
        await context.Users.FirstOrDefaultAsync(user => user.Email == email);

    public async Task CreateUser(Domain.Entities.User user)
    {
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
    }
}
