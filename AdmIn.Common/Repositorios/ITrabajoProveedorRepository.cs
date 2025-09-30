using AdmIn.Common;
using AdmIn.Common.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdmIn.Common.Repositorios
{
    public partial interface ITrabajoProveedorRepository
    {
        Task<DTO<TrabajoProveedor>> Crear(TrabajoProveedor trabajo);
        Task<DTO<TrabajoProveedor>> Actualizar(TrabajoProveedor trabajo);
        Task<DTO<bool>> Eliminar(TrabajoProveedor trabajo);
        Task<DTO<TrabajoProveedor>> Obtener_por_id(TrabajoProveedor trabajo);
        Task<DTO<IEnumerable<TrabajoProveedor>>> Obtener_todos();
        Task<DTO<Items_pagina<TrabajoProveedor>>> Obtener_paginado(Filtros_paginado filtros);

        // Documentos asociados a trabajo
        Task<DTO<TrabajoProveedorDocumento>> CrearDocumento(TrabajoProveedorDocumento doc);
        Task<DTO<TrabajoProveedorDocumento>> ObtenerDocumentoPorId(int documentoId);
        Task<DTO<IEnumerable<TrabajoProveedorDocumento>>> ObtenerDocumentosPorTrabajo(int trabajoId);
        Task<DTO<bool>> EliminarDocumento(int documentoId);
    }
}
