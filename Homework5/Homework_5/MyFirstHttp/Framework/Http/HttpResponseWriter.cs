using System.Net;
using System.Text;

namespace MyFirstHttp.Framework.Http;

public static class HttpResponseWriter
{
    public static Task WriteAsync(HttpListenerResponse response, int statusCode, string contentType, string body)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(body);
        return WriteAsync(response, statusCode, contentType, buffer);
    }

    public static async Task WriteAsync(HttpListenerResponse response, int statusCode, string contentType, byte[] buffer)
    {
        response.StatusCode = statusCode;
        response.ContentType = contentType;
        response.ContentLength64 = buffer.Length;
        await response.OutputStream.WriteAsync(buffer);
        await response.OutputStream.FlushAsync();
        response.Close();
    }
}
