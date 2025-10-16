namespace BBB_ApplicationDashboard.Application.Interfaces;

public interface ICloudinaryService
{
    Task<string> UploadBase64ImageAsync(string base64Image, string folder);
}