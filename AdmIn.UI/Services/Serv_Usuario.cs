using AdmIn.Business.Entidades;
using AdmIn.Business.Utilidades;
using AdmIn.Common;
using AdmIn.UI.Services.UtilityServices;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http;
using System.Net.Http.Headers;

namespace AdmIn.UI.Services
{
    public class Serv_Usuario : ServicioBase<Usuario>, IServ_Usuario
    {

        public Serv_Usuario(
            IHttpClientFactory httpClientFactory,
            IConfiguration config,
            IHostEnvironment env,
            ILogger<Usuario> logger,
            AuthenticationStateProvider auth,
            ITokenService tokenService
        ) : base(httpClientFactory, config, env, logger, auth, tokenService, "Usuario")
        {
        }

        public async Task<DTO<Usuario>> Obtener_usuario_por_email(string email)
        {
            return await EjecutarPeticion<DTO<Usuario>>(HttpMethod.Get, $"obtener_por_mail/{email}");
        }

        public async Task<DTO<bool>> Modificar_password(CambioClaveModel datos)
        {
            return await EjecutarPeticion<DTO<bool>>(HttpMethod.Post, "modificar_password", datos);
        }
    }


}
