using System.Net;
using System.Text;
using System.Text.Json;

namespace MyFirstHttp.Framework.Result;

public class HtmlResult
{
    private readonly string _html;
    public HtmlResult(string html)
    {
        _html = html;
    }

    public byte[] Execute(HttpListenerContext context)
    {
        // TODO: Логика преобразования
        
        context.Response.ContentType = "text/html; charset=utf-8";
        byte[] buffer = Encoding.UTF8.GetBytes(_html);
        
        return buffer;
    }
}