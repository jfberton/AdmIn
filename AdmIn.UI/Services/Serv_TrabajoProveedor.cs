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

        // New UI-side proxy methods for business actions
        public async Task<DTO<bool>> SolicitarTrabajo(TrabajoAccionRequest request)
        {
            return await EjecutarPeticion<DTO<bool>>(HttpMethod.Post, "solicitar", request);
        }

        public async Task<DTO<bool>> RechazarTrabajo(TrabajoAccionRequest request)
        {
            return await EjecutarPeticion<DTO<bool>>(HttpMethod.Post, "rechazar", request);
        }

        public async Task<DTO<bool>> MarcarFinalizado(TrabajoAccionRequest request)
        {
            return await EjecutarPeticion<DTO<bool>>(HttpMethod.Post, "marcar_finalizado", request);
        }

        public async Task<DTO<bool>> RevisarFinalizacion(RevisarFinalizacionRequest request)
        {
            return await EjecutarPeticion<DTO<bool>>(HttpMethod.Post, "revisar_finalizacion", request);
        }

        public async Task<DTO<bool>> CancelarTrabajo(TrabajoAccionRequest request)
        {
            return await EjecutarPeticion<DTO<bool>>(HttpMethod.Post, "cancelar", request);
        }

        // Document operations (UI side) - server endpoints added under TrabajoProveedorController
        public async Task<DTO<TrabajoProveedorDocumento>> CrearDocumento(TrabajoProveedorDocumento doc)
        {
            return await EjecutarPeticion<DTO<TrabajoProveedorDocumento>>(HttpMethod.Post, $"{doc.TrabajoProveedorId}/documentos", doc);
        }

        public async Task<DTO<IEnumerable<TrabajoProveedorDocumento>>> ObtenerDocumentosPorTrabajo(int trabajoId)
        {
            return await EjecutarPeticion<DTO<IEnumerable<TrabajoProveedorDocumento>>>(HttpMethod.Get, $"{trabajoId}/documentos");
        }

        public async Task<DTO<TrabajoProveedorDocumento>> ObtenerDocumentoPorId(int documentoId)
        {
            // Use JSON-specific endpoint that returns DTO with Contenido (bytes) instead of binary file endpoint
            return await EjecutarPeticion<DTO<TrabajoProveedorDocumento>>(HttpMethod.Get, $"documentos/json/{documentoId}");
        }

        public async Task<DTO<bool>> EliminarDocumento(int documentoId)
        {
            return await EjecutarPeticion<DTO<bool>>(HttpMethod.Delete, $"documentos/{documentoId}");
        }
    }
}
