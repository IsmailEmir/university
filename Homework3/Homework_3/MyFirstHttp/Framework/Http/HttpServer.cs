using System.IO.Pipelines;
using System.Net;
using MyFirstHttp.Framework.Utilities;

namespace MyFirstHttp.Framework.Http;

public class HttpServer
{
    private HttpListener _listener = new();
    

    private void Start()
    {
        var server = JsonDeserialize.GetServer;
        var uri = FabricUri.CreateUri(server);
        
        var prefix = uri.AbsoluteUri;
        _listener.Prefixes.Add(uri.AbsoluteUri);
        
        
        _listener.Start();
        
        LogMessage.PrintLog("Сервер запущен.");
        LogMessage.PrintLog($"Адрес: {prefix}");
    }

    private async Task GetContextAsync()
    {

        while (_listener.IsListening)
        {
            try
            {
                var context = await _listener.GetContextAsync();
                
                LogMessage.PrintLog(
                    $"Получен запрос: " +
                    $"{context.Request.HttpMethod} {context.Request.RawUrl}"
                );

                await HandleRequestAsync(context);

                LogMessage.PrintLog("Ответ отправлен.");
            }
            catch (HttpListenerException)
            {
                break;
            }
            catch (ObjectDisposedException)
            {
                break;
            }
        }
    }

    private async Task HandleRequestAsync(HttpListenerContext context)
    {
        string path = context.Request.Url?.AbsolutePath ?? "";
        var filePath =  Path.Combine(Directory.GetCurrentDirectory(),"wwwroot", path.Trim('/'));
        
        var response = context.Response;
        
        HttpChecker.HttpCheck(response, ref filePath);
        
        var fileInfo = new FileInfo(filePath);
        MimeTypeChecker.CheckType(response, fileInfo);
        
        byte[] buffer = await File.ReadAllBytesAsync(filePath);
        response.ContentLength64 = buffer.Length;
        await response.OutputStream.WriteAsync(buffer);
        await response.OutputStream.FlushAsync();
    }

    public void Run()
    {
        while (!_listener.IsListening)
        {
            LogMessage.PrintMenu();
            ConsoleCommands.ActivateCommand(_listener);
            Start();
        }
        

        var task1 = Task.Run(() => ConsoleCommands.ActivateCommand(_listener));
        
        var task2 = Task.Run(async () => await GetContextAsync());
        Task.WaitAll(task1, task2);
    }

    // public void Stop()
    // {
    //     if (!_listener.IsListening)
    //     {
    //         LogMessage.PrintLog("Сервер уже остановлен.");
    //         return;
    //     }
    //     _listener.Stop();
    //     LogMessage.PrintLog("Сервер остановлен.");
    // }
}