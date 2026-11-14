using System.Net;

namespace MyFirstHttp.Framework.Abstractions;

public interface IRequestHandler
{
    Task<bool> HandleAsync(HttpListenerContext context);
}
