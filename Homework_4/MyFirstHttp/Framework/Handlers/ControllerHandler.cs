using System.Net;
using MyFirstHttp.Framework.Abstractions;
using MyFirstHttp.Framework.Http;
using MyFirstHttp.Framework.Registry;

namespace MyFirstHttp.Framework.Handlers;

public class ControllerHandler : IRequestHandler
{
    private readonly IControllerRegistry _registry;
    private readonly IRequestHandler _successor;

    public ControllerHandler(IControllerRegistry registry, IRequestHandler successor)
    {
        _registry = registry;
        _successor = successor;
    }

    public async Task<bool> HandleAsync(HttpListenerContext context)
    {
        string path = context.Request.Url?.AbsolutePath.Trim('/') ?? "/";
        string httpMethod = context.Request.HttpMethod.ToUpper();
        var response = context.Response;

        try
        {
            if (path.Contains("."))
            {
                return await _successor.HandleAsync(context);
            }

            var pathParts = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            string controllerRoute = pathParts.Length > 0 ? pathParts[0] : "";
            string actionName = pathParts.Length > 1 ? pathParts[1] : "";

            if (!_registry.HasRoute(controllerRoute))
            {
                return await _successor.HandleAsync(context);
            }

            if (!_registry.TryResolve(controllerRoute, actionName, httpMethod, out var descriptor))
            {
                await HttpResponseWriter.WriteAsync(response, 405, "text/plain; charset=utf-8", "405 Method Not Allowed");
                return true;
            }

            object?[] args = await ParameterBinder.BindAsync(context.Request, descriptor.Parameters);
            object? result = descriptor.Method.Invoke(descriptor.Controller, args);
            
            await HttpResponseWriter.WriteAsync(response, 200, "text/plain; charset=utf-8", result?.ToString() ?? "OK");
            return true;
        }
        catch (InvalidCastException)
        {
            await TryWriteErrorAsync(response, 400, "400 Bad Request");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR IN ControllerHandler.cs] {ex}");
            await TryWriteErrorAsync(response, 500, "500 Internal Server Error");
            return true;
        }
    }

    private static async Task TryWriteErrorAsync(HttpListenerResponse response, int statusCode, string body)
    {
        try
        {
            await HttpResponseWriter.WriteAsync(response, statusCode, "text/plain; charset=utf-8", body);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR WHILE WRITING RESPONSE] {ex.Message}");
        }
    }
}
