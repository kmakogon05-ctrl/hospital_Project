using System;
using System.IO;
using System.Text;

// Рівні важливості повідомлень у журналі
enum LogLevel
{
    Info,      // звичайна подія (пацієнта прийнято, виписано)
    Warning,   // щось не так, але програма працює далі (відмова, неправильний рядок у файлі)
    Error      // операція не вдалася (не вдалося прочитати/записати файл)
}

// Інтерфейс логера. Hospital залежить лише від нього, а не від конкретного файлу:
// у програмі це FileLogger, у тестах можна підставити будь-який інший.
interface ILogger
{
    void Log(LogLevel level, string message);
}

// Зручні короткі виклики: log.Info("..."), log.Warn("..."), log.Error("...")
static class LoggerExtensions
{
    public static void Info(this ILogger logger, string message) => logger.Log(LogLevel.Info, message);
    public static void Warn(this ILogger logger, string message) => logger.Log(LogLevel.Warning, message);
    public static void Error(this ILogger logger, string message) => logger.Log(LogLevel.Error, message);
}

// «Порожній» логер: нічого не робить. Потрібен, щоб не перевіряти logger != null.
class NullLogger : ILogger
{
    public void Log(LogLevel level, string message) { }
}

// Пише рядки виду «2026-11-25 17:01:05 [Info] текст» у файл (дописує в кінець).
class FileLogger : ILogger
{
    private readonly string path;
    private readonly object sync = new();   // щоб два потоки не писали у файл одночасно

    public FileLogger(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Шлях до файлу журналу не може бути порожнім.");

        this.path = path;

        string? directory = Path.GetDirectoryName(Path.GetFullPath(path));
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);
    }

    public void Log(LogLevel level, string message)
    {
        string line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] {message}{Environment.NewLine}";
        try
        {
            lock (sync)
                File.AppendAllText(path, line, Encoding.UTF8);
        }
        catch (IOException)
        {
            // Збій журналу не повинен «валити» основну програму
        }
        catch (UnauthorizedAccessException)
        {
        }
    }
}
