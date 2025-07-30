using AdmIn.Common.Entidades;
using AdmIn.Common;
using AdmIn.UI.Services.UtilityServices;
using Microsoft.AspNetCore.Components.Authorization;

namespace AdmIn.UI.Services
{
    public class Serv_Caracteristica : ServicioBase<Caracteristica>, IServ_Caracteristica
    {
        public Serv_Caracteristica(
            IHttpClientFactory httpClientFactory,
            IConfiguration config,
            IHostEnvironment env,
            ILogger<Caracteristica> logger,
            AuthenticationStateProvider auth,
            ITokenService tokenService
        ) : base(httpClientFactory, config, env, logger, auth, tokenService, "Caracteristica")
        {
        }

        public async Task<DTO<IEnumerable<Caracteristica>>> Obtener_por_tipo(string tipo)
        {
            return await EjecutarPeticion<DTO<IEnumerable<Caracteristica>>>(HttpMethod.Get, $"obtener_por_tipo/{tipo}");
        }

        public async Task<DTO<IEnumerable<string>>> Obtener_tipos()
        {
            return await EjecutarPeticion<DTO<IEnumerable<string>>>(HttpMethod.Get, "obtener_tipos");
        }
    }
}