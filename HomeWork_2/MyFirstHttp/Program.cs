var server = new HttpServer();

Task? serverTask = null;

while (true)
{
    string? command = Console.ReadLine();

    switch (command)
    {
        case "start":
            if (serverTask == null)
            {
                serverTask = server.StartAsync();
            }
            break;

        case "stop":
            server.Stop();
            serverTask = null;
            break;
    }
}