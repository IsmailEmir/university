using System.Net;

namespace MiniHttpServer.Framework.Abstractions;

public interface IRequestHandler
{
    Task<bool> HandleAsync(HttpListenerContext context);
}
