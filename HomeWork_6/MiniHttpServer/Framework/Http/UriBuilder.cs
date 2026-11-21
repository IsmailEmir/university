using MyFirstHttp.Framework.Configuration;

namespace MyFirstHttp.Framework.Http;

public class FabricUri
{
    public static Uri CreateUri(ServerSettings settings)
    {
        UriBuilder uriBuilder = new();

        uriBuilder.Scheme = "http";
        uriBuilder.Host = settings.Server.Host;
        uriBuilder.Port = settings.Server.Port;
        return uriBuilder.Uri;
    }
}