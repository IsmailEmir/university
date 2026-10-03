namespace MyFirstHttp.Framework.Utilities;

public class LogMessage
{
    public static void PrintLog(string? message)
    {
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {message}");
    }

    public static void PrintMenu()
    {
        Console.WriteLine("");
        Console.WriteLine("====================");
        Console.WriteLine("  start - запустить ");
        Console.WriteLine("  stop - остановить ");
        Console.WriteLine("====================");
        Console.WriteLine("");
    }
}