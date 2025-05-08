using AdmIn.UI.Services.UtilityServices;
using Microsoft.AspNetCore.Components;
using Radzen;
using System.Threading.Tasks;

public abstract class BaseComponent : ComponentBase
{
    [Inject]
    public required ITokenService TokenService { get; set; }

    [Inject]
    public required NavigationManager NavigationManager { get; set; }

    [Inject]
    public required NotificationService NotificationService { get; set; }

    [Inject]
    public required PageTitleService PageTitleService { get; set; }

    [Inject]
    public required DeviceService Device_Service { get; set; }

    [Inject]
    public required LogHelper LogHelper { get; set; } // Inyección de LogHelper

    protected override async Task OnInitializedAsync()
    {
        await LogHelper.Debug("Inicializando componente...");
        var isTokenExpired = await TokenService.IsTokenExpiredAsync();
        if (isTokenExpired)
        {
            MostrarNotificacionYRedirigir();
        }
    }

    private void MostrarNotificacionYRedirigir()
    {
        var message = new NotificationMessage
        {
            Severity = NotificationSeverity.Warning,
            Summary = "Sesión expirada",
            Detail = "Tu sesión ha expirado. Serás redirigido al login.",
            Duration = 4000
        };

        NotificationService.Notify(message);

        Task.Delay(4000).ContinueWith(async _ =>
        {
            await LogHelper.Warn("Redirigiendo al login debido a sesión expirada.");
            NavigationManager.NavigateTo("/", true);
        });
    }

    public void MostrarNotificacion(NotificationSeverity tipo, string titulo, string detalle, string? redirectUrl = null)
    {
        var message = new NotificationMessage
        {
            Severity = tipo,
            Summary = titulo,
            Detail = detalle,
            Duration = 4000
        };

        NotificationService.Notify(message);

        if (!string.IsNullOrWhiteSpace(redirectUrl))
        {
            Task.Delay(4000).ContinueWith(async _ =>
            {
                await LogHelper.Info($"Redirigiendo a {redirectUrl} después de mostrar notificación.");
                NavigationManager.NavigateTo(redirectUrl, true);
            });
        }
    }

    protected void EstablecerTituloPagina(string titulo, string? subtitulo = null)
    {
        PageTitleService.SetTitle(titulo, subtitulo);
        LogHelper.Info($"Título de página establecido: {titulo}, Subtítulo: {subtitulo ?? "N/A"}").ConfigureAwait(false);
    }

    public void IrA(string path)
    {
        if (!string.IsNullOrWhiteSpace(path))
        {
            LogHelper.Info($"Navegando a {path}").ConfigureAwait(false);
            NavigationManager.NavigateTo(path);
        }
        else
        {
            LogHelper.Warn("Intento de navegación fallido: la ruta no puede estar vacía.").ConfigureAwait(false);
            MostrarNotificacion(NotificationSeverity.Warning, "Error de navegación", "La ruta no puede estar vacía.");
        }
    }

    public bool EsDispositivoMovil()
    {
        var esMovil = Device_Service.IsMobileDevice();
        return esMovil;
    }
}
