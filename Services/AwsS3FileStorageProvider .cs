using Amazon.S3;
using Amazon.S3.Model;
using SharedFolderAPI.Interfaces;

namespace SharedFolderAPI.Services
{
    public class AwsS3FileStorageProvider : IFileStorageProvider
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;

        public AwsS3FileStorageProvider(IAmazonS3 s3Client, string bucketName)
        {
            _s3Client = s3Client;
            _bucketName = bucketName;
        }

        public async Task CreateFolderAsync(string folderName)
        {
            // S3 does not have folders, just use a key prefix
            await Task.CompletedTask;
        }

        public async Task<IEnumerable<string>> ListFoldersAsync(string path = "")
        {
            throw new NotImplementedException();
        }

        public async Task DeleteFolderAsync(string folderName)
        {
            // Implement folder deletion logic using AWS SDK
        }

        public async Task UploadFileAsync(string folderName, string fileName, Stream fileStream)
        {
            var key = $"{folderName}/{fileName}";
            var request = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = key,
                InputStream = fileStream
            };
            await _s3Client.PutObjectAsync(request);
        }

        public async Task<IEnumerable<string>> ListFilesAsync(string folderName)
        {
            throw new NotImplementedException();
        }

        public async Task<Stream> GetFileAsync(string folderName, string fileName)
        {
            var key = $"{folderName}/{fileName}";
            var response = await _s3Client.GetObjectAsync(_bucketName, key);
            return response.ResponseStream;
        }

        public async Task<string> GetFileUrlAsync(string folderName, string fileName)
        {
            var key = $"{folderName}/{fileName}";
            var url = _s3Client.GetPreSignedURL(new GetPreSignedUrlRequest
            {
                BucketName = _bucketName,
                Key = key,
                Expires = DateTime.Now.AddMinutes(30)
            });
            return url;
        }

        public async Task DeleteFileAsync(string folderName, string fileName)
        {
            var key = $"{folderName}/{fileName}";
            await _s3Client.DeleteObjectAsync(_bucketName, key);
        }
    }
}
