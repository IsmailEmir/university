using System.Reflection;
using MyFirstHttp.Framework.Abstractions;
using MyFirstHttp.Framework.Attributes;
using MyFirstHttp.Framework.Utilities;

namespace MyFirstHttp.Framework.Registry;

public class ControllerRegistry : IControllerRegistry
{
    private readonly Dictionary<string, Dictionary<string, ActionDescriptor>> _actions = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<Type, Func<object>> _factories = new();

    public void RegisterFactory(Type controllerType, Func<object> factory)
    {
        _factories[controllerType] = factory;
    }

    public void Build()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();

        var controllerTypes = assembly.GetTypes()
            .Where(t => t.GetCustomAttribute<ControllerAttribute>() is not null)
            .ToList();

        foreach (var controllerType in controllerTypes)
        {
            string route = controllerType.GetCustomAttribute<ControllerAttribute>()!.Route.Trim().Trim('/');

            if (string.IsNullOrEmpty(route))
            {
                throw new InvalidOperationException($"Контроллер {controllerType.Name} должен иметь непустой маршрут в атрибуте [Controller]");
            }

            if (_actions.ContainsKey(route))
            {
                throw new InvalidOperationException($"Маршрут '{route}' используется больше одного контроллера");
            }

            object controller = CreateController(controllerType);

            var methods = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => !m.IsSpecialName)
                .Select(m => new { Method = m, Attribute = m.GetCustomAttribute<HttpMethodAttribute>() })
                .Where(m => m.Attribute is not null)
                .ToList();

            if (methods.Count == 0)
            {
                throw new InvalidOperationException($"Контроллер {controllerType.Name} не содержит действий с атрибутами [HttpGet] или [HttpPost]");
            }

            var actions = new Dictionary<string, ActionDescriptor>(StringComparer.OrdinalIgnoreCase);

            foreach (var item in methods)
            {
                string key = $"{item.Attribute!.Verb}:{item.Attribute.Method}";

                if (!actions.TryAdd(key, new ActionDescriptor(item.Method, item.Attribute.Verb, controller)))
                {
                    throw new InvalidOperationException($"Контроллер {controllerType.Name} содержит повторяющееся действие '{item.Method.Name}' с методом {item.Attribute.Verb}");
                }
            }

            _actions[route] = actions;
            LogMessage.PrintLog($"Контроллер {controllerType.Name}: маршрут '{route}', действий: {actions.Count}");
        }

        LogMessage.PrintLog($"Реестр контроллеров построен: контроллеров - {controllerTypes.Count}, действий - {_actions.Values.Sum(a => a.Count)}");
    }

    public bool HasRoute(string route)
    {
        return !string.IsNullOrEmpty(route) && _actions.ContainsKey(route);
    }

    public bool TryResolve(string route, string action, string verb, out ActionDescriptor descriptor)
    {
        descriptor = null!;

        if (string.IsNullOrEmpty(route) || string.IsNullOrEmpty(action) || string.IsNullOrEmpty(verb))
        {
            return false;
        }

        if (!_actions.TryGetValue(route, out var actions))
        {
            return false;
        }

        if (!actions.TryGetValue($"{verb}:{action}", out var actionDescriptor))
        {
            return false;
        }

        descriptor = actionDescriptor!;
        return true;
    }

    private object CreateController(Type controllerType)
    {
        if (_factories.TryGetValue(controllerType, out var factory))
        {
            return factory();
        }

        if (controllerType.GetConstructor(Type.EmptyTypes) is null)
        {
            throw new InvalidOperationException($"Контроллер {controllerType.Name} должен иметь публичный конструктор без параметров или зарегистрированную фабрику");
        }

        return Activator.CreateInstance(controllerType)!;
    }
}
