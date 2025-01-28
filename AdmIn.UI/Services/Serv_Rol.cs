using AdmIn.Business.Entidades;
using AdmIn.Common;
using AdmIn.UI.Services.UtilityServices;
using Microsoft.AspNetCore.Components.Authorization;

namespace AdmIn.UI.Services
{
    public class Serv_Rol : ServicioBase<Rol>, IServ_Rol
    {
        public Serv_Rol(
            IHttpClientFactory httpClientFactory,
            IConfiguration config,
            IHostEnvironment env,
            ILogger<Rol> logger,
            AuthenticationStateProvider auth,
            ITokenService tokenService
        ) : base(httpClientFactory, config, env, logger, auth, tokenService, "Rol")
        {
        }

        public async Task<DTO<IEnumerable<Rol>>> Obtener_por_usuario(int usuarioId)
        {
            return await EjecutarPeticion<DTO<IEnumerable<Rol>>>(HttpMethod.Get, $"obtener_por_usuario/{usuarioId}");
        }

        public async Task<DTO<IEnumerable<Rol>>> Obtener_todos()
        {
            return await EjecutarPeticion<DTO<IEnumerable<Rol>>>(HttpMethod.Get, $"obtener_todos");
        }
    }

}
