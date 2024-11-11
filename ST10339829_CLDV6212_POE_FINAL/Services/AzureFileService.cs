using Azure.Storage.Files.Shares;

namespace ST10339829_CLDV6212_POE_FINAL.Services
{
    public class AzureFileService
    {
        private readonly ShareClient _share;

        public AzureFileService(string connectionString)
        {
            _share = new ShareClient(connectionString, "contracts");
            _share.CreateIfNotExists();
        }

        public async Task UploadAsync(IFormFile formFile)
        {
            var directory = _share.GetRootDirectoryClient();
            var fileClient = directory.GetFileClient(formFile.FileName);

            using var readStream = formFile.OpenReadStream();

            await fileClient.CreateAsync(readStream.Length);

            await fileClient.UploadAsync(readStream);
        }

        public async Task<List<string>> FilesAsync()
        {
            var files = new List<string>();
            var directory = _share.GetRootDirectoryClient();
            await foreach (var item in directory.GetFilesAndDirectoriesAsync())
            {
                files.Add(item.Name);
            }
            return files;
        }
    }
}
