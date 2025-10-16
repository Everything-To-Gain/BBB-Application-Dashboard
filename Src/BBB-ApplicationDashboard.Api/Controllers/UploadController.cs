using Microsoft.AspNetCore.Mvc;

namespace BBB_ApplicationDashboard.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController(ICloudinaryService cloudinaryService) : CustomControllerBase
    {
        [HttpPost("image")]
        public async Task<IActionResult> UploadImage([FromBody] UploadImageRequest request)
        {
            var imageUrl = await cloudinaryService.UploadBase64ImageAsync(
                request.Base64Image,
                request.Folder
            );

            return SuccessResponseWithData(
                new { ImageUrl = imageUrl },
                "Image uploaded successfully to Cloudinary."
            );
        }
    }
}