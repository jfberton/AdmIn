using AdmIn.Business.Entidades;
using AdmIn.Common;
using AdmIn.UI.Services.UtilityServices;
using Microsoft.AspNetCore.Components.Authorization;

namespace AdmIn.UI.Services
{
    public class Serv_Permiso : ServicioBase<Permiso>, IServ_Permiso
    {
        public Serv_Permiso(
            IHttpClientFactory httpClientFactory,
            IConfiguration config,
            IHostEnvironment env,
            ILogger<Permiso> logger,
            AuthenticationStateProvider auth,
            ITokenService tokenService
        ) : base(httpClientFactory, config, env, logger, auth, tokenService, "Permiso")
        {
        }

        public async Task<DTO<IEnumerable<Permiso>>> Obtener_por_rol(int rolId)
        {
            return await EjecutarPeticion<DTO<IEnumerable<Permiso>>>(HttpMethod.Get, $"obtener_por_rol/{rolId}");
        }
    }

}
