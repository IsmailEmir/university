using System.Net;
using System.Text.Json;
using MyFirstHttp;

public class HttpServer
{
    private readonly HttpListener _listener;
    private readonly ServerSettings _settings;

    public HttpServer()
    {
        string json = File.ReadAllText("settings.json");

        _settings = JsonSerializer.Deserialize<ServerSettings>(json)
                    ?? throw new Exception("Не удалось прочитать settings.json");

        if (!_settings.Server.Path.StartsWith("/"))
        {
            _settings.Server.Path = "/" + _settings.Server.Path;
        }

        if (!_settings.Server.Path.EndsWith("/"))
        {
            _settings.Server.Path += "/";
        }

        _listener = new HttpListener();

        string prefix =
            $"http://{_settings.Server.Host}:{_settings.Server.Port}{_settings.Server.Path}";

        _listener.Prefixes.Add(prefix);
    }

    private void Log(string message)
    {
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {message}");
    }

    public async Task StartAsync()
    {
        if (_listener.IsListening)
        {
            Log("Сервер уже запущен.");
            return;
        }

        _listener.Start();
        Log("Сервер запущен.");
        
        string prefix = $"http://{_settings.Server.Host}:{_settings.Server.Port}{_settings.Server.Path}";

        Log($"Адрес: {prefix}");

        while (_listener.IsListening)
        {
            try
            {
                var context = await _listener.GetContextAsync();

                Log(
                    $"Получен запрос: " +
                    $"{context.Request.HttpMethod} {context.Request.RawUrl}"
                );

                await HandleRequestAsync(context);

                Log("Ответ отправлен.");
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
        string serverPath = _settings.Server.Path;

        string filePath;
        string contentType;

        if (path == serverPath || path == serverPath.TrimEnd('/'))
        {
            filePath = Path.Combine(
                AppContext.BaseDirectory,
                "wwwroot",
                "index.html"
            );
            contentType = "text/html; charset=utf-8";
        }
        else if (path == serverPath + "styles.css")
        {
            filePath = Path.Combine(
                AppContext.BaseDirectory,
                "wwwroot",
                "styles.css"
            );
            contentType = "text/css; charset=utf-8";
        }
        else
        {
            context.Response.StatusCode = 404;
            context.Response.Close();
            Log($"Файл не найден: {path}");
            return;
        }

        if (!File.Exists(filePath))
        {
            context.Response.StatusCode = 404;
            context.Response.Close();

            Log($"Файл не найден: {filePath}");

            return;
        }

        byte[] buffer = await File.ReadAllBytesAsync(filePath);

        var response = context.Response;

        response.ContentType = contentType;
        response.ContentLength64 = buffer.Length;

        await response.OutputStream.WriteAsync(buffer);

        response.Close();
    }

    public void Stop()
    {
        if (!_listener.IsListening)
        {
            Log("Сервер уже остановлен.");
            return;
        }
        _listener.Stop();
        Log("Сервер остановлен.");
    }
}