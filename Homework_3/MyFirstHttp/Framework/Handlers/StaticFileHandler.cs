using System.Net;
using MyFirstHttp.Framework.Http;

namespace MyFirstHttp.Framework.Handlers;

public class StaticFileHandler : IRequestHandler
{
    private IRequestHandler _successor;

    public void SetSuccessor(IRequestHandler successor)
    {
        _successor = successor;
    }

    public async Task<bool> HandleAsync(HttpListenerContext context)
    {
        string path = context.Request.Url?.AbsolutePath ?? "/";
        
        if (path == "/") path = "/index.html";
        bool hasDot = path.Contains(".");
        
        if (hasDot)
        {
            string cleanPath = path.Replace('\\', '/').TrimStart('/');
            
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", cleanPath);
            var response = context.Response;

            if (File.Exists(filePath))
            {
                var fileInfo = new FileInfo(filePath);
                MimeTypeChecker.CheckType(response, fileInfo);
                
                byte[] buffer = await File.ReadAllBytesAsync(filePath);
                response.ContentLength64 = buffer.Length;
                await response.OutputStream.WriteAsync(buffer);
                await response.OutputStream.FlushAsync();
                
                response.Close();
                
                return true;
            }

            response.StatusCode = 404;
            var filePath404 = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "404.html");

            if (File.Exists(filePath404))
            {
                var fileInfo = new FileInfo(filePath404);

                byte[] buffer = await File.ReadAllBytesAsync(filePath404);
                response.ContentLength64 = buffer.Length;
                await response.OutputStream.WriteAsync(buffer);
                await response.OutputStream.FlushAsync();
            }
            else
            {
                await response.OutputStream.WriteAsync("<h1>404 Not Found</h1>"u8.ToArray());
            }
            response.Close();
            return true;
        }

        if (_successor != null)
        {
            return await _successor.HandleAsync(context);
        }
        
        return false;
    }
}