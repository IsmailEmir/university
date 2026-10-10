namespace MyFirstHttp.Framework.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class HttpPostAttribute : HttpMethodAttribute
{
    public HttpPostAttribute() : base("POST")
    {
    }
}
