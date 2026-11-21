using System.Net;
using MyFirstHttp.Framework.Abstractions;

namespace MyFirstHttp.Framework.Results;

public class FileResult : IActionResult
{
    private readonly byte[] _fileBytes;
    private readonly string _fileName;
    private readonly string _contentType;

    public FileResult(byte[] fileBytes, string fileName, string contentType)
    {
        _fileBytes = fileBytes;
        _fileName = fileName;
        _contentType = contentType;
    }

    public async Task ExecuteAsync(HttpListenerContext context)
    {
        var response = context.Response;
        response.StatusCode = 200;
        response.ContentType = _contentType;
        response.AddHeader("Content-Disposition", "attachment; filename=" + _fileName);
        response.ContentLength64 = _fileBytes.Length;
        await response.OutputStream.WriteAsync(_fileBytes, 0, _fileBytes.Length);
        response.Close();
    }
}