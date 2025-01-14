using Serilog;

namespace LoggerService
{
    public class LogService
    {
        private readonly ILogger _logger;

        public LogService(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var logFilePath = Path.Combine(directory, "app-log.json");

            _logger = new LoggerConfiguration()
                .WriteTo.File(
                    path: logFilePath,
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message:lj}{NewLine}{Exception}",
                    formatProvider: null,
                    restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
                    shared: true
                )
                .CreateLogger();
        }

        public void LogInformation(string message)
        {
            _logger.Information(message);
        }

        public void LogError(string message, Exception? exception = null)
        {
            _logger.Error(exception, message);
        }

        public void LogDebug(string message)
        {
            _logger.Debug(message);
        }

        public void LogWarning(string message)
        {
            _logger.Warning(message);
        }
    }
}
