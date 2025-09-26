using AdmIn.Common;
using AdmIn.Common.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdmIn.Business.Servicios
{
    public interface IServ_HistorialDetalleTrabajo
    {
        Task<DTO<HistorialDetalleTrabajo>> Crear(HistorialDetalleTrabajo historialDetalleTrabajo);
        Task<DTO<HistorialDetalleTrabajo>> Actualizar(HistorialDetalleTrabajo historialDetalleTrabajo);
        Task<DTO<bool>> Eliminar(HistorialDetalleTrabajo historialDetalleTrabajo);
        Task<DTO<HistorialDetalleTrabajo>> Obtener_por_id(int historialDetalleTrabajoId);
        Task<DTO<IEnumerable<HistorialDetalleTrabajo>>> Obtener_todos();
        Task<DTO<Items_pagina<HistorialDetalleTrabajo>>> Obtener_paginado(Filtros_paginado filtros);
    }
}
