using AdmIn.Common.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdmIn.Common;

namespace AdmIn.Common.Repositorios
{
    public interface IHistorialDetalleTrabajoRepository
    {
        Task<DTO<HistorialDetalleTrabajo>> Crear(HistorialDetalleTrabajo historialDetalleTrabajo);
        Task<DTO<HistorialDetalleTrabajo>> Actualizar(HistorialDetalleTrabajo historialDetalleTrabajo);
        Task<DTO<bool>> Eliminar(HistorialDetalleTrabajo historialDetalleTrabajo);
        Task<DTO<HistorialDetalleTrabajo>> Obtener_por_id(int historialDetalleTrabajoId);
        Task<DTO<IEnumerable<HistorialDetalleTrabajo>>> Obtener_todos();
        Task<DTO<Items_pagina<HistorialDetalleTrabajo>>> Obtener_paginado(Filtros_paginado filtros);
    }
}
