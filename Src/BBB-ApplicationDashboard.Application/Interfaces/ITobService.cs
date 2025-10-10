using BBB_ApplicationDashboard.Domain.Entities;

namespace BBB_ApplicationDashboard.Application.Interfaces;

public interface ITobService
{
    public Task<List<Tob>> GetToBs(string? searchTerm);
}