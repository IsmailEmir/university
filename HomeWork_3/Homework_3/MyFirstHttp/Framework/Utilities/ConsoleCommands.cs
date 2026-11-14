using System.Net;
using MyFirstHttp.Framework.Http;

namespace MyFirstHttp.Framework.Utilities;

public class ConsoleCommands
{
    public static void ActivateCommand(HttpListener listener)
    {
        var command = Console.ReadLine()!.ToLower();
        if (command == "")
        {
            Console.WriteLine("Please enter a valid command.");
        }

        switch (command)
        {
            case "start":
                if (listener.IsListening)
                {
                    LogMessage.PrintLog("Сервер запущен.");
                }
                else listener.Start();
                break;

            case "stop":
                if (!listener.IsListening)
                {
                    LogMessage.PrintLog("Сервер выключен.");
                }
                else listener.Stop();
                break;
        }
    }
}