using System.Net;
using MyFirstHttp.Framework.Abstractions;
using MyFirstHttp.Framework.Http;

namespace MyFirstHttp.Framework.Handlers;

public class StaticFileHandler : IRequestHandler
{
    private readonly IRequestHandler _successor;

    public StaticFileHandler(IRequestHandler successor)
    {
        _successor = successor;
    }

    public async Task<bool> HandleAsync(HttpListenerContext context)
    {
        string path = context.Request.Url?.AbsolutePath ?? "/";

        if (path == "/") path = "/index.html";

        if (!path.Contains("."))
        {
            return await _successor.HandleAsync(context);
        }

        string wwwrootPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "wwwroot"));
        string cleanPath = path.Replace('\\', '/').TrimStart('/');
        string filePath = Path.GetFullPath(Path.Combine(wwwrootPath, cleanPath));

        if (filePath.StartsWith(wwwrootPath + Path.DirectorySeparatorChar, StringComparison.Ordinal) && File.Exists(filePath))
        {
            var fileInfo = new FileInfo(filePath);
            string contentType = MimeTypeChecker.CheckType(fileInfo);
            byte[] buffer = await File.ReadAllBytesAsync(filePath);
            await HttpResponseWriter.WriteAsync(context.Response, 200, contentType, buffer);
            return true;
        }

        return await _successor.HandleAsync(context);
    }
}
