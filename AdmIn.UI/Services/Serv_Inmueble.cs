using AdmIn.Common.Entidades;
using AdmIn.Common;
using AdmIn.UI.Services.UtilityServices;
using Microsoft.AspNetCore.Components.Authorization;

namespace AdmIn.UI.Services
{
    public class Serv_Inmueble : ServicioBase<Inmueble>, IServ_Inmueble
    {
        public Serv_Inmueble(
            IHttpClientFactory httpClientFactory,
            IConfiguration config,
            IHostEnvironment env,
            ILogger<Inmueble> logger,
            AuthenticationStateProvider auth,
            ITokenService tokenService
        ) : base(httpClientFactory, config, env, logger, auth, tokenService, "Inmueble")
        {
        }

        public async Task<IEnumerable<string>> ObtenerEstadosInmueble()
        {
            var response = await EjecutarPeticion<DTO<IEnumerable<string>>>(HttpMethod.Get, "obtener_estados");
            return response?.Datos ?? new List<string>();
        }

        public async Task<string?> ObtenerEstadoInmueblePorId(int id)
        {
            var response = await EjecutarPeticion<DTO<Inmueble>>(HttpMethod.Get, $"obtener_por_id/{id}");
            return response?.Datos?.Estado;
        }

        public async Task AgregarCaracteristica(int inmuebleId, CaracteristicaInmueble caracteristica)
        {
            await EjecutarPeticion<DTO<CaracteristicaInmueble>>(HttpMethod.Post, $"{inmuebleId}/caracteristicas", caracteristica);
        }

        public async Task ActualizarCaracteristica(int inmuebleId, CaracteristicaInmueble caracteristica)
        {
            await EjecutarPeticion<DTO<CaracteristicaInmueble>>(HttpMethod.Put, $"caracteristicas/{caracteristica.Id}", caracteristica);
        }

        public async Task EliminarCaracteristica(int inmuebleId, int caracteristicaId)
        {
            await EjecutarPeticion<DTO<bool>>(HttpMethod.Delete, $"caracteristicas/{caracteristicaId}");
        }

        // Método adicional para obtener características
        public async Task<DTO<IEnumerable<CaracteristicaInmueble>>> ObtenerCaracteristicas(int inmuebleId)
        {
            return await EjecutarPeticion<DTO<IEnumerable<CaracteristicaInmueble>>>(HttpMethod.Get, $"{inmuebleId}/caracteristicas");
        }

        public async Task AgregarImagen(int inmuebleId, Imagen imagen)
        {
            // Implementación pendiente - requiere endpoint específico
            await Task.CompletedTask;
        }

        public async Task EliminarImagen(int inmuebleId, Guid imagenId)
        {
            // Implementación pendiente - requiere endpoint específico
            await Task.CompletedTask;
        }

        public async Task EstablecerImagenPrincipal(int inmuebleId, Guid imagenId)
        {
            // Implementación pendiente - requiere endpoint específico
            await Task.CompletedTask;
        }

        // Métodos adicionales específicos
        public async Task<DTO<IEnumerable<Inmueble>>> Obtener_por_estado(string estado)
        {
            return await EjecutarPeticion<DTO<IEnumerable<Inmueble>>>(HttpMethod.Get, $"obtener_por_estado/{estado}");
        }

        public async Task<DTO<IEnumerable<Inmueble>>> Obtener_por_administrador(int administradorId)
        {
            return await EjecutarPeticion<DTO<IEnumerable<Inmueble>>>(HttpMethod.Get, $"obtener_por_administrador/{administradorId}");
        }

        public async Task<DTO<IEnumerable<Inmueble>>> Obtener_por_ubicacion(string? pais, string? estado, string? ciudad)
        {
            var query = $"obtener_por_ubicacion?";
            var parameters = new List<string>();

            if (!string.IsNullOrEmpty(pais))
                parameters.Add($"pais={Uri.EscapeDataString(pais)}");
            if (!string.IsNullOrEmpty(estado))
                parameters.Add($"estado={Uri.EscapeDataString(estado)}");
            if (!string.IsNullOrEmpty(ciudad))
                parameters.Add($"ciudad={Uri.EscapeDataString(ciudad)}");

            if (parameters.Any())
                query += string.Join("&", parameters);

            return await EjecutarPeticion<DTO<IEnumerable<Inmueble>>>(HttpMethod.Get, query);
        }

        public async Task<DTO<IEnumerable<Inmueble>>> Obtener_por_rango_precio(decimal precioMin, decimal precioMax)
        {
            var query = $"obtener_por_rango_precio?precioMin={precioMin}&precioMax={precioMax}";
            return await EjecutarPeticion<DTO<IEnumerable<Inmueble>>>(HttpMethod.Get, query);
        }
    }
}