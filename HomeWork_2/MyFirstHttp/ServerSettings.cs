namespace MyFirstHttp;

public class ServerSettings
{
    public Server Server { get; set; } = new();
}

public class Server
{
    public string Host { get; set; } = "";
    public int Port { get; set; }
    public string Path { get; set; } = "";
}