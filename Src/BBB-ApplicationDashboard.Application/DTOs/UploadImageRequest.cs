namespace BBB_ApplicationDashboard.Api.Controllers;

public class UploadImageRequest
{
    public string Base64Image { get; set; } = string.Empty;
    public string Folder { get; set; } = string.Empty;
}