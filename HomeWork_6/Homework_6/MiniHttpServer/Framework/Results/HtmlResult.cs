using System.Net;
using System.Text;
using System.Threading;

namespace MyFirstHttp.Framework.Results;

public class HtmlResult : IHttpResult
{
    private readonly string _html;

    public HtmlResult(string html)
    {
        _html = html;
    }

    public async Task ExecuteAsync(
        HttpListenerResponse response,
        CancellationToken cancellationToken = default)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(_html);
        response.StatusCode = 200;
        response.ContentType = "text/html; charset=utf-8";
        response.ContentLength64 = buffer.Length;
        await response.OutputStream.WriteAsync(buffer, cancellationToken);
    }
}
