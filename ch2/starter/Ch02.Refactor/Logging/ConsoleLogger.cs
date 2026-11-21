namespace EduHub.Ch02.Logging;

public class ConsoleLogger : IAppLogger
{
    public void Log(string message)
    {
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {message}");
    }
}
