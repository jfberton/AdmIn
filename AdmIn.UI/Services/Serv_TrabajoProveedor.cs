using AdmIn.Common;
using AdmIn.Common.Entidades;
using AdmIn.UI.Services.UtilityServices;
using Microsoft.AspNetCore.Components.Authorization;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System;

namespace AdmIn.UI.Services
{
    public class Serv_TrabajoProveedor : ServicioBase<TrabajoProveedor>, IServ_TrabajoProveedor
    {
        public Serv_TrabajoProveedor(
            IHttpClientFactory httpClientFactory,
            IConfiguration config,
            IHostEnvironment env,
            ILogger<TrabajoProveedor> logger,
            AuthenticationStateProvider auth,
            ITokenService tokenService
        ) : base(httpClientFactory, config, env, logger, auth, tokenService, "TrabajoProveedor")
        {
        }

        public async Task<DTO<Items_pagina<TrabajoProveedor>>> Obtener_paginado_filtrado(Filtros_paginado filtros)
        {
            return await EjecutarPeticion<DTO<Items_pagina<TrabajoProveedor>>>(HttpMethod.Post, "obtener_paginado", filtros);
        }

        public async Task<DTO<bool>> AceptarTrabajo(AceptarTrabajoRequest request)
        {
            return await EjecutarPeticion<DTO<bool>>(HttpMethod.Post, "aceptar", request);
        }
    }
}
