using BBB_ApplicationDashboard.Application.DTOs;

namespace BBB_ApplicationDashboard.Application.Interfaces;

public interface IMainServerClient
{
    public Task<string> SendFormData(SubmittedDataRequest submittedData, String internalAppId);
}
