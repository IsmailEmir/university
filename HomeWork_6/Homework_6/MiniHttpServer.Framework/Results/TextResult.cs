using System.Net;
using System.Text;
using System.Threading;
using MiniHttpServer.Framework.Http;

namespace MiniHttpServer.Framework.Results;

public class TextResult : IHttpResult
{
    private readonly string _data;
    private readonly int _statusCode;

    public TextResult(string data, HttpStatusCode statusCode)
    {
        _data = data;
        _statusCode = (int)statusCode;
    }

    public async Task ExecuteAsync(
        HttpListenerResponse response,
        CancellationToken cancellationToken = default)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(_data);
        response.StatusCode = _statusCode;
        response.ContentType = "text/plain; charset=utf-8";
        response.ContentLength64 = buffer.Length;
        await response.OutputStream.WriteAsync(buffer, 0, buffer.Length, cancellationToken);
    }
}
