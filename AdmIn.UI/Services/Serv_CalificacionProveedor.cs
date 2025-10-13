using AdmIn.Common;
using AdmIn.Common.Entidades;
using AdmIn.UI.Services.UtilityServices;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Components.Authorization;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;

namespace AdmIn.UI.Services
{
    public class Serv_CalificacionProveedor : ServicioBase<CalificacionProveedor>, IServ_CalificacionProveedor
    {
        public Serv_CalificacionProveedor(
            IHttpClientFactory httpClientFactory,
            IConfiguration config,
            IHostEnvironment env,
            ILogger<CalificacionProveedor> logger,
            AuthenticationStateProvider auth,
            ITokenService tokenService
        ) : base(httpClientFactory, config, env, logger, auth, tokenService, "CalificacionProveedor")
        {
        }

        public async Task<DTO<CalificacionProveedor>> Crear(CalificacionProveedor calificacionProveedor)
        {
            return await EjecutarPeticion<DTO<CalificacionProveedor>>(HttpMethod.Post, "crear", calificacionProveedor);
        }

        public async Task<DTO<CalificacionProveedor>> Actualizar(CalificacionProveedor calificacionProveedor)
        {
            return await EjecutarPeticion<DTO<CalificacionProveedor>>(HttpMethod.Put, "actualizar", calificacionProveedor);
        }

        public async Task<DTO<bool>> Eliminar(int calificacionProveedorId)
        {
            return await EjecutarPeticion<DTO<bool>>(HttpMethod.Delete, $"eliminar/{calificacionProveedorId}");
        }

        public async Task<DTO<CalificacionProveedor>> Obtener_por_id(int calificacionProveedorId)
        {
            return await EjecutarPeticion<DTO<CalificacionProveedor>>(HttpMethod.Get, $"obtener_por_id/{calificacionProveedorId}");
        }

        public async Task<DTO<IEnumerable<CalificacionProveedor>>> Obtener_todos()
        {
            return await EjecutarPeticion<DTO<IEnumerable<CalificacionProveedor>>>(HttpMethod.Get, "obtener_todos");
        }

        public async Task<DTO<Items_pagina<CalificacionProveedor>>> Obtener_paginado(Filtros_paginado filtros)
        {
            return await EjecutarPeticion<DTO<Items_pagina<CalificacionProveedor>>>(HttpMethod.Post, "obtener_paginado", filtros);
        }
    }
}
