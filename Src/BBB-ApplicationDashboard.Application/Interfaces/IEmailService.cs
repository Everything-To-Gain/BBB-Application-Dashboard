using BBB_ApplicationDashboard.Application.DTOs;

namespace BBB_ApplicationDashboard.Application.Interfaces;

public interface IEmailService
{
    public Task<bool> SendAsync(EmailMessage message, CancellationToken cancellationToken);
}
