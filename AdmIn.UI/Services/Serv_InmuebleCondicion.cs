using AdmIn.Common;
using AdmIn.Common.Entidades;
using AdmIn.UI.Services.UtilityServices;
using Microsoft.AspNetCore.Components.Authorization;

namespace AdmIn.UI.Services
{
    public class Serv_InmuebleCondicion : ServicioBase<InmuebleCondicion>, IServ_InmuebleCondicion
    {
        public Serv_InmuebleCondicion(
            IHttpClientFactory httpClientFactory,
            IConfiguration config,
            IHostEnvironment env,
            ILogger<InmuebleCondicion> logger,
            AuthenticationStateProvider auth,
            ITokenService tokenService
        ) : base(httpClientFactory, config, env, logger, auth, tokenService, "InmuebleCondicion")
        {
        }

        public async Task<DTO<InmuebleCondicion>> Crear(InmuebleCondicion condicion)
        {
            return await EjecutarPeticion<DTO<InmuebleCondicion>>(HttpMethod.Post, "crear", condicion) ??
                   new DTO<InmuebleCondicion> { Correcto = false, Mensaje = "Error al crear condición" };
        }

        public async Task<DTO<InmuebleCondicion>> Actualizar(InmuebleCondicion condicion)
        {
            return await EjecutarPeticion<DTO<InmuebleCondicion>>(HttpMethod.Put, "actualizar", condicion) ??
                   new DTO<InmuebleCondicion> { Correcto = false, Mensaje = "Error al actualizar condición" };
        }

        public async Task<DTO<bool>> Eliminar(int condicionId)
        {
            return await EjecutarPeticion<DTO<bool>>(HttpMethod.Delete, $"eliminar/{condicionId}") ??
                   new DTO<bool> { Correcto = false, Mensaje = "Error al eliminar condición" };
        }

        public async Task<DTO<InmuebleCondicion>> Obtener_por_id(int condicionId)
        {
            return await EjecutarPeticion<DTO<InmuebleCondicion>>(HttpMethod.Get, $"obtener_por_id/{condicionId}") ??
                   new DTO<InmuebleCondicion> { Correcto = false, Mensaje = "Error al obtener condición" };
        }

        public async Task<DTO<IEnumerable<InmuebleCondicion>>> Obtener_todos()
        {
            return await EjecutarPeticion<DTO<IEnumerable<InmuebleCondicion>>>(HttpMethod.Get, "obtener_todos") ??
                   new DTO<IEnumerable<InmuebleCondicion>> { Correcto = false, Mensaje = "Error al obtener condiciones" };
        }

        public async Task<DTO<Items_pagina<InmuebleCondicion>>> Obtener_paginado(Filtros_paginado filtros)
        {
            return await EjecutarPeticion<DTO<Items_pagina<InmuebleCondicion>>>(HttpMethod.Post, "obtener_paginado", filtros) ??
                   new DTO<Items_pagina<InmuebleCondicion>> { Correcto = false, Mensaje = "Error al obtener condiciones paginadas" };
        }

        public async Task<DTO<IEnumerable<InmuebleCondicion>>> Obtener_activos()
        {
            return await EjecutarPeticion<DTO<IEnumerable<InmuebleCondicion>>>(HttpMethod.Get, "obtener_activos") ??
                   new DTO<IEnumerable<InmuebleCondicion>> { Correcto = false, Mensaje = "Error al obtener condiciones activas" };
        }

        public async Task<DTO<InmuebleCondicion>> Obtener_por_nombre(string nombre)
        {
            return await EjecutarPeticion<DTO<InmuebleCondicion>>(HttpMethod.Get, $"obtener_por_nombre/{Uri.EscapeDataString(nombre)}") ??
                   new DTO<InmuebleCondicion> { Correcto = false, Mensaje = "Error al obtener condición por nombre" };
        }

        public async Task<DTO<InmuebleCondicion>> Obtener_por_defecto()
        {
            return await EjecutarPeticion<DTO<InmuebleCondicion>>(HttpMethod.Get, "obtener_por_defecto") ??
                   new DTO<InmuebleCondicion> { Correcto = false, Mensaje = "Error al obtener condición por defecto" };
        }
    }
}