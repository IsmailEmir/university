namespace MyFirstHttp.Framework.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class ControllerAttribute : Attribute
{
    public string Route { get; }

    public ControllerAttribute(string route)
    {
        Route = route;
    }
}
