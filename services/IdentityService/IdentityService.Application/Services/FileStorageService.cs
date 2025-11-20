using IdentityService.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;

namespace IdentityService.Application.Services;

public class FileStorageService(
    IMinioClient minioClient,
    IConfiguration configuration,
    ILogger<FileStorageService> logger)
    : IFileStorageService
{
    private readonly string? _bucketName = configuration["Minio:BucketName"];

    public async Task<string?> UploadAvatarAsync(Stream fileStream, string fileName, string contentType)
    {
        try 
        {
            var objectName = $"avatars/{Guid.NewGuid()}{Path.GetExtension(fileName)}";
            var args = new PutObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(objectName)
                .WithStreamData(fileStream)
                .WithObjectSize(fileStream.Length)
                .WithContentType(contentType);

            await minioClient.PutObjectAsync(args);
            logger.LogInformation($"Файл {fileName} загружен как {objectName}");
            return objectName;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка загрузки файла в MinIO");
            return null;
        }
    }

    public async Task<string> GetAvatarUrlAsync(string objectName)
    {
        var args = new PresignedGetObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(objectName)
            .WithExpiry(3600 * 24);

        return await minioClient.PresignedGetObjectAsync(args);
    }
    
    public async Task<bool> DeleteAvatarAsync(string objectName)
    {
        if (string.IsNullOrWhiteSpace(objectName) )
        {
            logger.LogWarning("Попытка удаления аватара с пустым именем объекта");
            return false;
        }

        try
        {
            var args = new RemoveObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(objectName);

            await minioClient.RemoveObjectAsync(args);
            logger.LogInformation("Аватар {ObjectName} успешно удалён", objectName);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка удаления аватара {ObjectName}", objectName);
            return false;
        }
    }
}