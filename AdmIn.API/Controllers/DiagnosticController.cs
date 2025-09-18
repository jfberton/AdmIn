using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AdmIn.Common;
using AdmIn.Business.Servicios;
using System.Security.Claims;
using Microsoft.Data.SqlClient;
using AdmIn.API.Services;

namespace AdmIn.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiagnosticController : ControllerBase
    {
        private readonly IServ_Rol _servRol;
        private readonly IServ_Usuario _servUsuario;
        private readonly IServ_Proveedor _servProveedor;
        private readonly IConfiguration _configuration;
        private readonly ILogger<DiagnosticController> _logger;
        private readonly IApiLoggerService _apiLogger;

        public DiagnosticController(IServ_Rol servRol, IServ_Usuario servUsuario, IServ_Proveedor servProveedor, 
            IConfiguration configuration, ILogger<DiagnosticController> logger, IApiLoggerService apiLogger)
        {
            _servRol = servRol;
            _servUsuario = servUsuario;
            _servProveedor = servProveedor;
            _configuration = configuration;
            _logger = logger;
            _apiLogger = apiLogger;
        }

        [HttpGet("ping")]
        public IActionResult Ping()
        {
            var pingMessage = $"DIAGNOSTIC PING - {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}";
            
            _apiLogger.WriteLog(pingMessage, "INFO", "DIAGNOSTIC");
            _logger.LogInformation("[DIAGNOSTIC] Ping endpoint llamado exitosamente");
            
            return Ok(new
            {
                success = true,
                message = "API está funcionando en IIS",
                timestamp = DateTime.Now,
                server = Environment.MachineName,
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
                processId = Environment.ProcessId,
                workingDirectory = Directory.GetCurrentDirectory(),
                baseDirectory = AppContext.BaseDirectory
            });
        }

        [HttpGet("logs")]
        public IActionResult GetLogs([FromQuery] int lines = 50)
        {
            try
            {
                var logContent = _apiLogger.ReadLogLines(lines);
                
                return Ok(new
                {
                    success = true,
                    logs = logContent,
                    timestamp = DateTime.Now,
                    totalLines = logContent?.Length ?? 0
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    success = false,
                    error = ex.Message,
                    timestamp = DateTime.Now
                });
            }
        }

        [HttpGet("logs/view")]
        public IActionResult ViewLogs([FromQuery] int lines = 100)
        {
            try
            {
                var logContent = _apiLogger.ReadLogLines(lines);
                
                var html = $@"
<!DOCTYPE html>
<html>
<head>
    <title>AdmIn API Logs</title>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1'>
    <style>
        body {{ font-family: monospace; margin: 20px; background: #1e1e1e; color: #fff; }}
        .log-container {{ background: #2d2d2d; padding: 15px; border-radius: 5px; }}
        .log-line {{ margin: 2px 0; padding: 2px 5px; }}
        .log-line:hover {{ background: #3e3e3e; }}
        .error {{ color: #ff6b6b; }}
        .success {{ color: #51cf66; }}
        .warning {{ color: #ffd43b; }}
        .info {{ color: #74c0fc; }}
        .debug {{ color: #868e96; }}
        .refresh-btn {{ 
            background: #339af0; color: white; padding: 10px 20px; 
            border: none, border-radius: 4px; cursor: pointer; margin-bottom: 15px;
        }}
        .refresh-btn:hover {{ background: #228be6; }}
        .log-stats {{ background: #2d2d2d; padding: 10px; border-radius: 5px; margin-bottom: 15px; }}
    </style>
    <script>
        function refreshLogs() {{
            window.location.reload();
        }}
        
        function autoRefresh() {{
            setInterval(refreshLogs, 10000); // Auto refresh every 10 seconds
        }}
    </script>
</head>
<body onload='autoRefresh()'>
    <h1>?? AdmIn API Logs - {DateTime.Now:yyyy-MM-dd HH:mm:ss}</h1>
    <div class='log-stats'>
        <p><strong>Servidor:</strong> {Environment.MachineName} | <strong>PID:</strong> {Environment.ProcessId} | <strong>Líneas:</strong> {logContent?.Length ?? 0}</p>
        <p><strong>Directorio Logs:</strong> {_apiLogger.GetLogsDirectory()}</p>
        <p><strong>Archivo Log:</strong> {Path.GetFileName(_apiLogger.GetLogFilePath())}</p>
    </div>
    <button class='refresh-btn' onclick='refreshLogs()'>?? Actualizar Logs</button>
    
    <div class='log-container'>
";

                if (logContent != null && logContent.Length > 0)
                {
                    for (int i = logContent.Length - 1; i >= 0; i--) // Reverse order (newest first)
                    {
                        var line = System.Web.HttpUtility.HtmlEncode(logContent[i]);
                        var cssClass = "log-line";
                        
                        if (line.Contains("[ERROR]"))
                            cssClass += " error";
                        else if (line.Contains("[WARNING]"))
                            cssClass += " warning";
                        else if (line.Contains("[DEBUG]"))
                            cssClass += " debug";
                        else if (line.Contains("[INFO]"))
                            cssClass += " info";
                        else if (line.Contains("SUCCESS") || line.Contains("?"))
                            cssClass += " success";
                            
                        html += $"<div class='{cssClass}'>{line}</div>";
                    }
                }
                else
                {
                    html += "<div class='log-line warning'>No hay logs disponibles</div>";
                }
                
                html += @"
    </div>
    <p style='margin-top: 20px; color: #868e96;'>
        Los logs se actualizan automáticamente cada 10 segundos.<br>
        Este visor muestra logs centralizados de todos los controladores.<br>
        Para ver logs de login, intenta hacer login en la aplicación.
    </p>
</body>
</html>";

                return Content(html, "text/html", System.Text.Encoding.UTF8);
            }
            catch (Exception ex)
            {
                return Content($@"
<!DOCTYPE html>
<html>
<head><title>Error - AdmIn API Logs</title></head>
<body style='font-family: monospace; background: #1e1e1e; color: #fff; padding: 20px;'>
    <h1 style='color: #ff6b6b;'>? Error al cargar logs</h1>
    <p>Error: {System.Web.HttpUtility.HtmlEncode(ex.Message)}</p>
    <p>Timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss}</p>
</body>
</html>", "text/html", System.Text.Encoding.UTF8);
            }
        }

        [HttpPost("test-login-trace")]
        public IActionResult TestLoginTrace([FromBody] object loginData)
        {
            var message = $"TEST LOGIN TRACE - Data: {loginData?.ToString() ?? "null"}";
            _apiLogger.WriteLog(message, "INFO", "DIAGNOSTIC");
            
            return Ok(new
            {
                success = true,
                message = "Test login trace guardado en logs",
                timestamp = DateTime.Now,
                logLocation = _apiLogger.GetLogFilePath()
            });
        }

        [HttpGet("test-logging")]
        public IActionResult TestLogging()
        {
            _apiLogger.WriteLog("TEST LOGGING - Probando escritura de logs", "INFO", "DIAGNOSTIC");
            _apiLogger.WriteLog("TEST LOGGING - Probando nivel WARNING", "WARNING", "DIAGNOSTIC");
            _apiLogger.WriteLog("TEST LOGGING - Probando nivel ERROR", "ERROR", "DIAGNOSTIC");
            _apiLogger.WriteLog("TEST LOGGING - Probando nivel DEBUG", "DEBUG", "DIAGNOSTIC");
            
            return Ok(new
            {
                message = "Logs de prueba escritos con diferentes niveles",
                timestamp = DateTime.Now,
                logLocation = _apiLogger.GetLogFilePath(),
                instructions = "Ve a /api/Diagnostic/logs/view para ver los logs"
            });
        }

        [HttpGet("connection")]
        public IActionResult TestConnection()
        {
            try
            {
                _apiLogger.WriteLog("CONNECTION TEST - Iniciando", "INFO", "DIAGNOSTIC");
                
                return Ok(new
                {
                    success = true,
                    connectionString = InfoSQL.Conexion?.Length > 0 ? 
                        InfoSQL.Conexion.Substring(0, Math.Min(50, InfoSQL.Conexion.Length)) + "..." : 
                        "No connection string set",
                    timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                _apiLogger.WriteLog($"CONNECTION TEST ERROR: {ex.Message}", "ERROR", "DIAGNOSTIC");
                return Ok(new
                {
                    success = false,
                    error = ex.Message,
                    timestamp = DateTime.Now
                });
            }
        }

        [HttpGet("database")]
        public async Task<IActionResult> DatabaseCheck()
        {
            try
            {
                _apiLogger.WriteLog("DATABASE CHECK - Iniciando", "INFO", "DIAGNOSTIC");
                
                using var connection = new SqlConnection(InfoSQL.Conexion);
                await connection.OpenAsync();
                
                _apiLogger.WriteLog("DATABASE CHECK - Conexión exitosa", "INFO", "DIAGNOSTIC");
                
                // Test if Usuario table exists
                var tableExistsQuery = @"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Usuario'";
                using var command = new SqlCommand(tableExistsQuery, connection);
                var tableCount = (int)await command.ExecuteScalarAsync();
                
                _apiLogger.WriteLog($"Usuario table exists: {tableCount > 0}", "INFO", "DIAGNOSTIC");
                
                if (tableCount > 0)
                {
                    // Count total records in Usuario table
                    var countQuery = "SELECT COUNT(*) FROM Usuario";
                    using var countCommand = new SqlCommand(countQuery, connection);
                    var userCount = (int)await countCommand.ExecuteScalarAsync();
                    
                    _apiLogger.WriteLog($"Total usuarios en BD: {userCount}", "INFO", "DIAGNOSTIC");
                    
                    // Get sample user emails (first 3)
                    var sampleQuery = "SELECT TOP 3 Email FROM Usuario ORDER BY UsuarioID";
                    using var sampleCommand = new SqlCommand(sampleQuery, connection);
                    var reader = await sampleCommand.ExecuteReaderAsync();
                    
                    var emails = new List<string>();
                    while (await reader.ReadAsync())
                    {
                        emails.Add(reader["Email"].ToString());
                    }
                    reader.Close();
                    
                    foreach (var email in emails)
                    {
                        _apiLogger.WriteLog($"Email usuario ejemplo: {email}", "DEBUG", "DIAGNOSTIC");
                    }
                    
                    return Ok(new
                    {
                        success = true,
                        database_connected = true,
                        usuario_table_exists = true,
                        total_users = userCount,
                        sample_emails = emails,
                        timestamp = DateTime.Now
                    });
                }
                else
                {
                    _apiLogger.WriteLog("ERROR: Tabla Usuario no existe", "ERROR", "DIAGNOSTIC");
                    return Ok(new
                    {
                        success = false,
                        error = "Tabla Usuario no existe en la base de datos",
                        timestamp = DateTime.Now
                    });
                }
            }
            catch (Exception ex)
            {
                _apiLogger.WriteLog($"DATABASE CHECK ERROR: {ex.Message}", "ERROR", "DIAGNOSTIC");
                return Ok(new
                {
                    success = false,
                    error = ex.Message,
                    timestamp = DateTime.Now
                });
            }
        }

        [HttpGet("connectivity-check")]
        public IActionResult ConnectivityCheck()
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            var message = $"CONNECTIVITY CHECK - {timestamp}";
            
            _apiLogger.WriteLog(message, "INFO", "DIAGNOSTIC");
            _apiLogger.WriteLog("API está recibiendo peticiones correctamente", "INFO", "DIAGNOSTIC");
            
            return Ok(new
            {
                success = true,
                message = "API está recibiendo peticiones correctamente",
                timestamp = DateTime.Now,
                server = Environment.MachineName,
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
                baseUrl = $"{Request.Scheme}://{Request.Host}",
                requestPath = Request.Path,
                requestMethod = Request.Method,
                userAgent = Request.Headers["User-Agent"].ToString()
            });
        }

        [HttpPost("test-auth-endpoint")]
        public IActionResult TestAuthEndpoint([FromBody] object testData)
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            var message = $"TEST AUTH ENDPOINT - {timestamp}";
            
            _apiLogger.WriteLog(message, "INFO", "DIAGNOSTIC");
            _apiLogger.WriteLog($"Petición POST recibida en test-auth-endpoint", "INFO", "DIAGNOSTIC");
            _apiLogger.WriteLog($"Datos recibidos: {testData?.ToString() ?? "null"}", "DEBUG", "DIAGNOSTIC");
            
            Console.WriteLine("========== TEST AUTH ENDPOINT ==========");
            Console.WriteLine($"[DIAGNOSTIC] {message}");
            Console.WriteLine($"[DIAGNOSTIC] Petición POST recibida correctamente");
            Console.WriteLine($"[DIAGNOSTIC] Datos: {testData?.ToString() ?? "null"}");
            
            return Ok(new
            {
                success = true,
                message = "Test auth endpoint funcionando correctamente",
                timestamp = DateTime.Now,
                receivedData = testData,
                requestInfo = new
                {
                    method = Request.Method,
                    path = Request.Path,
                    contentType = Request.ContentType,
                    hasBody = Request.ContentLength > 0
                }
            });
        }

        [HttpGet("auth-config")]
        public IActionResult AuthConfig()
        {
            try
            {
                var jwtKey = _configuration["Jwt:Key"];
                var hasJwtKey = !string.IsNullOrEmpty(jwtKey);
                
                _apiLogger.WriteLog($"AUTH CONFIG CHECK - JWT Key configured: {hasJwtKey}", "INFO", "DIAGNOSTIC");
                
                return Ok(new
                {
                    success = true,
                    jwtKeyConfigured = hasJwtKey,
                    jwtKeyLength = jwtKey?.Length ?? 0,
                    environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
                    timestamp = DateTime.Now,
                    authEndpointUrl = $"{Request.Scheme}://{Request.Host}/api/Auth/login"
                });
            }
            catch (Exception ex)
            {
                _apiLogger.WriteLog($"AUTH CONFIG ERROR: {ex.Message}", "ERROR", "DIAGNOSTIC");
                
                return Ok(new
                {
                    success = false,
                    error = ex.Message,
                    timestamp = DateTime.Now
                });
            }
        }

        [HttpPost("simulate-login")]
        public IActionResult SimulateLogin([FromBody] object loginData)
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            
            _apiLogger.WriteLog("========== SIMULATE LOGIN REQUEST ==========", "INFO", "DIAGNOSTIC");
            _apiLogger.WriteLog($"Simulate login called at {timestamp}", "INFO", "DIAGNOSTIC");
            _apiLogger.WriteLog($"Data received: {loginData?.ToString() ?? "null"}", "DEBUG", "DIAGNOSTIC");
            _apiLogger.WriteLog($"Request method: {Request.Method}", "DEBUG", "DIAGNOSTIC");
            _apiLogger.WriteLog($"Request path: {Request.Path}", "DEBUG", "DIAGNOSTIC");
            _apiLogger.WriteLog($"Content type: {Request.ContentType}", "DEBUG", "DIAGNOSTIC");
            _apiLogger.WriteLog("========== SIMULATE LOGIN END ==========", "INFO", "DIAGNOSTIC");
            
            Console.WriteLine("========== SIMULATE LOGIN REQUEST ==========");
            Console.WriteLine($"[DIAGNOSTIC] Simulate login called at {timestamp}");
            Console.WriteLine($"[DIAGNOSTIC] Data received: {loginData?.ToString() ?? "null"}");
            Console.WriteLine($"[DIAGNOSTIC] This simulates what should happen in Auth/login");
            Console.WriteLine("========== SIMULATE LOGIN END ==========");
            
            return Ok(new
            {
                success = true,
                message = "Simulate login request processed - check logs",
                timestamp = DateTime.Now,
                receivedData = loginData,
                note = "If you see this in logs but not Auth/login logs, there's a routing issue"
            });
        }

        // Helper methods for file operations
        private static void WriteToServerLogFile(string message)
        {
            try
            {
                var serverLogsPath = GetLogsDirectory();
                var logFile = Path.Combine(serverLogsPath, $"AdmIn-API-{DateTime.Now:yyyy-MM-dd}.log");
                var logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {message}{Environment.NewLine}";
                System.IO.File.AppendAllText(logFile, logMessage);
            }
            catch
            {
                // Ignore logging errors
            }
        }

        private static string GetLogsDirectory()
        {
            var serverLogsPath = @"C:\inetpub\logs\AdmIn";
            
            if (!Directory.Exists(serverLogsPath))
            {
                try
                {
                    Directory.CreateDirectory(serverLogsPath);
                    return serverLogsPath;
                }
                catch
                {
                    // Fallback to application directory
                    serverLogsPath = Path.Combine(AppContext.BaseDirectory, "Logs");
                    if (!Directory.Exists(serverLogsPath))
                    {
                        Directory.CreateDirectory(serverLogsPath);
                    }
                    return serverLogsPath;
                }
            }
            
            return serverLogsPath;
        }

        private static string GetLogFilePath()
        {
            var logsDir = GetLogsDirectory();
            return Path.Combine(logsDir, $"AdmIn-API-{DateTime.Now:yyyy-MM-dd}.log");
        }

        private static string[] ReadLogFile(int maxLines)
        {
            var logFile = GetLogFilePath();
            
            if (!System.IO.File.Exists(logFile))
            {
                return new[] { "Log file no existe aún. Intenta hacer login para generar logs." };
            }

            var allLines = System.IO.File.ReadAllLines(logFile);
            
            if (allLines.Length <= maxLines)
            {
                return allLines;
            }
            
            // Return the last 'maxLines' lines
            var result = new string[maxLines];
            Array.Copy(allLines, allLines.Length - maxLines, result, 0, maxLines);
            return result;
        }
    }
}