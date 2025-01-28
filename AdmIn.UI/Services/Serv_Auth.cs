using AdmIn.Business.Entidades;
using AdmIn.Business.Utilidades;
using AdmIn.Common;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http;
using System.Net.Http.Headers;

namespace AdmIn.UI.Services
{
    public class Serv_Auth : IServ_Auth
    {

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AuthenticationStateProvider _auth;
        private readonly IHostEnvironment _env;
        private readonly IConfiguration _config;
        private string path_api;
        private ILogger<Usuario> _logger;
        private string token = string.Empty;
        private Usuario usuarioLogueado;

        public Serv_Auth(IHttpClientFactory httpClientFactory, IHostEnvironment env, IConfiguration config, ILogger<Usuario> logger, AuthenticationStateProvider auth)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
            _env = env;
            _logger = logger;
            _auth = auth;

            if (_env.IsDevelopment())
                path_api = _config["Path_api_dev"];
            else
                path_api = _config["Path_api_prod"];

            _auth = auth;
        }

        public async Task<Usuario> Login(LoginModel login)
        {
            var clienteHttp = _httpClientFactory.CreateClient();

            var response = await clienteHttp.PostAsJsonAsync(path_api + "Auth/login", login);

            if (response.IsSuccessStatusCode)
            {
                var dto = await response.Content.ReadFromJsonAsync<DTO<Usuario>>();

                if (dto.Correcto)
                {
                    usuarioLogueado = dto.Datos;
                }

                return dto.Datos;
            }
            else
            {
                return null;
            }
        }
    }
}
