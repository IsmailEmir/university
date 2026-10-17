namespace MyFirstHttp.Framework.Attributes;

public abstract class HttpMethodAttribute : Attribute
{
    public string Method { get; }
    public string Verb { get; }

    protected HttpMethodAttribute(string verb,  string method)
    {
        Verb = verb;
        Method = method;
    }
}
