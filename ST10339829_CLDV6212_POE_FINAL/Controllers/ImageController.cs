using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ST10339829_CLDV6212_POE_FINAL.Controllers
{
    public class ImageController : Controller
    {
        private readonly string _azureFunctionBaseUrl;
        private readonly string _uploadImageKey;
        private readonly string _getImageKey;
        private readonly HttpClient _httpClient;
        private readonly ILogger<ImageController> _logger;

        public ImageController(HttpClient httpClient, IConfiguration configuration, ILogger<ImageController> logger)
        {
            _httpClient = httpClient;
            _azureFunctionBaseUrl = configuration["AzureSettings:BaseURL"];
            _uploadImageKey = configuration["AzureSettings:UploadImageKey"];
            _getImageKey = configuration["AzureSettings:GetImageURLKey"];
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var files = await GetImageUrlsAsync();
                ViewBag.ImageUrls = files;
                return View(files);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to load images: {ex.Message}");
                TempData["Error"] = "Failed to load images. Please try again later.";
                return View(new List<string>());
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadImageIndex(IFormFile formFile)
        {
            if (formFile != null && formFile.Length > 0)
            {
                try
                {
                    var formFileName = Path.GetFileName(formFile.FileName);
                    var url = await UploadImageToFunctionAsync(formFile, formFileName);
                    TempData["Message"] = "Your image has been successfully uploaded!";
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Failed to upload image: {ex.Message}");
                    TempData["Error"] = "Failed to upload image. Please try again.";
                }
            }
            else
            {
                TempData["Error"] = "Please select an image to upload.";
            }
            return RedirectToAction("Index");
        }

        public async Task<string> UploadImageToFunctionAsync(IFormFile image, string imageName)
        {
            var uploadUrl = $"{_azureFunctionBaseUrl}upload-image?code={_uploadImageKey}";
            _logger.LogInformation($"Uploading image to: {uploadUrl}");

            using (var stream = image.OpenReadStream())
            {
                var content = new MultipartFormDataContent();
                var streamContent = new StreamContent(stream);
                content.Add(streamContent, "file", imageName);

                var response = await _httpClient.PostAsync(uploadUrl, content);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"Image upload response: {result}");
                return result;
            }
        }

        public async Task<List<string>> GetImageUrlsAsync()
        {
            var getUrl = $"{_azureFunctionBaseUrl}get-image-urls?code={_getImageKey}";
            _logger.LogInformation($"Fetching URLs from: {getUrl}");

            var response = await _httpClient.GetAsync(getUrl);
            response.EnsureSuccessStatusCode();

            var jsonResult = await response.Content.ReadAsStringAsync();
            _logger.LogInformation($"Fetched image URLs: {jsonResult}");

            return JsonSerializer.Deserialize<List<string>>(jsonResult) ?? new List<string>();
        }
    }
}
