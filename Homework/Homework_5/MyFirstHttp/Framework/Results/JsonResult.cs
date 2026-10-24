using System.Net;
using System.Text.Json;
using MyFirstHttp.Framework.Abstractions;
using MyFirstHttp.Framework.Http;

namespace MyFirstHttp.Framework.Results;

public class JsonResult : IActionResult
{
    private readonly object _data;
    public JsonResult(object data)
    {
        _data = data;
    }

    public async Task ExecuteAsync(HttpListenerContext context)
    {
        string json = JsonSerializer.Serialize(_data, new JsonSerializerOptions{WriteIndented =  true});
        await HttpResponseWriter.WriteAsync(context.Response, 200, "application/json; charset=utf-8", json);
    }
}