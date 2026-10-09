using System.Net;

namespace MyFirstHttp.Framework.Handlers;

public interface IRequestHandler
{
    Task<bool> HandleAsync(HttpListenerContext context);
    void SetSuccessor(IRequestHandler handler);
}