using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AdmIn.Common;
using AdmIn.Business.Servicios;
using System.Security.Claims;
using Microsoft.Data.SqlClient;

namespace AdmIn.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiagnosticController : ControllerBase
    {
        private readonly IServ_Rol _servRol;
        private readonly IServ_Usuario _servUsuario;
        private readonly IServ_Proveedor _servProveedor;

        public DiagnosticController(IServ_Rol servRol, IServ_Usuario servUsuario, IServ_Proveedor servProveedor)
        {
            _servRol = servRol;
            _servUsuario = servUsuario;
            _servProveedor = servProveedor;
        }

        [HttpGet("connection")]
        public IActionResult TestConnection()
        {
            try
            {
                Console.WriteLine("[DIAGNOSTIC] ===== CONNECTION TEST =====");
                Console.WriteLine($"[DIAGNOSTIC] Connection String Length: {InfoSQL.Conexion?.Length ?? 0}");
                
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
                Console.WriteLine($"[DIAGNOSTIC] Connection test error: {ex.Message}");
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
                Console.WriteLine("[DIAGNOSTIC] ===== DATABASE CHECK =====");
                Console.WriteLine($"[DIAGNOSTIC] Connection String: {InfoSQL.Conexion?.Substring(0, Math.Min(80, InfoSQL.Conexion?.Length ?? 0)) + "..."}");

                using var connection = new SqlConnection(InfoSQL.Conexion);
                await connection.OpenAsync();
                
                Console.WriteLine("[DIAGNOSTIC] Database connection successful");
                
                // Test if Proveedor table exists
                var tableExistsQuery = @"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Proveedor'";
                using var command = new SqlCommand(tableExistsQuery, connection);
                var tableCount = (int)await command.ExecuteScalarAsync();
                
                Console.WriteLine($"[DIAGNOSTIC] Proveedor table exists: {tableCount > 0}");
                
                if (tableCount > 0)
                {
                    // Count total records in Proveedor table
                    var countQuery = "SELECT COUNT(*) FROM Proveedor";
                    using var countCommand = new SqlCommand(countQuery, connection);
                    var totalRecords = (int)await countCommand.ExecuteScalarAsync();
                    
                    Console.WriteLine($"[DIAGNOSTIC] Total Proveedor records: {totalRecords}");
                    
                    // Count active records
                    var activeCountQuery = "SELECT COUNT(*) FROM Proveedor WHERE Activo = 1";
                    using var activeCountCommand = new SqlCommand(activeCountQuery, connection);
                    var activeRecords = (int)await activeCountCommand.ExecuteScalarAsync();
                    
                    Console.WriteLine($"[DIAGNOSTIC] Active Proveedor records: {activeRecords}");
                    
                    return Ok(new
                    {
                        connectionSuccess = true,
                        tableExists = true,
                        totalRecords = totalRecords,
                        activeRecords = activeRecords,
                        connectionString = InfoSQL.Conexion?.Substring(0, Math.Min(50, InfoSQL.Conexion.Length)) + "..."
                    });
                }
                else
                {
                    Console.WriteLine("[DIAGNOSTIC] Proveedor table does not exist");
                    return Ok(new
                    {
                        connectionSuccess = true,
                        tableExists = false,
                        message = "Proveedor table does not exist",
                        connectionString = InfoSQL.Conexion?.Substring(0, Math.Min(50, InfoSQL.Conexion.Length)) + "..."
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DIAGNOSTIC] Database check error: {ex.Message}");
                Console.WriteLine($"[DIAGNOSTIC] StackTrace: {ex.StackTrace}");
                
                return Ok(new
                {
                    connectionSuccess = false,
                    error = ex.Message,
                    stackTrace = ex.StackTrace,
                    connectionString = InfoSQL.Conexion?.Substring(0, Math.Min(50, InfoSQL.Conexion?.Length ?? 0)) + "..."
                });
            }
        }

        [HttpGet("providers-direct")]
        public async Task<IActionResult> ProvidersDirectCheck()
        {
            try
            {
                Console.WriteLine("[DIAGNOSTIC] ===== DIRECT PROVIDERS CHECK =====");
                
                using var connection = new SqlConnection(InfoSQL.Conexion);
                await connection.OpenAsync();
                
                var query = @"SELECT 
                    ProveedorID as Id, 
                    Nombre, 
                    RFC, 
                    Email, 
                    Telefono, 
                    Direccion, 
                    UsuarioId, 
                    Activo, 
                    FechaCreacion, 
                    FechaModificacion, 
                    UsuarioCreadorId, 
                    UsuarioModificadorId 
                FROM Proveedor 
                WHERE Activo = 1 
                ORDER BY Nombre";
                
                Console.WriteLine($"[DIAGNOSTIC] Executing query: {query}");
                
                using var command = new SqlCommand(query, connection);
                using var reader = await command.ExecuteReaderAsync();
                
                var providers = new List<object>();
                while (await reader.ReadAsync())
                {
                    providers.Add(new
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        Nombre = reader.IsDBNull(reader.GetOrdinal("Nombre")) ? null : reader.GetString(reader.GetOrdinal("Nombre")),
                        RFC = reader.IsDBNull(reader.GetOrdinal("RFC")) ? null : reader.GetString(reader.GetOrdinal("RFC")),
                        Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString(reader.GetOrdinal("Email")),
                        Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? null : reader.GetString(reader.GetOrdinal("Telefono")),
                        Direccion = reader.IsDBNull(reader.GetOrdinal("Direccion")) ? null : reader.GetString(reader.GetOrdinal("Direccion")),
                        UsuarioId = reader.IsDBNull(reader.GetOrdinal("UsuarioId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("UsuarioId")),
                        Activo = reader.GetBoolean(reader.GetOrdinal("Activo")),
                        FechaCreacion = reader.GetDateTime(reader.GetOrdinal("FechaCreacion")),
                        FechaModificacion = reader.GetDateTime(reader.GetOrdinal("FechaModificacion"))
                    });
                }
                
                Console.WriteLine($"[DIAGNOSTIC] Found {providers.Count} active providers");
                
                return Ok(new
                {
                    success = true,
                    count = providers.Count,
                    providers = providers
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DIAGNOSTIC] Direct providers check error: {ex.Message}");
                
                return Ok(new
                {
                    success = false,
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        [HttpGet("providers-service")]
        public async Task<IActionResult> ProvidersServiceCheck()
        {
            try
            {
                Console.WriteLine("[DIAGNOSTIC] ===== PROVIDERS SERVICE CHECK =====");
                
                var result = await _servProveedor.Obtener_activos();
                
                Console.WriteLine($"[DIAGNOSTIC] Service result - Success: {result.Correcto}, Message: {result.Mensaje}");
                Console.WriteLine($"[DIAGNOSTIC] Providers count: {result.Datos?.Count() ?? 0}");
                
                return Ok(new
                {
                    success = result.Correcto,
                    message = result.Mensaje,
                    count = result.Datos?.Count() ?? 0,
                    providers = result.Datos?.Take(5).Select(p => new { p.Id, p.Nombre, p.RFC, p.Activo }).ToList()
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DIAGNOSTIC] Providers service check error: {ex.Message}");
                
                return Ok(new
                {
                    success = false,
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        [HttpGet("roles")]
        public async Task<IActionResult> GetAllRoles()
        {
            try
            {
                var result = await _servRol.Obtener_todos();
                return Ok(new
                {
                    success = result.Correcto,
                    message = result.Mensaje,
                    roles = result.Datos?.Select(r => new { r.Id, r.Nombre }).ToList(),
                    count = result.Datos?.Count() ?? 0
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    success = false,
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        [HttpGet("current-user-claims")]
        [Authorize]
        public IActionResult GetCurrentUserClaims()
        {
            try
            {
                var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
                var roles = User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
                var userName = User.Identity?.Name;

                return Ok(new
                {
                    success = true,
                    userName = userName,
                    isAuthenticated = User.Identity?.IsAuthenticated ?? false,
                    roles = roles,
                    allClaims = claims
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    success = false,
                    error = ex.Message
                });
            }
        }

        [HttpGet("test-admin-role")]
        [Authorize(Roles = "admin_usuario")]
        public IActionResult TestAdminRole()
        {
            return Ok(new
            {
                success = true,
                message = "Successfully accessed admin_usuario protected endpoint",
                user = User.Identity?.Name
            });
        }

        [HttpGet("test-any-auth")]
        [Authorize]
        public IActionResult TestAnyAuth()
        {
            return Ok(new
            {
                success = true,
                message = "Successfully accessed any authenticated endpoint",
                user = User.Identity?.Name,
                roles = User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList()
            });
        }
    }
}