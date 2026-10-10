using System.Net;
using System.Text;
using System.Text.Json;

namespace MyFirstHttp.Framework.Result;

public class JsonResult
{
    private readonly object _data;
    public JsonResult(object data)
    {
        _data = data;
    }

    public byte[] Execute(HttpListenerContext context)
    {
        // TODO: Логика преобразования
        string json = JsonSerializer.Serialize(_data);
        context.Response.ContentType = "application/json; charset=utf-8";
        byte[] buffer = Encoding.UTF8.GetBytes(json);
        
        return buffer;
    }
}