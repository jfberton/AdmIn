using AdmIn.Business.Servicios;
using AdmIn.Common;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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

// Register application services
builder.Services.AddScoped<IServ_Usuario, Serv_Usuario>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    InfoSQL.Conexion = app.Configuration.GetConnectionString("DevCS");
}
else if (builder.Environment.IsEnvironment("Test"))
{
    InfoSQL.Conexion = builder.Configuration.GetConnectionString("TestCS");
}
else
{
    InfoSQL.Conexion = builder.Configuration.GetConnectionString("ProdCS");
}

app.UseHttpsRedirection();

// Apply middlewares
app.UseCors("AllowAll"); // Habilitar CORS
app.UseAuthentication(); // Validación de tokens JWT
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
