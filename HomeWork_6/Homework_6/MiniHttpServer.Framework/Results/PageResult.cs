using Templater;
using MiniHttpServer.Framework.Abstractions;
using System.Net;

namespace MiniHttpServer.Framework.Results;

public class PageResult : IHttpResult
{
    private readonly string _templatePath;
    private readonly object _dataModel;

    public PageResult(string templatePath, object dataModel)
    {
        _templatePath = templatePath;
        _dataModel = dataModel;
    }

    public async Task ExecuteAsync(HttpListenerResponse response, CancellationToken cancellationToken = default)
    {
        string html = new Templater.HtmlTemplate().RenderFromFile(_templatePath, _dataModel);

        response.StatusCode = 200;
        response.ContentType = "text/html; charset=utf-8";

        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(html);
        response.ContentLength64 = buffer.Length;
        await response.OutputStream.WriteAsync(buffer, 0, buffer.Length, cancellationToken);
    }
}
