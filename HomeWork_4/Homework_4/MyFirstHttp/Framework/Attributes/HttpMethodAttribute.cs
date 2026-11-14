namespace MyFirstHttp.Framework.Attributes;

public abstract class HttpMethodAttribute : Attribute
{
    public string Verb { get; }

    protected HttpMethodAttribute(string verb)
    {
        Verb = verb;
    }
}
