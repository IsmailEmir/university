using System.Net;
using MiniHttpServer.Framework.Abstractions;
using MiniHttpServer.Framework.Http;

namespace MiniHttpServer.Framework.Handlers;

public class NotFoundHandler : IRequestHandler
{
    public async Task<bool> HandleAsync(HttpListenerContext context)
    {
        string filePath = Path.Combine(AppContext.BaseDirectory, "wwwroot", "404.html");

        if (File.Exists(filePath))
        {
            var fileInfo = new FileInfo(filePath);
            string contentType = MimeTypeChecker.CheckType(fileInfo);
            byte[] buffer = await File.ReadAllBytesAsync(filePath);
            await HttpResponseWriter.WriteAsync(context.Response, 404, contentType, buffer);
            return true;
        }

        await HttpResponseWriter.WriteAsync(context.Response, 404, "text/html; charset=utf-8", "<h1>404 Not Found</h1>");
        return true;
    }
}
