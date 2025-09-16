using AdmIn.Business.Servicios;
using AdmIn.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using System;

namespace AdmIn.API.Controllers
{
    [ApiController]
    [Route("/")]
    public class HealthCheckController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public HealthCheckController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
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

        [HttpGet("health")]
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
                        uptime = DateTime.UtcNow - System.Diagnostics.Process.GetCurrentProcess().StartTime
                    }
                }
            };

            return isHealthy ? Ok(healthStatus) : StatusCode(503, healthStatus);
        }

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
                
                // Execute a simple query to verify the connection works
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
                
                // Mask sensitive information
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
    }
}
