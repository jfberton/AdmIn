using AdmIn.Common;
using AdmIn.Common.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdmIn.Business.Servicios
{
    public interface IServ_HistorialTrabajo
    {
        Task<DTO<HistorialTrabajo>> Crear(HistorialTrabajo historialTrabajo);
        Task<DTO<HistorialTrabajo>> Actualizar(HistorialTrabajo historialTrabajo);
        Task<DTO<bool>> Eliminar(HistorialTrabajo historialTrabajo);
        Task<DTO<HistorialTrabajo>> Obtener_por_id(int historialTrabajoId);
        Task<DTO<IEnumerable<HistorialTrabajo>>> Obtener_todos();
        Task<DTO<Items_pagina<HistorialTrabajo>>> Obtener_paginado(Filtros_paginado filtros);
    }
}
