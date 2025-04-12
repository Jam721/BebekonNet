namespace IdentityService.Application.Interfaces.Services;

public interface IFileStorageService
{
    Task<string?> UploadAvatarAsync(Stream fileStream, string fileName, string contentType);
    Task<string> GetAvatarUrlAsync(string objectName);
}