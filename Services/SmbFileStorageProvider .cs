using SharedFolderAPI.Interfaces;
using SharpCifs.Smb;

namespace SharedFolderAPI.Services
{
    public class SmbFileStorageProvider : IFileStorageProvider
    {
        private readonly string _baseUrl;
        private readonly string _username;
        private readonly string _password;

        public SmbFileStorageProvider(string baseUrl, string username, string password)
        {
            _baseUrl = baseUrl;
            _username = username;
            _password = password;
        }

        private SmbFile GetSmbFile(string path)
        {
            var url = $"{_baseUrl}/{path}".Replace("\\", "/");
            var auth = new NtlmPasswordAuthentication(string.Empty, _username, _password);
            return new SmbFile(url, auth);
        }

        public async Task CreateFolderAsync(string folderName)
        {
            var folder = GetSmbFile(folderName);
            if (!folder.Exists())
            {
                folder.Mkdir();
            }
            await Task.CompletedTask;
        }

        //public async Task<IEnumerable<string>> ListFoldersAsync()
        //{
        //    var root = GetSmbFile(string.Empty);
        //    return root.ListFiles().Where(f => f.IsDirectory()).Select(f => f.GetName()).ToList();
        //}

        public async Task<IEnumerable<string>> ListFoldersAsync(string path = "")
        {
            var root = GetSmbFile(path);
            return root.ListFiles().Where(f => f.IsDirectory()).Select(f => f.GetName()).ToList();
        }

        public async Task DeleteFolderAsync(string folderName)
        {
            var folder = GetSmbFile(folderName);
            if (folder.Exists())
            {
                folder.Delete();
            }
            await Task.CompletedTask;
        }

        public async Task UploadFileAsync(string folderName, string fileName, Stream fileStream)
        {
            var file = GetSmbFile(Path.Combine(folderName, fileName));
            using (var outputStream = file.GetOutputStream())
            {
                await fileStream.CopyToAsync(outputStream);
            }
        }

        public async Task<IEnumerable<string>> ListFilesAsync(string folderName)
        {
            var folder = GetSmbFile(folderName);
            return folder.ListFiles().Where(f => !f.IsDirectory()).Select(f => f.GetName()).ToList();
        }

        public async Task<Stream> GetFileAsync(string folderName, string fileName)
        {
            var file = GetSmbFile(Path.Combine(folderName, fileName));
            var memoryStream = new MemoryStream();
            using (var inputStream = file.GetInputStream())
            {
                inputStream.CopyTo(memoryStream);
            }
            memoryStream.Position = 0;
            return await Task.FromResult(memoryStream);
        }


        public async Task<string> GetFileUrlAsync(string folderName, string fileName)
        {
            SmbFile file;
            if (folderName == null || folderName == "")
            {
                file = GetSmbFile(Path.Combine(fileName));
                return file.GetPath();
            }
            else
            {
                file = GetSmbFile(Path.Combine(folderName, fileName));
                return file.GetPath();
            }
        }

        public async Task DeleteFileAsync(string folderName, string fileName)
        {
            SmbFile file;
            if (folderName==null || folderName=="")
            {
                file = GetSmbFile(Path.Combine(fileName));
            }
            else
            {
                file = GetSmbFile(Path.Combine(folderName, fileName));
            }

            if (file.Exists())
            {
                file.Delete();
            }
            await Task.CompletedTask;
        }
    }

    public static class InputStreamExtensions
    {
        public static void CopyTo(this SharpCifs.Util.Sharpen.InputStream input, Stream output)
        {
            byte[] buffer = new byte[81920]; // 80 KB buffer
            int bytesRead;
            while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, bytesRead);
            }
        }
    }
}
