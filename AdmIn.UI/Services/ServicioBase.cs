using AdmIn.Common;
using AdmIn.UI.Services.UtilityServices;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;

namespace AdmIn.UI.Services
{
    public class ServicioBase<T> : IServicioBase<T> where T : class
    {
        protected readonly IHttpClientFactory _httpClientFactory;
        protected readonly string _pathApi;
        private readonly AuthenticationStateProvider _auth;
        private readonly ILogger<T> _logger;
        private readonly ITokenService _tokenService;

        private readonly Dictionary<int, T> _diccionario = new();

        public ServicioBase(
            IHttpClientFactory httpClientFactory,
            IConfiguration config,
            IHostEnvironment env,
            ILogger<T> logger,
            AuthenticationStateProvider auth,
            ITokenService tokenService,
            string recurso
        )
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _auth = auth;
            _tokenService = tokenService;

            _pathApi = env.IsDevelopment()
                ? $"{config["Path_api_dev"]}{recurso}/"
                : $"{config["Path_api_prod"]}{recurso}/";
        }

        public async Task<DTO<T>> Crear(T entidad)
        {
            return await EjecutarPeticion<DTO<T>>(HttpMethod.Post, "nuevo", entidad);
        }

        public async Task<DTO<T>> Actualizar(T entidad)
        {
            return await EjecutarPeticion<DTO<T>>(HttpMethod.Post, "modificar", entidad);
        }

        public async Task<DTO<bool>> Eliminar(int id)
        {
            return await EjecutarPeticion<DTO<bool>>(HttpMethod.Delete, $"eliminar/{id}");
        }

        public async Task<DTO<T>> Obtener_por_id(int id)
        {
            return await EjecutarPeticion<DTO<T>>(HttpMethod.Get, $"obtener_por_id/{id}");
        }

        public async Task<DTO<Items_pagina<T>>> Obtener_paginado(Filtros_paginado filtros)
        {
            return await EjecutarPeticion<DTO<Items_pagina<T>>>(HttpMethod.Post, "obtener_paginado", filtros);
        }

        protected async Task<TResult> EjecutarPeticion<TResult>(HttpMethod metodo, string endpoint, object contenido = null)
        {
            var clienteHttp = _httpClientFactory.CreateClient();
            var token = await ObtenerToken();
            clienteHttp.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpRequestMessage request = new(metodo, _pathApi + endpoint);
            if (contenido != null)
            {
                request.Content = JsonContent.Create(contenido);
            }

            var response = await clienteHttp.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<TResult>();
            }

            _logger.LogError($"Error al realizar la petición: {response.StatusCode}");
            return default;
        }

        private async Task<string> ObtenerToken()
        {
            var token = await _tokenService.GetTokenAsync();

            if (string.IsNullOrEmpty(token))
            {
                return string.Empty;
            }

            return token;
        }

        #region Diccionario Local

        public void Diccionario_local_agregar(int id, T objeto)
        {
            _diccionario[id] = objeto;
        }

        public void Diccionario_local_eliminar(int id)
        {
            _diccionario.Remove(id);
        }

        public T Diccionario_local_obtener(int id)
        {
            return _diccionario.TryGetValue(id, out T objeto) ? objeto : null;
        }

        #endregion
    }

}
