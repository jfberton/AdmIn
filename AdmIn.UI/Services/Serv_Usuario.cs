using AdmIn.Business.Entidades;
using AdmIn.Business.Utilidades;
using AdmIn.Common;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;

namespace AdmIn.UI.Services
{
    public class Serv_Usuario : IServ_Usuario
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AuthenticationStateProvider _auth;
        private readonly IHostEnvironment _env;
        private readonly IConfiguration _config;
        private string path_api;
        private ILogger<Usuario> _logger;
        private string token = string.Empty;
        private Usuario usuarioLogueado;

        public Serv_Usuario(IHttpClientFactory httpClientFactory, IHostEnvironment env, IConfiguration config, ILogger<Usuario> logger, AuthenticationStateProvider auth)
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

        public async Task<DTO<Usuario>> Actualizar_usuario(Usuario usuario)
        {
            //no se actualiza el pasword desde aca
            usuario.Password = string.Empty;

            var clienteHttp = _httpClientFactory.CreateClient();
            token = await ObtenerToken();
            clienteHttp.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await clienteHttp.PostAsJsonAsync(path_api + "Usuario/modificar", usuario);

            if (response.IsSuccessStatusCode)
            {
                var dto = await response.Content.ReadFromJsonAsync<DTO<Usuario>>();
                return dto;
            }
            else
            {
                return new DTO<Usuario>
                {
                    Correcto = false,
                    Mensaje = $"Error en la conexion para modificar el usuario: {response.StatusCode.ToString()}"
                };
            }
        }

        public async Task<DTO<Usuario>> Crear_usuario(Usuario usuario)
        {
            var clienteHttp = _httpClientFactory.CreateClient();
            token = await ObtenerToken();
            clienteHttp.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await clienteHttp.PostAsJsonAsync(path_api + "Usuario/nuevo", usuario);

            if (response.IsSuccessStatusCode)
            {
                var dto = await response.Content.ReadFromJsonAsync<DTO<Usuario>>();
                return dto;
            }
            else
            {
                return new DTO<Usuario>
                {
                    Correcto = false,
                    Mensaje = $"Error en la conexion para crear el usuario: {response.StatusCode.ToString()}"
                };
            }
        }

        public async Task<DTO<bool>> Modificar_password(CambioClaveModel datos)
        {
            var clienteHttp = _httpClientFactory.CreateClient();
            token = await ObtenerToken();
            clienteHttp.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await clienteHttp.PostAsJsonAsync(path_api + "Usuario/modificar_password", datos);

            if (response.IsSuccessStatusCode)
            {
                var dto = await response.Content.ReadFromJsonAsync<DTO<bool>>();
                return dto;
            }
            else
            {
                return new DTO<bool>
                {
                    Correcto = false,
                    Mensaje = $"Error en la conexion para modificar la clave: {response.StatusCode.ToString()}"
                };
            }
        }

        public async Task<DTO<bool>> Eliminar_usuario(int usuarioId)
        {
            var clienteHttp = _httpClientFactory.CreateClient();
            token = await ObtenerToken();
            clienteHttp.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var respuesta = await clienteHttp.DeleteAsync($"{path_api}Usuario/eliminar/{usuarioId}");

            if (respuesta.IsSuccessStatusCode)
            {
                var dto = await respuesta.Content.ReadFromJsonAsync<DTO<bool>>();
                return dto;
            }
            else
            {
                return new DTO<bool>
                {
                    Correcto = false,
                    Mensaje = $"Error en la conexion para eliminar el usuario: {respuesta.StatusCode.ToString()}"
                };
            }
        }

        public async Task<DTO<Items_pagina<Usuario>>> Obtener_usuarios(Filtros_paginado filtros)
        {
            var clienteHttp = _httpClientFactory.CreateClient();
            token = await ObtenerToken();
            clienteHttp.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await clienteHttp.PostAsJsonAsync(path_api + "Usuario/obtener_paginado", filtros);

            if (response.IsSuccessStatusCode)
            {
                var dto = await response.Content.ReadFromJsonAsync<DTO<Items_pagina<Usuario>>>();
                return dto;
            }
            else
            {
                return new DTO<Items_pagina<Usuario>>
                {
                    Correcto = false,
                    Mensaje = $"Error en la conexion para obtener los usuarios: {response.StatusCode.ToString()}"
                };
            }

        }

        public async Task<DTO<Usuario>> Obtener_usuario_por_email(string email)
        {
            var clienteHttp = _httpClientFactory.CreateClient();
            token = await ObtenerToken();
            clienteHttp.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await clienteHttp.GetAsync($"{path_api}Usuario/obtener_por_mail/{email}");

            if (response.IsSuccessStatusCode)
            {
                var dto = await response.Content.ReadFromJsonAsync<DTO<Usuario>>();
                return dto;
            }
            else
            {
                return null;
            }
        }

        //a veces al ingresar a la pagina si quiero consultar por aqui el usuario logueado retorna null,
        //por lo que paso el token logueado por parametro
        public async Task<DTO<Usuario>> Obtener_usuario_por_email(string email, string token)
        {
            var clienteHttp = _httpClientFactory.CreateClient();
            clienteHttp.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await clienteHttp.GetAsync($"{path_api}Usuario/obtener_por_mail/{email}");

            if (response.IsSuccessStatusCode)
            {
                var dto = await response.Content.ReadFromJsonAsync<DTO<Usuario>>();
                return dto;
            }
            else
            {
                return null;
            }
        }

        private async Task<string> ObtenerToken()
        {
            string ret = string.Empty;
            var authState = await _auth.GetAuthenticationStateAsync();
            var user = authState.User;
            if (user.Identity.IsAuthenticated)
            {
                var tokenClaim = user.FindFirst("Token");
                if (tokenClaim != null)
                {
                    ret = tokenClaim.Value;
                }
            }

            return ret;
        }
    }   
}
