using System.Net;
using MyFirstHttp.Framework.Abstractions;
using MyFirstHttp.Framework.Utilities;

namespace MyFirstHttp.Framework.Http;

public class HttpServer
{
    private readonly HttpListener _listener = new();
    private readonly IRequestHandler _handlerChain;

    public HttpServer(IRequestHandler handlerChain)
    {
        _handlerChain = handlerChain;
    }

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

                bool handled = await _handlerChain.HandleAsync(context);

                LogMessage.PrintLog(handled ? "Ответ отправлен." : "Запрос не обработан.");
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
}
