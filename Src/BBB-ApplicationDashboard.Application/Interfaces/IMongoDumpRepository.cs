namespace BBB_ApplicationDashboard.Application;

public interface IMongoDumpRepository
{
    Task InsertAsync(Dictionary<string, object> payload);
    Task<List<Dictionary<string, object>>> GetAllAsync();
}
