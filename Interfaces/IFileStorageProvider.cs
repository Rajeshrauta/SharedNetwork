namespace SharedFolderAPI.Interfaces
{
    public interface IFileStorageProvider
    {
        Task CreateFolderAsync(string folderName);
        Task<IEnumerable<string>> ListFoldersAsync();
        Task DeleteFolderAsync(string folderName);

        Task UploadFileAsync(string folderName, string fileName, Stream fileStream);
        Task<IEnumerable<string>> ListFilesAsync(string folderName);
        Task<Stream> GetFileAsync(string folderName, string fileName);
        Task<string> GetFileUrlAsync(string folderName, string fileName);
        Task DeleteFileAsync(string folderName, string fileName);
    }
}
