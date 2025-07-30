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
    public required DialogService DialogService { get; set; }

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

    /// <summary>
    /// Muestra un diálogo de confirmación y retorna la respuesta del usuario
    /// </summary>
    /// <param name="mensaje">El mensaje a mostrar en el diálogo</param>
    /// <param name="titulo">El título del diálogo (opcional)</param>
    /// <param name="textoConfirmar">Texto del botón de confirmación (por defecto "Sí")</param>
    /// <param name="textoCancelar">Texto del botón de cancelación (por defecto "No")</param>
    /// <returns>True si el usuario confirma, False si cancela</returns>
    public async Task<bool> MostrarConfirmacion(string mensaje, string titulo = "Confirmación", string textoConfirmar = "Sí", string textoCancelar = "No")
    {
        try
        {
            await LogHelper.Debug($"Mostrando diálogo de confirmación: {titulo}");
            
            var result = await DialogService.Confirm(
                message: mensaje,
                title: titulo,
                new ConfirmOptions()
                {
                    OkButtonText = textoConfirmar,
                    CancelButtonText = textoCancelar,
                    AutoFocusFirstElement = true
                });

            await LogHelper.Debug($"Resultado del diálogo de confirmación: {result}");
            return result ?? false;
        }
        catch (Exception ex)
        {
            await LogHelper.Error($"Error al mostrar diálogo de confirmación: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Muestra un diálogo de alerta/información
    /// </summary>
    /// <param name="mensaje">El mensaje a mostrar</param>
    /// <param name="titulo">El título del diálogo (opcional)</param>
    /// <param name="textoBoton">Texto del botón (por defecto "Aceptar")</param>
    public async Task MostrarAlerta(string mensaje, string titulo = "Información", string textoBoton = "Aceptar")
    {
        try
        {
            await LogHelper.Debug($"Mostrando diálogo de alerta: {titulo}");
            
            await DialogService.Alert(
                message: mensaje,
                title: titulo,
                new AlertOptions()
                {
                    OkButtonText = textoBoton
                });
        }
        catch (Exception ex)
        {
            await LogHelper.Error($"Error al mostrar diálogo de alerta: {ex.Message}");
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
