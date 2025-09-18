using AdmIn.Business.Servicios;
using AdmIn.Common;
using AdmIn.Data.Repositorios;
using AdmIn.Common.Repositorios;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.StaticFiles;
using AdmIn.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Swagger/OpenAPI Configuration
builder.Services.AddEndpointsApiExplorer();

// JWT Authentication Configuration
builder.Services.AddAuthorization();
builder.Services.AddAuthentication("Bearer").AddJwtBearer(op =>
{
    var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]));

    op.RequireHttpsMetadata = false; // Para entornos locales, evita problemas con certificados
    op.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        IssuerSigningKey = signingKey
    };
});

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Enhanced Logging Configuration for IIS
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    
    // Console logging (para desarrollo y debug en IIS)
    logging.AddConsole(options =>
    {
        options.IncludeScopes = true;
        options.TimestampFormat = "yyyy-MM-dd HH:mm:ss.fff ";
    });
    
    // Debug logging
    logging.AddDebug();
    
    // Event Log (para producción en Windows Server)
    if (OperatingSystem.IsWindows())
    {
        try
        {
            logging.AddEventLog(settings =>
            {
                settings.SourceName = "AdmIn.API";
                settings.LogName = "Application";
            });
        }
        catch
        {
            // Ignore if Event Log is not available
        }
    }
});

// Register centralized logging service
builder.Services.AddScoped<IApiLoggerService, ApiLoggerService>();

// Register application services
builder.Services.AddScoped<IServ_Usuario, Serv_Usuario>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

// Register Rol services
builder.Services.AddScoped<IServ_Rol, Serv_Rol>();
builder.Services.AddScoped<IRolRepository, RolRepository>();

// Register Moneda services
builder.Services.AddScoped<IServ_Moneda, Serv_Moneda>();
builder.Services.AddScoped<IMonedaRepository, MonedaRepository>();

// Register Inmueble services
builder.Services.AddScoped<IServ_Inmueble, Serv_Inmueble>();
builder.Services.AddScoped<IInmuebleRepository, InmuebleRepository>();

// Register InmuebleCondicion services
builder.Services.AddScoped<IServ_InmuebleCondicion, Serv_InmuebleCondicion>();
builder.Services.AddScoped<IInmuebleCondicionRepository, InmuebleCondicionRepository>();

// Register Caracteristica services
builder.Services.AddScoped<IServ_Caracteristica, Serv_Caracteristica>();
builder.Services.AddScoped<ICaracteristicaRepository, CaracteristicaRepository>();

// Register TipoServicio services
builder.Services.AddScoped<IServ_TipoServicio, Serv_TipoServicio>();
builder.Services.AddScoped<ITipoServicioRepository, TipoServicioRepository>();

// Register Proveedor services (Serv_Proveedor needs both IProveedorRepository and IUsuarioRepository)
builder.Services.AddScoped<IServ_Proveedor, Serv_Proveedor>();
builder.Services.AddScoped<IProveedorRepository, ProveedorRepository>();

// Servicios de imágenes
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();
builder.Services.AddScoped<IServ_ImagenUpload, Serv_ImagenUpload>();
builder.Services.AddScoped<IImagenRepository, ImagenRepository>();

// Password Service with bcrypt
builder.Services.AddScoped<IPasswordService, PasswordService>();

// Static Files and File Content Type Provider
builder.Services.AddSingleton<IContentTypeProvider, FileExtensionContentTypeProvider>();

var app = builder.Build();

// Helper method for file logging optimizado para IIS usando el servicio centralizado
static async void WriteToLogFile(string message)
{
    try
    {
        // En IIS, usar una ruta fija accesible
        var serverLogsPath = @"C:\inetpub\logs\AdmIn";
        
        // Si no tenemos permisos en C:\inetpub, usar la carpeta de la aplicación
        if (!Directory.Exists(serverLogsPath))
        {
            try
            {
                Directory.CreateDirectory(serverLogsPath);
            }
            catch
            {
                // Fallback: usar carpeta de la aplicación
                serverLogsPath = Path.Combine(AppContext.BaseDirectory, "Logs");
                if (!Directory.Exists(serverLogsPath))
                {
                    Directory.CreateDirectory(serverLogsPath);
                }
            }
        }
        
        var logFile = Path.Combine(serverLogsPath, $"AdmIn-API-{DateTime.Now:yyyy-MM-dd}.log");
        var logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] [INFO] [STARTUP] {message}{Environment.NewLine}";
        await File.AppendAllTextAsync(logFile, logMessage);
    }
    catch
    {
        // Ignore logging errors to prevent breaking the application
    }
}

// Configure the HTTP request pipeline.
var environmentName = app.Environment.EnvironmentName;
var machineName = Environment.MachineName;
var processId = Environment.ProcessId;

WriteToLogFile($"===== API STARTUP ===== Environment: {environmentName}, Machine: {machineName}, PID: {processId}");

// Configure connection string based on environment
if (app.Environment.IsDevelopment())
{
    InfoSQL.Conexion = app.Configuration.GetConnectionString("DevCS");
    WriteToLogFile("Entorno: Development, Connection String Key: DevCS");
}
else if (builder.Environment.IsEnvironment("Test"))
{
    InfoSQL.Conexion = builder.Configuration.GetConnectionString("TestCS");
    WriteToLogFile("Entorno: Test, Connection String Key: TestCS");
}
else
{
    InfoSQL.Conexion = builder.Configuration.GetConnectionString("ProdCS");
    WriteToLogFile("Entorno: Production, Connection String Key: ProdCS");
}

WriteToLogFile($"Connection String configurado, length: {InfoSQL.Conexion?.Length ?? 0}");

// Verify JWT configuration
var jwtKey = app.Configuration["Jwt:Key"];
WriteToLogFile($"JWT Key configurado: {(!string.IsNullOrEmpty(jwtKey) ? "SÍ" : "NO")} (length: {jwtKey?.Length ?? 0})");

// Test connection at startup (for all environments in server)
try
{
    using var connection = new Microsoft.Data.SqlClient.SqlConnection(InfoSQL.Conexion);
    await connection.OpenAsync();
    WriteToLogFile("DATABASE CONNECTION SUCCESS");
    
    var cmd = connection.CreateCommand();
    cmd.CommandText = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Usuario'";
    var userTableExists = (int)await cmd.ExecuteScalarAsync();
    WriteToLogFile($"Usuario table exists: {userTableExists > 0}");
    
    if (userTableExists > 0)
    {
        cmd.CommandText = "SELECT COUNT(*) FROM Usuario";
        var userCount = (int)await cmd.ExecuteScalarAsync();
        WriteToLogFile($"Total users in database: {userCount}");
    }
    
    connection.Close();
}
catch (Exception ex)
{
    WriteToLogFile($"DATABASE CONNECTION ERROR: {ex.Message}");
}

WriteToLogFile($"API STARTED SUCCESSFULLY - Machine: {machineName}, PID: {processId}");
WriteToLogFile("Centralized logging service registered and ready");

// Enable static files serving
app.UseStaticFiles();

app.UseHttpsRedirection();

// Apply middlewares
app.UseCors("AllowAll"); // Habilitar CORS
app.UseAuthentication(); // Validación de tokens JWT
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
