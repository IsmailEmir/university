namespace MyFirstHttp.Framework.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class HttpGetAttribute : HttpMethodAttribute
{
    public HttpGetAttribute(string route = "") : base("GET", route)
    {
    }
}
