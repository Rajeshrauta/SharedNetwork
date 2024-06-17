using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedFolderAPI.Interfaces;

namespace SharedFolderAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IStorageProviderFactory _storageProviderFactory;

        public FilesController(IStorageProviderFactory storageProviderFactory)
        {
            _storageProviderFactory = storageProviderFactory;
        }

        [HttpPost("CreateFolder/{providerType}/folders")]
        public async Task<IActionResult> CreateFolder(string providerType, [FromQuery] string folderName)
        {
            var provider = _storageProviderFactory.CreateProvider(providerType);
            await provider.CreateFolderAsync(folderName);
            return Ok();
        }

        [HttpGet("ListFolders/{providerType}/folders")]
        public async Task<IActionResult> ListFolders(string providerType)
        {
            var provider = _storageProviderFactory.CreateProvider(providerType);
            var folders = await provider.ListFoldersAsync();
            return Ok(folders);
        }

        [HttpDelete("DeleteFolder/{providerType}/folders")]
        public async Task<IActionResult> DeleteFolder(string providerType, [FromQuery] string folderName)
        {
            var provider = _storageProviderFactory.CreateProvider(providerType);
            await provider.DeleteFolderAsync(folderName);
            return Ok();
        }

        [HttpPost("UploadFile/{providerType}/files")]
        public async Task<IActionResult> UploadFile(string providerType, [FromQuery] string folderName, IFormFile file)
        {
            var provider = _storageProviderFactory.CreateProvider(providerType);
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Position = 0;
                await provider.UploadFileAsync(folderName, file.FileName, stream);
            }
            return Ok();
        }

        [HttpGet("ListFiles/{providerType}/files")]
        public async Task<IActionResult> ListFiles(string providerType, [FromQuery] string folderName)
        {
            var provider = _storageProviderFactory.CreateProvider(providerType);
            var files = await provider.ListFilesAsync(folderName);
            return Ok(files);
        }

        [HttpGet("GetFile/{providerType}/files/{fileName}")]
        public async Task<IActionResult> GetFile(string providerType, string folderName, string fileName)
        {
            var provider = _storageProviderFactory.CreateProvider(providerType);
            var stream = await provider.GetFileAsync(folderName, fileName);
            return File(stream, "application/octet-stream", fileName);
        }

        [HttpGet("GetFileUrl/{providerType}/files/{fileName}/url")]
        public async Task<IActionResult> GetFileUrl(string providerType, string folderName, string fileName)
        {
            var provider = _storageProviderFactory.CreateProvider(providerType);
            var fileUrl = await provider.GetFileUrlAsync(folderName, fileName);
            fileUrl = fileUrl.Replace("smb://", "file://");
            return Ok(fileUrl);
        }

        [HttpDelete("DeleteFile/{providerType}/files")]
        public async Task<IActionResult> DeleteFile(string providerType, [FromQuery] string folderName, [FromQuery] string fileName)
        {
            var provider = _storageProviderFactory.CreateProvider(providerType);
            await provider.DeleteFileAsync(folderName, fileName);
            return Ok();
        }
    }
}
