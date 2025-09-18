namespace AdmIn.API.Services
{
    public class ApiLoggerService : IApiLoggerService
    {
        private readonly ILogger<ApiLoggerService> _logger;
        private static readonly object _lockObject = new object();

        public ApiLoggerService(ILogger<ApiLoggerService> logger)
        {
            _logger = logger;
        }

        public void WriteLog(string message, string logLevel = "INFO", string source = "API")
        {
            try
            {
                var logEntry = FormatLogEntry(message, logLevel, source);
                WriteToFile(logEntry);
                
                // También escribir a ILogger para ASP.NET Core logging
                switch (logLevel.ToUpper())
                {
                    case "ERROR":
                        _logger.LogError("[{Source}] {Message}", source, message);
                        break;
                    case "WARNING":
                        _logger.LogWarning("[{Source}] {Message}", source, message);
                        break;
                    case "DEBUG":
                        _logger.LogDebug("[{Source}] {Message}", source, message);
                        break;
                    default:
                        _logger.LogInformation("[{Source}] {Message}", source, message);
                        break;
                }
            }
            catch
            {
                // Ignorar errores de logging para no afectar la aplicación
            }
        }

        public async Task WriteLogAsync(string message, string logLevel = "INFO", string source = "API")
        {
            try
            {
                var logEntry = FormatLogEntry(message, logLevel, source);
                await WriteToFileAsync(logEntry);
                
                // También escribir a ILogger para ASP.NET Core logging
                switch (logLevel.ToUpper())
                {
                    case "ERROR":
                        _logger.LogError("[{Source}] {Message}", source, message);
                        break;
                    case "WARNING":
                        _logger.LogWarning("[{Source}] {Message}", source, message);
                        break;
                    case "DEBUG":
                        _logger.LogDebug("[{Source}] {Message}", source, message);
                        break;
                    default:
                        _logger.LogInformation("[{Source}] {Message}", source, message);
                        break;
                }
            }
            catch
            {
                // Ignorar errores de logging para no afectar la aplicación
            }
        }

        public string[] ReadLogLines(int maxLines = 100)
        {
            try
            {
                var logFile = GetLogFilePath();
                
                if (!File.Exists(logFile))
                {
                    return new[] { $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] [INFO] [API] Log file no existe aún. Esperando logs..." };
                }

                var allLines = File.ReadAllLines(logFile);
                
                if (allLines.Length <= maxLines)
                {
                    return allLines;
                }
                
                // Devolver las últimas 'maxLines' líneas
                var result = new string[maxLines];
                Array.Copy(allLines, allLines.Length - maxLines, result, 0, maxLines);
                return result;
            }
            catch (Exception ex)
            {
                return new[] { $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] [ERROR] [API] Error leyendo logs: {ex.Message}" };
            }
        }

        public string GetLogFilePath()
        {
            var logsDir = GetLogsDirectory();
            return Path.Combine(logsDir, $"AdmIn-API-{DateTime.Now:yyyy-MM-dd}.log");
        }

        public string GetLogsDirectory()
        {
            // Intentar primero la ubicación de IIS
            var iisLogsPath = @"C:\inetpub\logs\AdmIn";
            
            if (TryCreateDirectory(iisLogsPath))
            {
                return iisLogsPath;
            }
            
            // Fallback a directorio de la aplicación
            var appLogsPath = Path.Combine(AppContext.BaseDirectory, "Logs");
            TryCreateDirectory(appLogsPath);
            
            return appLogsPath;
        }

        private string FormatLogEntry(string message, string logLevel, string source)
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            return $"[{timestamp}] [{logLevel.ToUpper()}] [{source}] {message}";
        }

        private void WriteToFile(string logEntry)
        {
            lock (_lockObject)
            {
                try
                {
                    var logFile = GetLogFilePath();
                    File.AppendAllText(logFile, logEntry + Environment.NewLine);
                }
                catch
                {
                    // Ignorar errores de escritura
                }
            }
        }

        private async Task WriteToFileAsync(string logEntry)
        {
            try
            {
                var logFile = GetLogFilePath();
                await File.AppendAllTextAsync(logFile, logEntry + Environment.NewLine);
            }
            catch
            {
                // Ignorar errores de escritura
            }
        }

        private bool TryCreateDirectory(string path)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}