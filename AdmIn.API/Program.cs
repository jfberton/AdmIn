using AdmIn.Business.Servicios;
using AdmIn.Common;
using AdmIn.Data.Repositorios;
using AdmIn.Common.Repositorios;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.StaticFiles;

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

// Logging Configuration
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.AddDebug();
});

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    InfoSQL.Conexion = app.Configuration.GetConnectionString("DevCS");
    Console.WriteLine($"[PROGRAM] ===== CONFIGURACIÓN DE CONEXIÓN =====");
    Console.WriteLine($"[PROGRAM] Entorno: Development");
    Console.WriteLine($"[PROGRAM] Connection String configurado: {InfoSQL.Conexion?.Substring(0, Math.Min(80, InfoSQL.Conexion?.Length ?? 0)) + "..."}");
}
else if (builder.Environment.IsEnvironment("Test"))
{
    InfoSQL.Conexion = builder.Configuration.GetConnectionString("TestCS");
    Console.WriteLine($"[PROGRAM] Entorno: Test");
    Console.WriteLine($"[PROGRAM] Connection String configurado: {InfoSQL.Conexion?.Substring(0, Math.Min(80, InfoSQL.Conexion?.Length ?? 0)) + "..."}");
}
else
{
    InfoSQL.Conexion = builder.Configuration.GetConnectionString("ProdCS");
    Console.WriteLine($"[PROGRAM] Entorno: Production");
    Console.WriteLine($"[PROGRAM] Connection String configurado: {InfoSQL.Conexion?.Substring(0, Math.Min(80, InfoSQL.Conexion?.Length ?? 0)) + "..."}");
}

Console.WriteLine($"[PROGRAM] ===== APLICACIÓN INICIADA =====");
Console.WriteLine($"[PROGRAM] API iniciada correctamente");
Console.WriteLine($"[PROGRAM] Logging habilitado en consola");

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
