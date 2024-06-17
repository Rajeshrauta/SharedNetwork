using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using SharedFolderAPI.Interfaces;

namespace SharedFolderAPI.Services
{
    public class AzureBlobStorageProvider : IFileStorageProvider
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly BlobContainerClient _containerClient;

        public AzureBlobStorageProvider(string connectionString, string containerName)
        {
            _blobServiceClient = new BlobServiceClient(connectionString);
            _containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            _containerClient.CreateIfNotExists(PublicAccessType.BlobContainer);
        }

        public async Task CreateFolderAsync(string folderName)
        {
            // Azure Blob Storage does not have a concept of folders; you can create a zero-byte blob to represent a folder
            var blobClient = _containerClient.GetBlobClient($"{folderName}/");
            using var stream = new MemoryStream();
            await blobClient.UploadAsync(stream);
        }

        public async Task<IEnumerable<string>> ListFoldersAsync()
        {
            var folders = new HashSet<string>();
            await foreach (BlobItem blobItem in _containerClient.GetBlobsAsync())
            {
                var folderName = blobItem.Name.Split('/')[0];
                folders.Add(folderName);
            }
            return folders;
        }

        public async Task DeleteFolderAsync(string folderName)
        {
            await foreach (BlobItem blobItem in _containerClient.GetBlobsAsync(prefix: $"{folderName}/"))
            {
                var blobClient = _containerClient.GetBlobClient(blobItem.Name);
                await blobClient.DeleteAsync();
            }
        }

        public async Task UploadFileAsync(string folderName, string fileName, Stream fileStream)
        {
            var blobClient = _containerClient.GetBlobClient($"{folderName}/{fileName}");
            await blobClient.UploadAsync(fileStream, true);
        }

        public async Task<IEnumerable<string>> ListFilesAsync(string folderName)
        {
            var files = new List<string>();
            await foreach (BlobItem blobItem in _containerClient.GetBlobsAsync(prefix: $"{folderName}/"))
            {
                files.Add(blobItem.Name);
            }
            return files;
        }

        public async Task<Stream> GetFileAsync(string folderName, string fileName)
        {
            var blobClient = _containerClient.GetBlobClient($"{folderName}/{fileName}");
            var response = await blobClient.DownloadAsync();
            return response.Value.Content;
        }

        public async Task<string> GetFileUrlAsync(string folderName, string fileName)
        {
            var blobClient = _containerClient.GetBlobClient($"{folderName}/{fileName}");
            return blobClient.Uri.AbsoluteUri;
        }

        public async Task DeleteFileAsync(string folderName, string fileName)
        {
            var blobClient = _containerClient.GetBlobClient($"{folderName}/{fileName}");
            await blobClient.DeleteAsync();
        }
    }
}
