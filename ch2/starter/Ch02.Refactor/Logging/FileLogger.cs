namespace EduHub.Ch02.Logging;

public class FileLogger : IAppLogger
{
    private readonly string _filePath;

    public FileLogger(string filePath)
    {
        _filePath = string.IsNullOrWhiteSpace(filePath)
            ? throw new ArgumentException("Log file path is required.", nameof(filePath))
            : filePath;
    }

    public void Log(string message)
    {
        File.AppendAllText(_filePath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}{Environment.NewLine}");
    }
}
