public class FileLoggerTests
{
    [Fact]
    public void Log_WritesLevelAndMessageToFile()
    {
        string dir = Path.Combine(Path.GetTempPath(), "logger_tests_" + Guid.NewGuid());
        string path = Path.Combine(dir, "logs", "test.log");   // вкладеної папки ще немає
        try
        {
            var logger = new FileLogger(path);

            logger.Info("перше");
            logger.Warn("друге");
            logger.Error("третє");

            string[] lines = File.ReadAllLines(path);
            Assert.Equal(3, lines.Length);
            Assert.Contains("[Info] перше", lines[0]);
            Assert.Contains("[Warning] друге", lines[1]);
            Assert.Contains("[Error] третє", lines[2]);
        }
        finally
        {
            if (Directory.Exists(dir)) Directory.Delete(dir, true);
        }
    }

    [Fact]
    public void Log_AppendsToExistingFile()
    {
        string path = Path.Combine(Path.GetTempPath(), "append_" + Guid.NewGuid() + ".log");
        try
        {
            new FileLogger(path).Info("один");
            new FileLogger(path).Info("два");   // новий логер дописує, а не перезаписує

            Assert.Equal(2, File.ReadAllLines(path).Length);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void Constructor_EmptyPath_Throws()
    {
        Assert.Throws<ArgumentException>(() => new FileLogger("  "));
    }

    [Fact]
    public void NullLogger_DoesNothing()
    {
        var exception = Record.Exception(() => new NullLogger().Info("нічого"));

        Assert.Null(exception);
    }
}
