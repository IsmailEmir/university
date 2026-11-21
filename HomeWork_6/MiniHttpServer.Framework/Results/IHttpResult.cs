using System.Net;
using System.Threading;

namespace MiniHttpServer.Framework.Results;

public interface IHttpResult
{
    Task ExecuteAsync(
        HttpListenerResponse response,
        CancellationToken cancellationToken = default);
}