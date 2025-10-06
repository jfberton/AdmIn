using AdmIn.Common;
using AdmIn.Common.Entidades;
using AdmIn.UI.Services.UtilityServices;
using Microsoft.AspNetCore.Components.Authorization;

namespace AdmIn.UI.Services
{
    public class Serv_DetalleTrabajo : ServicioBase<DetalleTrabajo>, IServ_DetalleTrabajo
    {
        public Serv_DetalleTrabajo(
            IHttpClientFactory httpClientFactory,
            IConfiguration config,
            IHostEnvironment env,
            ILogger<DetalleTrabajo> logger,
            AuthenticationStateProvider auth,
            ITokenService tokenService
        ) : base(httpClientFactory, config, env, logger, auth, tokenService, "DetalleTrabajo")
        {
        }

        // Controller uses 'crear' endpoint instead of ServicioBase default 'nuevo'
        public new async Task<DTO<DetalleTrabajo>> Crear(DetalleTrabajo detalle)
        {
            return await EjecutarPeticion<DTO<DetalleTrabajo>>(HttpMethod.Post, "crear", detalle);
        }

        // Controller uses 'actualizar' endpoint instead of ServicioBase default 'modificar'
        public new async Task<DTO<DetalleTrabajo>> Actualizar(DetalleTrabajo detalle)
        {
            return await EjecutarPeticion<DTO<DetalleTrabajo>>(HttpMethod.Put, "actualizar", detalle);
        }

        public async Task<DTO<IEnumerable<DetalleTrabajo>>> Obtener_por_trabajo(int trabajoId)
            => await EjecutarPeticion<DTO<IEnumerable<DetalleTrabajo>>>(HttpMethod.Get, $"obtener_por_trabajo/{trabajoId}");
    }
}
