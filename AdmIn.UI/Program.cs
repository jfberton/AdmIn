using AdmIn.UI.Authentication;
using AdmIn.UI.Components;
using AdmIn.UI.Services;
using AdmIn.UI.Services.UtilityServices;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

Console.WriteLine("[UI PROGRAM] ===== CONFIGURACIÓN DE UI =====");
Console.WriteLine($"[UI PROGRAM] Environment: {builder.Environment.EnvironmentName}");
Console.WriteLine($"[UI PROGRAM] Is Development: {builder.Environment.IsDevelopment()}");

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddRadzenComponents();
builder.Services.AddHttpClient();

// Logging Configuration for UI
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.AddDebug();
});

//Authentication 
builder.Services.AddAuthenticationCore();
builder.Services.AddScoped<ProtectedSessionStorage>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

//UI Services - These make HTTP calls to the API
builder.Services.AddScoped<IServ_Auth, Serv_Auth>();
builder.Services.AddScoped<IServ_Usuario, Serv_Usuario>();
builder.Services.AddScoped<IServ_Rol, Serv_Rol>();
builder.Services.AddScoped<IServ_Moneda, Serv_Moneda>();
builder.Services.AddScoped<IServ_Inmueble, Serv_Inmueble>();
builder.Services.AddScoped<IServ_InmuebleCondicion, Serv_InmuebleCondicion>();
builder.Services.AddScoped<IServ_Caracteristica, Serv_Caracteristica>();
builder.Services.AddScoped<IServ_TipoServicio, Serv_TipoServicio>();
builder.Services.AddScoped<IServ_Proveedor, Serv_Proveedor>();
builder.Services.AddScoped<IServ_Imagen, Serv_Imagen>();
builder.Services.AddScoped<IServ_TrabajoProveedor, Serv_TrabajoProveedor>();
builder.Services.AddScoped<IServ_CalificacionProveedor, Serv_CalificacionProveedor>();
builder.Services.AddScoped<IServ_ReservaUi, Serv_Reserva>();

// Radzen services
builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<IServ_Notificacion, Serv_Notificacion>();
builder.Services.AddScoped<IServ_HistorialTrabajo, Serv_HistorialTrabajo>();
builder.Services.AddScoped<IServ_HistorialDetalleTrabajo, Serv_HistorialDetalleTrabajo>();
builder.Services.AddScoped<IServ_DetalleTrabajo, Serv_DetalleTrabajo>();
builder.Services.AddScoped<AdmIn.UI.Components.Shared.ReservaDialog>();

//Utility Services
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<PageTitleService>();
builder.Services.AddScoped<DeviceService>();
builder.Services.AddScoped<LogHelper>();
builder.Services.AddScoped<ITokenService, TokenService>();

Console.WriteLine("[UI PROGRAM] ? Servicios registrados correctamente");

// Check API configuration
var pathApiDev = builder.Configuration["Path_api_dev"];
var pathApiProd = builder.Configuration["Path_api_prod"];
Console.WriteLine($"[UI PROGRAM] ? Path API Dev configurado: {pathApiDev ?? "NO CONFIGURADO"}");
Console.WriteLine($"[UI PROGRAM] ? Path API Prod configurado: {pathApiProd ?? "NO CONFIGURADO"}");

var app = builder.Build();

Console.WriteLine("[UI PROGRAM] ===== APLICACIÓN UI =====");
Console.WriteLine($"[UI PROGRAM] ? Aplicación construida correctamente");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    Console.WriteLine($"[UI PROGRAM] ? Configuración de producción aplicada");
}
else
{
    Console.WriteLine($"[UI PROGRAM] ? Modo desarrollo - sin HSTS");
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

Console.WriteLine("[UI PROGRAM] ? Pipeline configurado correctamente");
Console.WriteLine("[UI PROGRAM] ? Componentes Razor mapeados");
Console.WriteLine("[UI PROGRAM] ? Logging habilitado en consola");
Console.WriteLine("[UI PROGRAM] ===== UI INICIADA =====");

app.Run();
