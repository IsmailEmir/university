using System.Collections.Specialized;
using System.Net;
using MyFirstHttp.Framework.Abstractions;
using MyFirstHttp.Framework.Results;

namespace MyFirstHttp.Framework;

public abstract class ControllerBase
{
    protected HttpListenerContext Context{ get; private set; }

    public NameValueCollection Headers => Context.Request.Headers;
    public NameValueCollection Query => Context.Request.QueryString;
    
    internal void SetContext(HttpListenerContext context)
    {
        Context = context;
    }

    protected IActionResult Html(string html) => new HtmlResult(html);
    protected IActionResult Json(string html) => new JsonResult(html);
    protected IActionResult File(byte[] bytes, string contentType, string fileName) => new FileResult(bytes, contentType, fileName);
    protected IActionResult Text(string text, HttpStatusCode statusCode = HttpStatusCode.OK) 
        => new TextResult(text, statusCode);
}