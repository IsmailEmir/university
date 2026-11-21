using System.Net;
using System.Threading;

namespace MyFirstHttp.Framework.Results;

public interface IHttpResult
{
    Task ExecuteAsync(
        HttpListenerResponse response,
        CancellationToken cancellationToken = default);
}