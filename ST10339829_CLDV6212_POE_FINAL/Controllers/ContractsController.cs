using Microsoft.AspNetCore.Mvc;

namespace ST10339829_CLDV6212_POE_FINAL.Controllers
{
    public class ContractsController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _azureFunctionBaseUrl;
        private readonly string _uploadContractFunctionKey;

        public ContractsController(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _azureFunctionBaseUrl = configuration["AzureSettings:BaseURL"];
            _uploadContractFunctionKey = configuration["AzureSettings:UploadContractKey"];
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile formFile)
        {
            if (formFile != null && formFile.Length > 0)
            {
                var result = await UploadContractToFunctionAsync(formFile);
                if (result.IsSuccessStatusCode)
                {
                    TempData["Message"] = "Contract uploaded successfully!";
                }
                else
                {
                    TempData["Error"] = "Failed to upload contract.";
                }
            }
            else
            {
                TempData["Error"] = "Please select a file to upload.";
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task<HttpResponseMessage> UploadContractToFunctionAsync(IFormFile formFile)
        {
            var requestUri = $"{_azureFunctionBaseUrl}UploadContract?code={_uploadContractFunctionKey}";
            using var content = new MultipartFormDataContent();
            using var fileStream = formFile.OpenReadStream();
            using var fileContent = new StreamContent(fileStream);
            content.Add(fileContent, "file", Path.GetFileName(formFile.FileName));

            return await _httpClient.PostAsync(requestUri, content);
        }
    }
}
