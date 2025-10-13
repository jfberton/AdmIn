using AdmIn.Common;
using AdmIn.Common.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdmIn.UI.Services
{
    public interface IServ_TrabajoProveedor : IServicioBase<TrabajoProveedor>
    {
        Task<DTO<Items_pagina<TrabajoProveedor>>> Obtener_paginado_filtrado(Filtros_paginado filtros);
        Task<DTO<bool>> AceptarTrabajo(AceptarTrabajoRequest request);

        // Add proxy methods for new actions implemented in business layer
        Task<DTO<bool>> SolicitarTrabajo(TrabajoAccionRequest request);
        Task<DTO<bool>> RechazarTrabajo(TrabajoAccionRequest request);
        Task<DTO<bool>> MarcarFinalizado(TrabajoAccionRequest request);
        Task<DTO<bool>> RevisarFinalizacion(RevisarFinalizacionRequest request);
        Task<DTO<bool>> CancelarTrabajo(TrabajoAccionRequest request);

        // Document endpoints
        Task<DTO<TrabajoProveedorDocumento>> CrearDocumento(TrabajoProveedorDocumento doc);
        Task<DTO<TrabajoProveedorDocumento>> ObtenerDocumentoPorId(int documentoId);
        Task<DTO<IEnumerable<TrabajoProveedorDocumento>>> ObtenerDocumentosPorTrabajo(int trabajoId);
        Task<DTO<bool>> EliminarDocumento(int documentoId);
    }
}
