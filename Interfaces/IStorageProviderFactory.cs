using SharedFolderAPI.Services;

namespace SharedFolderAPI.Interfaces
{
    public interface IStorageProviderFactory
    {
        IFileStorageProvider CreateProvider(string providerType);
    }

    public class StorageProviderFactory : IStorageProviderFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public StorageProviderFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IFileStorageProvider CreateProvider(string providerType)
        {
            return providerType switch
            {
                "SMB" => _serviceProvider.GetService<SmbFileStorageProvider>(),
                "AWS" => _serviceProvider.GetService<AwsS3FileStorageProvider>(),
                "Azure" => _serviceProvider.GetService<AzureBlobStorageProvider>(),
                _ => throw new ArgumentException("Invalid provider type", nameof(providerType)),
            };
        }
    }
}
