using System.Reflection;

namespace MyFirstHttp.Framework.Registry;

public class ActionDescriptor
{
    public MethodInfo Method { get; }
    public string Verb { get; }
    public ParameterInfo[] Parameters { get; }
    public object Controller { get; }

    public ActionDescriptor(MethodInfo method, string verb, object controller)
    {
        Method = method;
        Verb = verb;
        Controller = controller;
        Parameters = method.GetParameters();
    }
}
