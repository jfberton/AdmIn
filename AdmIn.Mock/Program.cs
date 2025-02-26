using AdmIn.Mock.Components;
using AdmIn.Mock.Services;
using AdmIn.Mock.Services.UtilityServices;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRadzenComponents();
builder.Services.AddHttpClient();

//Mis Servicios Utiles
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<PageTitleService>();
builder.Services.AddScoped<DeviceService>();
builder.Services.AddScoped<LogHelper>();

builder.Services.AddScoped<IServ_Mock, MockData>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>();

app.Run();
