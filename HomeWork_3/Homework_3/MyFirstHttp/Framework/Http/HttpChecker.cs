using System.Net;
using MyFirstHttp.Framework.Utilities;

namespace MyFirstHttp.Framework.Http;

public class HttpChecker
{
    public static void HttpCheck(HttpListenerResponse response, ref string filePath)
    {
        if (!File.Exists(filePath))
        {
            response.StatusCode = 404;
            filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "404.html");
            LogMessage.PrintLog($"Файл не найден: {filePath}");
        }
    }
}