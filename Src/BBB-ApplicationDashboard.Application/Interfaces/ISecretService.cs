using BBB_ApplicationDashboard.Domain.ValueObjects;

namespace BBB_ApplicationDashboard.Application.Interfaces;

public interface ISecretService
{
    string GetSecret(ProjectSecrets name, Folders folder);
}
