using System.Net;
using MyFirstHttp.Framework.Abstractions;
using MyFirstHttp.Framework.Http;

namespace MyFirstHttp.Framework.Results;

public class TextResult : IActionResult
{
    private readonly string _data;
    private readonly int _statusCode;
    
    public  TextResult(string data, HttpStatusCode statusCode)
    {
        _data = data;
        _statusCode = (int)statusCode;
    }

    public async Task ExecuteAsync(HttpListenerContext context)
    {
        await HttpResponseWriter.WriteAsync(context.Response, _statusCode, "text/plain; charset=utf-8", _data);
    }
}