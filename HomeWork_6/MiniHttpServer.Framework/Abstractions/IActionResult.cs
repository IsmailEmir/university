using System.Net;

namespace MiniHttpServer.Framework.Abstractions;

public interface IActionResult
{
    Task ExecuteAsync(HttpListenerContext context);
}