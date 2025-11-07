using AdmIn.Business.Servicios;
using AdmIn.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AdmIn.API.Services
{
    public class ReservaExpirationService : BackgroundService
    {
        private readonly ILogger<ReservaExpirationService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _periodo;

        public ReservaExpirationService(ILogger<ReservaExpirationService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            // Default: ejecutar cada1 hora
            _periodo = TimeSpan.FromHours(1);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ReservaExpirationService iniciado. Intervalo: {period}.", _periodo);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Ejecutando expiración de reservas: {time}", DateTime.UtcNow);

                    // Crear alcance para resolver servicios scoped
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var servReserva = scope.ServiceProvider.GetService<IServ_Reserva>();

                        if (servReserva == null)
                        {
                            _logger.LogWarning("IServ_Reserva no está registrado en el contenedor de DI.");
                        }
                        else
                        {
                            var resultado = await servReserva.ExpirarReservas();

                            if (!resultado.Correcto)
                            {
                                _logger.LogWarning("ExpirarReservas devolvió error: {msg}", resultado.Mensaje);
                           
                            }
                            else
                            {
                                _logger.LogInformation("ExpirarReservas completado correctamente.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error ejecutando ExpirarReservas");
                }

                try
                {
                    await Task.Delay(_periodo, stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    // Stopping
                }
            }

            _logger.LogInformation("ReservaExpirationService detenido.");
        }
    }
}
