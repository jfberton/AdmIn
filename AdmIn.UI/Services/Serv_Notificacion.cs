using AdmIn.Common;
using AdmIn.Common.Entidades;
using AdmIn.UI.Services.UtilityServices;
using Microsoft.AspNetCore.Components.Authorization;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace AdmIn.UI.Services
{
    public class Serv_Notificacion : ServicioBase<Notificacion>, IServ_Notificacion
    {
        public Serv_Notificacion(
            IHttpClientFactory httpClientFactory,
            IConfiguration config,
            IHostEnvironment env,
            ILogger<Notificacion> logger,
            AuthenticationStateProvider auth,
            ITokenService tokenService
        ) : base(httpClientFactory, config, env, logger, auth, tokenService, "Notificacion")
        {
        }

        public async Task<DTO<int>> Contar_no_leidas(int usuarioId)
            => await EjecutarPeticion<DTO<int>>(HttpMethod.Get, $"contar_no_leidas/{usuarioId}");

        public async Task<DTO<IEnumerable<Notificacion>>> Obtener_por_usuario(int usuarioId)
            => await EjecutarPeticion<DTO<IEnumerable<Notificacion>>>(HttpMethod.Get, $"obtener_por_usuario/{usuarioId}");

        public async Task<DTO<Notificacion>> Crear(Notificacion notificacion)
            => await EjecutarPeticion<DTO<Notificacion>>(HttpMethod.Post, "crear", notificacion);

        public async Task<DTO<bool>> MarcarComoLeida(int notificacionId)
            => await EjecutarPeticion<DTO<bool>>(HttpMethod.Post, $"marcar_leida/{notificacionId}");
    }
}
