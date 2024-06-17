using Amazon.S3;
using Microsoft.Extensions.Options;
using SharedFolderAPI.Interfaces;
using SharedFolderAPI.Models;
using SharedFolderAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.Configure<SmbSettings>(builder.Configuration.GetSection("SmbSettings"));
builder.Services.Configure<AwsS3Settings>(builder.Configuration.GetSection("AwsS3Settings"));
builder.Services.Configure<AzureBlobSettings>(builder.Configuration.GetSection("AzureBlobSettings"));

builder.Services.AddSingleton<SmbFileStorageProvider>(provider =>
{
    var smbSettings = provider.GetRequiredService<IOptions<SmbSettings>>().Value;
    return new SmbFileStorageProvider(smbSettings.Url, smbSettings.Username, smbSettings.Password);
});

// Register AWS S3 file storage provider
builder.Services.AddAWSService<IAmazonS3>();
builder.Services.AddSingleton<AwsS3FileStorageProvider>(provider =>
{
    var s3Client = provider.GetRequiredService<IAmazonS3>();
    var s3Settings = provider.GetRequiredService<IOptions<AwsS3Settings>>().Value;
    return new AwsS3FileStorageProvider(s3Client, s3Settings.BucketName);
});

// Register Azure Blob Storage provider
builder.Services.AddSingleton<AzureBlobStorageProvider>(provider =>
{
    var azureSettings = provider.GetRequiredService<IOptions<AzureBlobSettings>>().Value;
    return new AzureBlobStorageProvider(azureSettings.ConnectionString, azureSettings.ContainerName);
});

// Register the storage provider factory
builder.Services.AddSingleton<IStorageProviderFactory, StorageProviderFactory>();


//builder.Services.AddSingleton<SmbFileStorageProvider>(provider =>
//        new SmbFileStorageProvider("smb://192.168.50.187/SharedFolder", "Rajesh Rauta", "Logyscal@5"));

//// Register AWS S3 file storage provider
//builder.Services.AddAWSService<IAmazonS3>();
//builder.Services.AddSingleton<AwsS3FileStorageProvider>(provider =>
//    new AwsS3FileStorageProvider(provider.GetRequiredService<IAmazonS3>(), "bucketName"));

//// Register Azure Blob Storage provider
//builder.Services.AddSingleton<AzureBlobStorageProvider>(provider =>
//    new AzureBlobStorageProvider("connectionString", "containerName"));

// Register the factory
builder.Services.AddSingleton<IStorageProviderFactory, StorageProviderFactory>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
