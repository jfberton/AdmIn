using AdmIn.Common;
using AdmIn.Common.Entidades;
using AdmIn.UI.Services.UtilityServices;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdmIn.UI.Services
{
    public class Serv_HistorialDetalleTrabajo : ServicioBase<HistorialDetalleTrabajo>, IServ_HistorialDetalleTrabajo
    {
        public Serv_HistorialDetalleTrabajo(IHttpClientFactory httpClientFactory, IConfiguration config, IHostEnvironment env, ILogger<HistorialDetalleTrabajo> logger, AuthenticationStateProvider auth, ITokenService tokenService)
            : base(httpClientFactory, config, env, logger, auth, tokenService, "HistorialDetalleTrabajo")
        {
        }

        public async Task<DTO<HistorialDetalleTrabajo>> Crear(HistorialDetalleTrabajo detalle)
            => await EjecutarPeticion<DTO<HistorialDetalleTrabajo>>(HttpMethod.Post, "crear", detalle);

        public async Task<DTO<HistorialDetalleTrabajo>> Actualizar(HistorialDetalleTrabajo detalle)
            => await EjecutarPeticion<DTO<HistorialDetalleTrabajo>>(HttpMethod.Put, "actualizar", detalle);

        public async Task<DTO<bool>> Eliminar(HistorialDetalleTrabajo detalle)
            => await EjecutarPeticion<DTO<bool>>(HttpMethod.Delete, $"eliminar/{detalle.HistorialDetalleTrabajoId}");

        public async Task<DTO<HistorialDetalleTrabajo>> Obtener_por_id(int id)
            => await EjecutarPeticion<DTO<HistorialDetalleTrabajo>>(HttpMethod.Get, $"obtener_por_id/{id}");

        public async Task<DTO<IEnumerable<HistorialDetalleTrabajo>>> Obtener_todos()
            => await EjecutarPeticion<DTO<IEnumerable<HistorialDetalleTrabajo>>>(HttpMethod.Get, "obtener_todos");

        public async Task<DTO<Items_pagina<HistorialDetalleTrabajo>>> Obtener_paginado(Filtros_paginado filtros)
            => await EjecutarPeticion<DTO<Items_pagina<HistorialDetalleTrabajo>>>(HttpMethod.Post, "obtener_paginado", filtros);
    }
}