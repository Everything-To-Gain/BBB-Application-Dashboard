namespace BBB_ApplicationDashboard.Application;

public interface IDumpService
{
    Task DumpIntoMongo(Dictionary<string, object> payloads, string token);
    Task<IEnumerable<Dictionary<string, object>>> GetAllAsync();
}
