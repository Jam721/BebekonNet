namespace IdentityService.API.Validation;

public class FileValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IList<string> _allowedExtensions = new[] { ".jpg", ".png", ".gif", ".jpeg", ".bmp", ".ico" };
    private const long MaxFileSize = 1 * 1024 * 1024;

    public FileValidationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        if (context.Request.HasFormContentType)
        {
            var form = await context.Request.ReadFormAsync();
            foreach (var file in form.Files)
            {
                if (file.Length > MaxFileSize)
                {
                    context.Response.StatusCode = 413;
                    await context.Response.WriteAsync("File size exceeds limit");
                    return;
                }

                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (string.IsNullOrEmpty(ext) || !_allowedExtensions.Contains(ext))
                {
                    context.Response.StatusCode = 415;
                    await context.Response.WriteAsync("Unsupported file type");
                    return;
                }
            }
        }

        await _next(context);
    }
}