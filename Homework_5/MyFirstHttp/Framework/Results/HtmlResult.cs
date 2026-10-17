using System.Net;
using MyFirstHttp.Framework.Abstractions;
using MyFirstHttp.Framework.Http;

namespace MyFirstHttp.Framework.Results;

public class HtmlResult :IActionResult
{
    private readonly string _data;
    public  HtmlResult(string data)
    {
        _data = data;
    }

    public async Task ExecuteAsync(HttpListenerContext context)
    {
        string filePath = Path.Combine(AppContext.BaseDirectory, "wwwroot", _data);

        if (File.Exists(filePath))
        {
            string htmlContent = await File.ReadAllTextAsync(filePath);
            await HttpResponseWriter.WriteAsync(context.Response, 200, "text/html; charset=utf-8", htmlContent);
        }
        else
        {
            await HttpResponseWriter.WriteAsync(context.Response, 404, "text/plain; charset=utf-8", "404 File Not Found");
        }
    }
}