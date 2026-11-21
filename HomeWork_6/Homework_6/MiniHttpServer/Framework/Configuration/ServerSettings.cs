namespace MyFirstHttp.Framework.Configuration;

public class ServerSettings
{
    public Server Server { get; set; } = new();
}

public class Server
{
    public string Host { get; set; } = "127.0.0.1";
    public int Port { get; set; } = 8080;
    public string Path { get; set; } = "connection";
}