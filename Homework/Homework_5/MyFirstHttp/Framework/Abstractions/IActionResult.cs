using System.Net;

namespace MyFirstHttp.Framework.Abstractions;

public interface IActionResult
{
    Task ExecuteAsync(HttpListenerContext context);
}