using System.Collections.Specialized;
using System.Net;
using System.Text;
using Templater;
using MyFirstHttp.Framework.Results;

namespace MyFirstHttp.Framework;

public abstract class BaseController
{
    protected HttpListenerContext Context { get; private set; }

    public NameValueCollection Headers => Context?.Request.Headers ?? new NameValueCollection();

    public NameValueCollection Query => Context?.Request.QueryString ?? new NameValueCollection();

    internal void SetContext(HttpListenerContext context)
    {
        Context = context;
    }

    protected IHttpResult Html(string html) => new HtmlResult(html);

    protected IHttpResult Json(object data) => new JsonResult(data);

    protected IHttpResult Text(string text, HttpStatusCode statusCode = HttpStatusCode.OK) => new TextResult(text, statusCode);

    protected IHttpResult Page(string templatePath, object dataModel) => new PageResult(templatePath, dataModel);
}
