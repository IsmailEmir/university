using System.Net;
using MyFirstHttp.Framework.Result;

namespace MyFirstHttp.Framework;

public abstract class ControllerBase
{
    protected HttpListenerContext Context{ get; private set; }

    internal void SetContext(HttpListenerContext context)
    {
        Context = context;
    }
    
    protected JsonResult Json(object data) => new JsonResult(data);
    protected HtmlResult Html(string data) => new HtmlResult(data);
}