using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace ST10339829_CLDV6212_POE_FINAL.Services
{
    public class AzureBlobService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly BlobContainerClient _blobContainerClient;

        public AzureBlobService(string connectionString)
        {
            _blobServiceClient = new BlobServiceClient(connectionString);
            _blobContainerClient = _blobServiceClient.GetBlobContainerClient("images");

            // Create the container if it doesn't exist
            _blobContainerClient.CreateIfNotExists(PublicAccessType.Blob);
        }

        // Upload the image asynchronously
        public async Task UploadImageAsync(IFormFile file)
        {
            var imageBlobClient = _blobContainerClient.GetBlobClient(file.FileName);

            // Open a stream for the file and upload it
            using var fileStream = file.OpenReadStream();
            var blobHttpHeaders = new BlobHttpHeaders
            {
                ContentType = file.ContentType // Set the content type
            };

            await imageBlobClient.UploadAsync(fileStream, new BlobUploadOptions { HttpHeaders = blobHttpHeaders });
        }

        // List all files in the container asynchronously
        public async Task<List<string>> GetFilesAsync()
        {
            var files = new List<string>();

            // List all blobs in the container
            await foreach (var blobItem in _blobContainerClient.GetBlobsAsync())
            {
                files.Add(blobItem.Name);
            }

            return files;
        }
    }
}
