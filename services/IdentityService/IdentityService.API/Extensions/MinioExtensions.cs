using Minio;

namespace IdentityService.API.Extensions;

public static class MinioExtensions
{
    public static void AddMinioExtension(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMinio(configureClient => 
        {
            var endpoint = configuration["Minio:Endpoint"];
            var accessKey = configuration["Minio:AccessKey"];
            var secretKey = configuration["Minio:SecretKey"];
            var useSsl = bool.Parse(configuration["Minio:UseSSL"] ?? "false");

            configureClient
                .WithEndpoint(endpoint)
                .WithCredentials(accessKey, secretKey)
                .WithSSL(useSsl);
        });
    }
}