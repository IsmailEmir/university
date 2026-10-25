using System.Net;

namespace MyFirstHttp.Framework;

public abstract class ControllerBase
{
    protected HttpListenerContext Context{ get; private set; }

    internal void SetContext(HttpListenerContext context)
    {
        Context = context;
    }
}