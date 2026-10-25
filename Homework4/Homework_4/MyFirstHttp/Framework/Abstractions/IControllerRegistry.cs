using MyFirstHttp.Framework.Registry;

namespace MyFirstHttp.Framework.Abstractions;

public interface IControllerRegistry
{
    bool HasRoute(string route);
    bool TryResolve(string route, string action, string verb, out ActionDescriptor descriptor);
}
