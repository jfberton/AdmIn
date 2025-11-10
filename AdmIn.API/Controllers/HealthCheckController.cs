using AdmIn.API.Services;
using AdmIn.Common;
using AdmIn.Common.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Mail;
using System.Threading.Tasks;

namespace AdmIn.API.Controllers
{
    /// <summary>
    /// Unified Health Check and Diagnostics Controller
    /// Provides health checks for monitoring tools and diagnostic endpoints for debugging
    /// </summary>
    [ApiController]
    [Route("/")]
    public class HealthCheckController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IApiLoggerService _apiLogger;
        private readonly ILogger<HealthCheckController> _logger;
        private readonly IEmailService _emailService;

        public HealthCheckController(
            IConfiguration configuration,
            IApiLoggerService apiLogger,
            ILogger<HealthCheckController> logger,
            IEmailService emailService)
        {
            _configuration = configuration;
            _apiLogger = apiLogger;
            _logger = logger;
            _emailService = emailService;
        }

        #region Public Health Checks (for monitoring tools)

        /// <summary>
        /// Basic health check - GET /
        /// Used by load balancers and monitoring tools
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            var healthStatus = new
            {
                status = "API is active",
                timestamp = DateTime.UtcNow,
                version = "1.0.0",
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
                databaseConnection = await TestDatabaseConnection()
            };

            return Ok(healthStatus);
        }

        /// <summary>
        /// Detailed health check - GET /health
        /// Includes database and API status
        /// </summary>
        [HttpGet("health")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDetailed()
        {
            var databaseStatus = await TestDatabaseConnection();
            var isHealthy = databaseStatus == "Connected successfully";

            var healthStatus = new
            {
                status = isHealthy ? "Healthy" : "Unhealthy",
                timestamp = DateTime.UtcNow,
                version = "1.0.0",
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
                checks = new
                {
                    database = new
                    {
                        status = databaseStatus,
                        connectionString = GetMaskedConnectionString()
                    },
                    api = new
                    {
                        status = "API is running",
                        uptime = DateTime.UtcNow - Process.GetCurrentProcess().StartTime
                    }
                }
            };

            return isHealthy ? Ok(healthStatus) : StatusCode(503, healthStatus);
        }

        /// <summary>
        /// Kubernetes-style readiness probe - GET /ready
        /// Returns 200 if ready to receive traffic, 503 if not
        /// </summary>
        [HttpGet("ready")]
        [AllowAnonymous]
        public async Task<IActionResult> Ready()
        {
            var dbStatus = await TestDatabaseConnection();
            var isReady = dbStatus == "Connected successfully";

            return isReady ? Ok(new { status = "Ready" }) : StatusCode(503, new { status = "Not Ready" });
        }

        /// <summary>
        /// Kubernetes-style liveness probe - GET /live
        /// Returns 200 if process is alive
        /// </summary>
        [HttpGet("live")]
        [AllowAnonymous]
        public IActionResult Live()
        {
            return Ok(new { status = "Alive", timestamp = DateTime.UtcNow });
        }

        #endregion

        #region Diagnostic Endpoints (protected, for debugging)

        /// <summary>
        /// Ping endpoint with detailed server info - GET /diagnostic/ping
        /// Used by UI to verify connectivity before login
        /// </summary>
        [HttpGet("diagnostic/ping")]
        [AllowAnonymous] // Usado por Serv_Auth en UI para verificar conectividad
        public IActionResult Ping()
        {
            var pingMessage = $"DIAGNOSTIC PING - {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}";

            _apiLogger.WriteLog(pingMessage, "INFO", "HEALTH_CHECK");
            _logger.LogInformation("[HEALTH_CHECK] Ping endpoint llamado exitosamente");

            return Ok(new
            {
                success = true,
                message = "API está funcionando",
                timestamp = DateTime.Now,
                server = Environment.MachineName,
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
                processId = Environment.ProcessId,
                workingDirectory = Directory.GetCurrentDirectory(),
                baseDirectory = AppContext.BaseDirectory
            });
        }

        /// <summary>
        /// Get logs in JSON format - GET /diagnostic/logs?lines=50
        /// Requires admin_sistema role
        /// </summary>
        [HttpGet("diagnostic/logs")]
        [Authorize(Roles = "admin_sistema")]
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

        /// <summary>
        /// View logs in HTML with auto-refresh - GET /diagnostic/logs/view?lines=100
        /// Requires admin_sistema role
        /// </summary>
        [HttpGet("diagnostic/logs/view")]
        [Authorize(Roles = "admin_sistema")]
        public IActionResult ViewLogs([FromQuery] int lines = 100)
        {
            try
            {
                var logContent = _apiLogger.ReadLogLines(lines);
                var html = GenerateLogsHtml(logContent);
                return Content(html, "text/html", System.Text.Encoding.UTF8);
            }
            catch (Exception ex)
            {
                return Content(GenerateErrorHtml(ex.Message), "text/html", System.Text.Encoding.UTF8);
            }
        }

        /// <summary>
        /// Detailed database check - GET /diagnostic/database
        /// Requires admin_sistema role
        /// </summary>
        [HttpGet("diagnostic/database")]
        [Authorize(Roles = "admin_sistema")]
        public async Task<IActionResult> DatabaseCheck()
        {
            try
            {
                _apiLogger.WriteLog("DATABASE CHECK - Iniciando", "INFO", "HEALTH_CHECK");

                using var connection = new SqlConnection(InfoSQL.Conexion);
                await connection.OpenAsync();

                _apiLogger.WriteLog("DATABASE CHECK - Conexión exitosa", "INFO", "HEALTH_CHECK");

                var tableExistsQuery = @"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Usuario'";
                using var command = new SqlCommand(tableExistsQuery, connection);
                var tableCount = (int)await command.ExecuteScalarAsync();

                if (tableCount > 0)
                {
                    var countQuery = "SELECT COUNT(*) FROM Usuario";
                    using var countCommand = new SqlCommand(countQuery, connection);
                    var userCount = (int)await countCommand.ExecuteScalarAsync();

                    _apiLogger.WriteLog($"Total usuarios en BD: {userCount}", "INFO", "HEALTH_CHECK");

                    return Ok(new
                    {
                        success = true,
                        database_connected = true,
                        usuario_table_exists = true,
                        total_users = userCount,
                        timestamp = DateTime.Now
                    });
                }
                else
                {
                    _apiLogger.WriteLog("ERROR: Tabla Usuario no existe", "ERROR", "HEALTH_CHECK");
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
                _apiLogger.WriteLog($"DATABASE CHECK ERROR: {ex.Message}", "ERROR", "HEALTH_CHECK");
                return Ok(new
                {
                    success = false,
                    error = ex.Message,
                    timestamp = DateTime.Now
                });
            }
        }

        /// <summary>
        /// Verify JWT configuration - GET /diagnostic/auth-config
        /// Requires admin_sistema role
        /// </summary>
        [HttpGet("diagnostic/auth-config")]
        [Authorize(Roles = "admin_sistema")]
        public IActionResult AuthConfig()
        {
            try
            {
                var jwtKey = _configuration["Jwt:Key"];
                var hasJwtKey = !string.IsNullOrEmpty(jwtKey);

                _apiLogger.WriteLog($"AUTH CONFIG CHECK - JWT Key configured: {hasJwtKey}", "INFO", "HEALTH_CHECK");

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
                _apiLogger.WriteLog($"AUTH CONFIG ERROR: {ex.Message}", "ERROR", "HEALTH_CHECK");

                return Ok(new
                {
                    success = false,
                    error = ex.Message,
                    timestamp = DateTime.Now
                });
            }
        }

        /// <summary>
        /// Test logging functionality - GET /diagnostic/test-logging
        /// Requires admin_sistema role
        /// </summary>
        [HttpGet("diagnostic/test-logging")]
        [Authorize(Roles = "admin_sistema")]
        public IActionResult TestLogging()
        {
            _apiLogger.WriteLog("TEST LOGGING - Probando escritura de logs", "INFO", "HEALTH_CHECK");
            _apiLogger.WriteLog("TEST LOGGING - Probando nivel WARNING", "WARNING", "HEALTH_CHECK");
            _apiLogger.WriteLog("TEST LOGGING - Probando nivel ERROR", "ERROR", "HEALTH_CHECK");
            _apiLogger.WriteLog("TEST LOGGING - Probando nivel DEBUG", "DEBUG", "HEALTH_CHECK");

            return Ok(new
            {
                message = "Logs de prueba escritos con diferentes niveles",
                timestamp = DateTime.Now,
                logLocation = _apiLogger.GetLogFilePath(),
                instructions = "Ve a /diagnostic/logs/view para ver los logs"
            });
        }

        /// <summary>
        /// Detailed connectivity check - GET /diagnostic/connectivity-check
        /// Requires admin_sistema role
        /// </summary>
        [HttpGet("diagnostic/connectivity-check")]
        [Authorize(Roles = "admin_sistema")]
        public IActionResult ConnectivityCheck()
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            var message = $"CONNECTIVITY CHECK - {timestamp}";

            _apiLogger.WriteLog(message, "INFO", "HEALTH_CHECK");
            _apiLogger.WriteLog("API está recibiendo peticiones correctamente", "INFO", "HEALTH_CHECK");

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

        /// <summary>
        /// Test email sending functionality - POST /diagnostic/testmail
        /// Requires admin_sistema role
        /// </summary>
        [HttpPost("diagnostic/testmail")]
        [Authorize(Roles = "admin_sistema")]
        public async Task<IActionResult> TestMail([FromBody] TestEmailRequest request)
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            
            _apiLogger.WriteLog($"========== TEST EMAIL START - {timestamp} ==========", "INFO", "HEALTH_CHECK");
            _logger.LogInformation("[HEALTH_CHECK] Iniciando prueba de envío de email");

            try
            {
                // Validar request
                if (request == null || string.IsNullOrWhiteSpace(request.To))
                {
                    _apiLogger.WriteLog("ERROR: Email destinatario no proporcionado", "ERROR", "HEALTH_CHECK");
                    return BadRequest(new
                    {
                        success = false,
                        error = "Email destinatario (To) es requerido",
                        timestamp = DateTime.Now
                    });
                }

                _apiLogger.WriteLog($"Destinatario: {request.To}", "INFO", "HEALTH_CHECK");
                _apiLogger.WriteLog($"Asunto: {request.Subject ?? "Email de Prueba"}", "INFO", "HEALTH_CHECK");
                _apiLogger.WriteLog($"Tipo de prueba: {request.TestType ?? "simple"}", "INFO", "HEALTH_CHECK");

                // Obtener y validar configuración SMTP
                var smtpHost = _configuration["Smtp:Host"];
                var smtpPort = _configuration["Smtp:Port"];
                var smtpUser = _configuration["Smtp:User"];
                var smtpPass = _configuration["Smtp:Pass"];
                var enableSsl = _configuration["Smtp:EnableSsl"];
                var smtpFrom = _configuration["Smtp:From"];

                _apiLogger.WriteLog($"SMTP Host: {smtpHost ?? "(no configurado)"}", "INFO", "HEALTH_CHECK");
                _apiLogger.WriteLog($"SMTP Port: {smtpPort ?? "(no configurado)"}", "INFO", "HEALTH_CHECK");
                _apiLogger.WriteLog($"SMTP User: {smtpUser ?? "(no configurado)"}", "INFO", "HEALTH_CHECK");
                _apiLogger.WriteLog($"SMTP Pass: {(string.IsNullOrEmpty(smtpPass) ? "NO CONFIGURADO" : "***CONFIGURADO***")}", "INFO", "HEALTH_CHECK");
                _apiLogger.WriteLog($"Enable SSL: {enableSsl ?? "(no configurado)"}", "INFO", "HEALTH_CHECK");
                _apiLogger.WriteLog($"From: {smtpFrom ?? "(no configurado)"}", "INFO", "HEALTH_CHECK");

                // Validar configuración
                if (string.IsNullOrEmpty(smtpHost))
                {
                    _apiLogger.WriteLog("ERROR: SMTP Host no configurado", "ERROR", "HEALTH_CHECK");
                    return Ok(new
                    {
                        success = false,
                        error = "SMTP Host no configurado. Revisa appsettings.json o User Secrets.",
                        timestamp = DateTime.Now,
                        smtpConfig = new
                        {
                            host = smtpHost ?? "(no configurado)",
                            port = smtpPort ?? "(no configurado)",
                            user = smtpUser ?? "(no configurado)",
                            passwordConfigured = !string.IsNullOrEmpty(smtpPass),
                            sslEnabled = enableSsl ?? "(no configurado)",
                            from = smtpFrom ?? "(no configurado)"
                        }
                    });
                }

                if (string.IsNullOrEmpty(smtpUser) || string.IsNullOrEmpty(smtpPass))
                {
                    _apiLogger.WriteLog("ERROR: SMTP User o Password no configurados", "ERROR", "HEALTH_CHECK");
                    return Ok(new
                    {
                        success = false,
                        error = "SMTP User o Password no configurados. Para Gmail necesitas una 'App Password'.",
                        timestamp = DateTime.Now,
                        instructions = new[]
                        {
                            "1. Ve a https://myaccount.google.com/apppasswords",
                            "2. Genera una 'Contraseña de aplicación'",
                            "3. Usa esa contraseña (no tu contraseña normal)",
                            "4. Configura en User Secrets o appsettings"
                        }
                    });
                }

                // Preparar el cuerpo del email según el tipo
                string subject = request.Subject ?? "Email de Prueba - AdmIn API";
                string htmlBody;

                switch (request.TestType?.ToLower())
                {
                    case "password_reset":
                        var resetModel = new Dictionary<string, string>
                        {
                            { "Name", "Usuario de Prueba" },
                            { "Link", "https://localhost:7189/confirm-password-reset?token=TEST123" },
                            { "ExpiryHours", "24" }
                        };
                        htmlBody = _emailService.RenderTemplate("PasswordReset_Request.html", resetModel);
                        subject = "Restablecer contraseña - PRUEBA";
                        _apiLogger.WriteLog("Usando template: PasswordReset_Request.html", "INFO", "HEALTH_CHECK");
                        break;

                    case "new_user":
                        var newUserModel = new Dictionary<string, string>
                        {
                            { "Name", "Nuevo Usuario" },
                            { "Link", "https://localhost:7189/confirm-password-reset?token=NEWUSER123" },
                            { "ExpiryHours", "24" }
                        };
                        htmlBody = _emailService.RenderTemplate("NewUser_SetPassword.html", newUserModel);
                        subject = "Bienvenido - Configura tu contraseña - PRUEBA";
                        _apiLogger.WriteLog("Usando template: NewUser_SetPassword.html", "INFO", "HEALTH_CHECK");
                        break;

                    default:
                        htmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1'>
</head>
<body style='font-family: Arial, sans-serif; padding: 20px; background-color: #f5f5f5;'>
    <div style='max-width: 600px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 10px; box-shadow: 0 2px 4px rgba(0,0,0,0.1);'>
        <h2 style='color: #333; border-bottom: 2px solid #007bff; padding-bottom: 10px;'>📧 Email de Prueba - AdmIn API</h2>
        <p style='color: #666; line-height: 1.6;'>{request.Body ?? "Este es un email de prueba enviado desde el sistema AdmIn para verificar la configuración SMTP."}</p>
        <div style='background-color: #f8f9fa; padding: 15px; border-radius: 5px; margin-top: 20px;'>
            <p style='margin: 5px 0; color: #666;'><strong>Servidor:</strong> {Environment.MachineName}</p>
            <p style='margin: 5px 0; color: #666;'><strong>Timestamp:</strong> {DateTime.Now:yyyy-MM-dd HH:mm:ss}</p>
            <p style='margin: 5px 0; color: #666;'><strong>Environment:</strong> {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}</p>
        </div>
        <hr style='border: none; border-top: 1px solid #ddd; margin: 20px 0;'/>
        <p style='color: #999; font-size: 12px; text-align: center;'>
        Este es un email de prueba del sistema AdmIn.<br/>
          Si no esperabas recibir este email, puedes ignorarlo con seguridad.
    </p>
 </div>
</body>
</html>";
                        _apiLogger.WriteLog("Usando template simple por defecto", "INFO", "HEALTH_CHECK");
                        break;
                }

                if (string.IsNullOrEmpty(htmlBody))
                {
                    htmlBody = $"<p>{request.Body ?? "Email de prueba"}</p>";
                    _apiLogger.WriteLog("ADVERTENCIA: Template resultó vacío, usando cuerpo simple", "WARNING", "HEALTH_CHECK");
                }

                _apiLogger.WriteLog($"HTML Body generado - Length: {htmlBody.Length}", "INFO", "HEALTH_CHECK");
                _apiLogger.WriteLog("Intentando enviar email...", "INFO", "HEALTH_CHECK");

                // Intentar envío
                try
                {
                    await _emailService.SendAsync(request.To, subject, htmlBody);

                    _apiLogger.WriteLog("✓ Email enviado correctamente", "INFO", "HEALTH_CHECK");
                    _logger.LogInformation("[HEALTH_CHECK] Email de prueba enviado exitosamente a {Email}", request.To);

                    return Ok(new
                    {
                        success = true,
                        message = $"✓ Email enviado correctamente a {request.To}",
                        timestamp = DateTime.Now,
                        details = new
                        {
                            to = request.To,
                            subject = subject,
                            smtpConfig = new
                            {
                                host = smtpHost,
                                port = smtpPort,
                                ssl = enableSsl,
                                user = smtpUser,
                                from = smtpFrom
                            },
                            bodyLength = htmlBody.Length,
                            testType = request.TestType ?? "simple"
                        }
                    });
                }
                catch (SmtpException smtpEx)
                {
                    _apiLogger.WriteLog($"✗ SMTP Exception: {smtpEx.Message}", "ERROR", "HEALTH_CHECK");
                    _apiLogger.WriteLog($"Status Code: {smtpEx.StatusCode}", "ERROR", "HEALTH_CHECK");
                    _apiLogger.WriteLog($"Stack Trace: {smtpEx.StackTrace}", "ERROR", "HEALTH_CHECK");
                    _logger.LogError(smtpEx, "[HEALTH_CHECK] Error SMTP al enviar email de prueba");

                    var errorMsg = $"✗ Error SMTP: {smtpEx.Message}";
                    var suggestions = new List<string>();

                    if (smtpEx.Message.Contains("Authentication", StringComparison.OrdinalIgnoreCase) || 
                        smtpEx.StatusCode == SmtpStatusCode.GeneralFailure)
                    {
                        suggestions.Add("Problema de autenticación detectado");
                        suggestions.Add("Para Gmail:");
                        suggestions.Add("1. Ve a https://myaccount.google.com/apppasswords");
                        suggestions.Add("2. Genera una 'Contraseña de aplicación'");
                        suggestions.Add("3. Usa esa contraseña (no tu contraseña normal)");
                        suggestions.Add("4. Configura en User Secrets:");
                        suggestions.Add("   - Smtp:Host = smtp.gmail.com");
                        suggestions.Add("   - Smtp:Port = 587");
                        suggestions.Add("   - Smtp:EnableSsl = true");
                        suggestions.Add("   - Smtp:User = tu@gmail.com");
                        suggestions.Add("   - Smtp:Pass = (contraseña de aplicación)");
                    }
                    else if (smtpEx.Message.Contains("timed out", StringComparison.OrdinalIgnoreCase))
                    {
                        suggestions.Add("Timeout de conexión");
                        suggestions.Add("Verifica:");
                        suggestions.Add("1. Firewall no está bloqueando el puerto");
                        suggestions.Add("2. Host y puerto son correctos");
                        suggestions.Add("3. Conexión a internet está funcionando");
                    }
                    else if (smtpEx.Message.Contains("SSL", StringComparison.OrdinalIgnoreCase) || 
                             smtpEx.Message.Contains("TLS", StringComparison.OrdinalIgnoreCase))
                    {
                        suggestions.Add("Problema con SSL/TLS");
                        suggestions.Add("Verifica que EnableSsl esté configurado correctamente");
                        suggestions.Add("Para Gmail debe ser: true");
                    }

                    return Ok(new
                    {
                        success = false,
                        error = errorMsg,
                        smtpStatusCode = smtpEx.StatusCode.ToString(),
                        innerException = smtpEx.InnerException?.Message,
                        timestamp = DateTime.Now,
                        suggestions = suggestions.Any() ? suggestions : new List<string> { "Error SMTP general. Revisa la configuración." },
                        smtpConfig = new
                        {
                            host = smtpHost,
                            port = smtpPort,
                            ssl = enableSsl,
                            user = smtpUser,
                            from = smtpFrom
                        }
                    });
                }
                catch (Exception ex)
                {
                    _apiLogger.WriteLog($"✗ Exception general: {ex.GetType().Name}", "ERROR", "HEALTH_CHECK");
                    _apiLogger.WriteLog($"Message: {ex.Message}", "ERROR", "HEALTH_CHECK");
                    _apiLogger.WriteLog($"Stack Trace: {ex.StackTrace}", "ERROR", "HEALTH_CHECK");
                    _logger.LogError(ex, "[HEALTH_CHECK] Error general al enviar email de prueba");

                    if (ex.InnerException != null)
                    {
                        _apiLogger.WriteLog($"Inner Exception: {ex.InnerException.Message}", "ERROR", "HEALTH_CHECK");
                    }

                    return Ok(new
                    {
                        success = false,
                        error = $"✗ Error: {ex.Message}",
                        exceptionType = ex.GetType().Name,
                        innerException = ex.InnerException?.Message,
                        stackTrace = ex.StackTrace,
                        timestamp = DateTime.Now
                    });
                }
            }
            catch (Exception ex)
            {
                _apiLogger.WriteLog($"✗ ERROR CRÍTICO en test de email: {ex.Message}", "ERROR", "HEALTH_CHECK");
                _logger.LogError(ex, "[HEALTH_CHECK] Error crítico en endpoint de prueba de email");

                return StatusCode(500, new
                {
                    success = false,
                    error = $"Error crítico: {ex.Message}",
                    exceptionType = ex.GetType().Name,
                    timestamp = DateTime.Now
                });
            }
            finally
            {
                _apiLogger.WriteLog($"========== TEST EMAIL END - {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} ==========", "INFO", "HEALTH_CHECK");
            }
        }

        /// <summary>
        /// Get SMTP configuration status - GET /diagnostic/smtp-config
        /// Requires admin_sistema role
        /// </summary>
        [HttpGet("diagnostic/smtp-config")]
        [Authorize(Roles = "admin_sistema")]
        public IActionResult GetSmtpConfig()
        {
            try
            {
                var smtpHost = _configuration["Smtp:Host"];
                var smtpPort = _configuration["Smtp:Port"];
                var smtpUser = _configuration["Smtp:User"];
                var smtpPass = _configuration["Smtp:Pass"];
                var enableSsl = _configuration["Smtp:EnableSsl"];
                var smtpFrom = _configuration["Smtp:From"];

                var isConfigured = !string.IsNullOrEmpty(smtpHost) && 
                                   !string.IsNullOrEmpty(smtpUser) && 
                                   !string.IsNullOrEmpty(smtpPass);

                _apiLogger.WriteLog($"SMTP Config Check - Configured: {isConfigured}", "INFO", "HEALTH_CHECK");

                return Ok(new
                {
                    success = true,
                    isConfigured = isConfigured,
                    timestamp = DateTime.Now,
                    config = new
                    {
                        host = smtpHost ?? "(no configurado)",
                        port = smtpPort ?? "(no configurado)",
                        user = smtpUser ?? "(no configurado)",
                        passwordConfigured = !string.IsNullOrEmpty(smtpPass),
                        enableSsl = enableSsl ?? "(no configurado)",
                        from = smtpFrom ?? "(no configurado)"
                    },
                    warnings = GetSmtpConfigWarnings(smtpHost, smtpUser, smtpPass, enableSsl)
                });
            }
            catch (Exception ex)
            {
                _apiLogger.WriteLog($"ERROR obteniendo configuración SMTP: {ex.Message}", "ERROR", "HEALTH_CHECK");
                return Ok(new
                {
                    success = false,
                    error = ex.Message,
                    timestamp = DateTime.Now
                });
            }
        }

        #endregion

        #region Helper Methods

        private async Task<string> TestDatabaseConnection()
        {
            try
            {
                if (string.IsNullOrEmpty(InfoSQL.Conexion))
                {
                    return "Connection string not configured";
                }

                using var connection = new SqlConnection(InfoSQL.Conexion);
                await connection.OpenAsync();

                using var command = new SqlCommand("SELECT 1", connection);
                await command.ExecuteScalarAsync();

                return "Connected successfully";
            }
            catch (Exception ex)
            {
                return $"Connection failed: {ex.Message}";
            }
        }

        private string GetMaskedConnectionString()
        {
            if (string.IsNullOrEmpty(InfoSQL.Conexion))
                return "Not configured";

            try
            {
                var builder = new SqlConnectionStringBuilder(InfoSQL.Conexion);

                if (!string.IsNullOrEmpty(builder.Password))
                    builder.Password = "***";
                if (!string.IsNullOrEmpty(builder.UserID))
                    builder.UserID = "***";

                return builder.ConnectionString;
            }
            catch
            {
                return "Invalid connection string format";
            }
        }

        private string GenerateLogsHtml(string[] logContent)
        {
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
            border: none; border-radius: 4px; cursor: pointer; margin-bottom: 15px;
        }}
        .refresh-btn:hover {{ background: #228be6; }}
        .log-stats {{ background: #2d2d2d; padding: 10px; border-radius: 5px; margin-bottom: 15px; }}
    </style>
    <script>
        function refreshLogs() {{ window.location.reload(); }}
        function autoRefresh() {{ setInterval(refreshLogs, 10000); }}
    </script>
</head>
<body onload='autoRefresh()'>
    <h1>📊 AdmIn API Logs - {DateTime.Now:yyyy-MM-dd HH:mm:ss}</h1>
    <div class='log-stats'>
        <p><strong>Servidor:</strong> {Environment.MachineName} | <strong>PID:</strong> {Environment.ProcessId} | <strong>Líneas:</strong> {logContent?.Length ?? 0}</p>
        <p><strong>Directorio Logs:</strong> {_apiLogger.GetLogsDirectory()}</p>
        <p><strong>Archivo Log:</strong> {Path.GetFileName(_apiLogger.GetLogFilePath())}</p>
    </div>
    <button class='refresh-btn' onclick='refreshLogs()'>🔄 Actualizar Logs</button>
    <div class='log-container'>";

            if (logContent != null && logContent.Length > 0)
            {
                for (int i = logContent.Length - 1; i >= 0; i--)
                {
                    var line = System.Web.HttpUtility.HtmlEncode(logContent[i]);
                    var cssClass = "log-line";

                    if (line.Contains("[ERROR]")) cssClass += " error";
                    else if (line.Contains("[WARNING]")) cssClass += " warning";
                    else if (line.Contains("[DEBUG]")) cssClass += " debug";
                    else if (line.Contains("[INFO]")) cssClass += " info";
                    else if (line.Contains("SUCCESS") || line.Contains("✓")) cssClass += " success";

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
        Este visor muestra logs centralizados de todos los controladores.
    </p>
</body>
</html>";

            return html;
        }

        private string GenerateErrorHtml(string errorMessage)
        {
            return $@"
<!DOCTYPE html>
<html>
<head><title>Error - AdmIn API Logs</title></head>
<body style='font-family: monospace; background: #1e1e1e; color: #fff; padding: 20px;'>
    <h1 style='color: #ff6b6b;'>❌ Error al cargar logs</h1>
    <p>Error: {System.Web.HttpUtility.HtmlEncode(errorMessage)}</p>
    <p>Timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss}</p>
</body>
</html>";
        }

        private List<string> GetSmtpConfigWarnings(string host, string user, string pass, string ssl)
        {
            var warnings = new List<string>();

            if (string.IsNullOrEmpty(host))
                warnings.Add("⚠️ SMTP Host no configurado");
            
            if (string.IsNullOrEmpty(user))
                warnings.Add("⚠️ SMTP User no configurado");
      
            if (string.IsNullOrEmpty(pass))
                warnings.Add("⚠️ SMTP Password no configurado");
 
            if (string.IsNullOrEmpty(ssl))
                warnings.Add("⚠️ Enable SSL no configurado");
     
            if (!string.IsNullOrEmpty(host) && host.Contains("gmail", StringComparison.OrdinalIgnoreCase))
            {
                warnings.Add("ℹ️ Para Gmail, asegúrate de usar una 'App Password', no tu contraseña normal");
                if (ssl != "true")
                    warnings.Add("⚠️ Gmail requiere SSL habilitado (debe ser 'true')");
            }

            return warnings;
        }

        #endregion
    }

    /// <summary>
    /// Request model for email testing
    /// </summary>
    public class TestEmailRequest
    {
        /// <summary>
        /// Email destinatario
        /// </summary>
        public string To { get; set; } = string.Empty;

        /// <summary>
        /// Asunto del email (opcional)
        /// </summary>
      public string? Subject { get; set; }

        /// <summary>
        /// Cuerpo del mensaje (opcional)
     /// </summary>
    public string? Body { get; set; }

  /// <summary>
        /// Tipo de prueba: simple, password_reset, new_user
   /// </summary>
   public string? TestType { get; set; } = "simple";
    }
}
