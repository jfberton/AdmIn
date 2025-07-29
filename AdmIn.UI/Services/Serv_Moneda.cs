using AdmIn.Common.Entidades;
using AdmIn.Common;
using AdmIn.UI.Services.UtilityServices;
using Microsoft.AspNetCore.Components.Authorization;

namespace AdmIn.UI.Services
{
    public class Serv_Moneda : ServicioBase<Moneda>, IServ_Moneda
    {
        public Serv_Moneda(
            IHttpClientFactory httpClientFactory,
            IConfiguration config,
            IHostEnvironment env,
            ILogger<Moneda> logger,
            AuthenticationStateProvider auth,
            ITokenService tokenService
        ) : base(httpClientFactory, config, env, logger, auth, tokenService, "Moneda")
        {
        }
    }
}