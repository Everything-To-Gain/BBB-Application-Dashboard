using BBB_ApplicationDashboard.Application.Interfaces;
using BBB_ApplicationDashboard.Domain.ValueObjects;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Logging;

namespace BBB_ApplicationDashboard.Infrastructure.Services.Cloudinary;

public class CloudinaryService : ICloudinaryService
{
    private readonly CloudinaryDotNet.Cloudinary _cloudinary;
    private readonly ILogger<CloudinaryService> _logger;

    public CloudinaryService(ISecretService secretService, ILogger<CloudinaryService> logger)
    {
        _logger = logger;

        var cloudName = secretService.GetSecret(
            ProjectSecrets.CloudinaryCloudname,
            Folders.Cloudinary
        );
        var apiKey = secretService.GetSecret(ProjectSecrets.CloudinaryApiKey, Folders.Cloudinary);
        var apiSecret = secretService.GetSecret(
            ProjectSecrets.CloudinaryApiSecret,
            Folders.Cloudinary
        );

        var account = new Account(cloudName, apiKey, apiSecret);
        _cloudinary = new CloudinaryDotNet.Cloudinary(account);
    }

    public async Task<string> UploadBase64ImageAsync(string base64Image, string folder)
    {
        try
        {
            // Remove data URL prefix if present (e.g., "data:image/png;base64,")
            var base64Data = base64Image;
            if (base64Image.Contains(","))
            {
                base64Data = base64Image.Split(',')[1];
            }

            // Convert base64 to byte array
            var imageBytes = Convert.FromBase64String(base64Data);

            using var stream = new MemoryStream(imageBytes);

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(Guid.NewGuid().ToString(), stream),
                Folder = folder,
                Overwrite = false,
                UseFilename = false,
                UniqueFilename = true,
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.Error != null)
            {
                _logger.LogError(
                    "Cloudinary upload failed: {ErrorMessage}",
                    uploadResult.Error.Message
                );
                throw new InvalidOperationException(
                    $"Cloudinary upload failed: {uploadResult.Error.Message}"
                );
            }

            _logger.LogInformation("Image uploaded successfully to Cloudinary: {Url}", uploadResult.SecureUrl);
            return uploadResult.SecureUrl.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading image to Cloudinary");
            throw;
        }
    }
}