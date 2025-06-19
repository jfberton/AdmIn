using AdmIn.UI.Authentication;
using AdmIn.UI.Components;
using AdmIn.UI.Services;
using AdmIn.UI.Services.UtilityServices;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


builder.Services.AddRadzenComponents();
builder.Services.AddHttpClient();

//Autenticacion 
builder.Services.AddAuthenticationCore();
builder.Services.AddScoped<ProtectedSessionStorage>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();



//Mis servicios
builder.Services.AddScoped<IServ_Auth, Serv_Auth>();
builder.Services.AddScoped<IServ_Usuario, Serv_Usuario>();
builder.Services.AddScoped<IServ_Permiso, Serv_Permiso>();
builder.Services.AddScoped<IServ_Rol, Serv_Rol>();

//Servicio de mock
// builder.Services.AddScoped<IServ_Mock, MockData>(); // Removed
builder.Services.AddScoped<AdmIn.UI.Services.Mock.IPersonaService, AdmIn.UI.Services.Mock.MockPersonaService>();
builder.Services.AddScoped<AdmIn.UI.Services.Mock.IInmuebleService, AdmIn.UI.Services.Mock.MockInmuebleService>();
builder.Services.AddScoped<AdmIn.UI.Services.Mock.IEmpleadoService, AdmIn.UI.Services.Mock.MockEmpleadoService>();
builder.Services.AddScoped<AdmIn.UI.Services.Mock.IUsuarioService, AdmIn.UI.Services.Mock.MockUsuarioService>();
builder.Services.AddScoped<AdmIn.UI.Services.Mock.IContratoService, AdmIn.UI.Services.Mock.MockContratoService>();
builder.Services.AddScoped<AdmIn.UI.Services.Mock.IReparacionService, AdmIn.UI.Services.Mock.MockReparacionService>();

//Mis Servicios Utiles
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<PageTitleService>();
builder.Services.AddScoped<DeviceService>();
builder.Services.AddScoped<LogHelper>();
builder.Services.AddScoped<ITokenService, TokenService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
