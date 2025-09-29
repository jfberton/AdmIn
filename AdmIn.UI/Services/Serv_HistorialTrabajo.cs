using AdmIn.Common;
using AdmIn.Common.Entidades;
using AdmIn.UI.Services.UtilityServices;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdmIn.UI.Services
{
    public class Serv_HistorialTrabajo : ServicioBase<HistorialTrabajo>, IServ_HistorialTrabajo
    {
        public Serv_HistorialTrabajo(IHttpClientFactory httpClientFactory, IConfiguration config, IHostEnvironment env, ILogger<HistorialTrabajo> logger, AuthenticationStateProvider auth, ITokenService tokenService)
            : base(httpClientFactory, config, env, logger, auth, tokenService, "HistorialTrabajo")
        {
        }

        public async Task<DTO<HistorialTrabajo>> Crear(HistorialTrabajo historialTrabajo)
            => await EjecutarPeticion<DTO<HistorialTrabajo>>(HttpMethod.Post, "crear", historialTrabajo);

        public async Task<DTO<HistorialTrabajo>> Actualizar(HistorialTrabajo historialTrabajo)
            => await EjecutarPeticion<DTO<HistorialTrabajo>>(HttpMethod.Put, "actualizar", historialTrabajo);

        public async Task<DTO<bool>> Eliminar(HistorialTrabajo historialTrabajo)
            => await EjecutarPeticion<DTO<bool>>(HttpMethod.Delete, $"eliminar/{historialTrabajo.HistorialTrabajoId}");

        public async Task<DTO<HistorialTrabajo>> Obtener_por_id(int historialTrabajoId)
            => await EjecutarPeticion<DTO<HistorialTrabajo>>(HttpMethod.Get, $"obtener_por_id/{historialTrabajoId}");

        public async Task<DTO<IEnumerable<HistorialTrabajo>>> Obtener_todos()
            => await EjecutarPeticion<DTO<IEnumerable<HistorialTrabajo>>>(HttpMethod.Get, "obtener_todos");

        public async Task<DTO<Items_pagina<HistorialTrabajo>>> Obtener_paginado(Filtros_paginado filtros)
            => await EjecutarPeticion<DTO<Items_pagina<HistorialTrabajo>>>(HttpMethod.Post, "obtener_paginado", filtros);

        public async Task<DTO<IEnumerable<HistorialTrabajo>>> Obtener_por_trabajo(int trabajoId)
            => await EjecutarPeticion<DTO<IEnumerable<HistorialTrabajo>>>(HttpMethod.Get, $"obtener_por_trabajo/{trabajoId}");
    }
}