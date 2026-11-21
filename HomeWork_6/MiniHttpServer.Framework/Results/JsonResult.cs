using System.Net;
using System.Text.Json;
using System.Threading;

namespace MiniHttpServer.Framework.Results;

public class JsonResult : IHttpResult
{
    private readonly object _data;

    public JsonResult(object data)
    {
        _data = data;
    }

    public async Task ExecuteAsync(
        HttpListenerResponse response,
        CancellationToken cancellationToken = default)
    {
        byte[] buffer = JsonSerializer.SerializeToUtf8Bytes(_data);
        response.StatusCode = 200;
        response.ContentType = "application/json; charset=utf-8";
        response.ContentLength64 = buffer.Length;
        await response.OutputStream.WriteAsync(buffer, 0, buffer.Length, cancellationToken);
    }
}
