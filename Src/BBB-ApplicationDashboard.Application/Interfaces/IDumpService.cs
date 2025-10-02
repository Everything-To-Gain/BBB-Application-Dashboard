namespace BBB_ApplicationDashboard.Application;

public interface IDumpService
{
    Task DumpIntoMongo(Dictionary<string, object> payload);
    Task<IEnumerable<Dictionary<string, object>>> GetAllAsync();
}
