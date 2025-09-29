using AdmIn.Common;
using AdmIn.Common.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdmIn.UI.Services
{
    public interface IServ_HistorialDetalleTrabajo
    {
        Task<DTO<HistorialDetalleTrabajo>> Crear(HistorialDetalleTrabajo detalle);
        Task<DTO<HistorialDetalleTrabajo>> Actualizar(HistorialDetalleTrabajo detalle);
        Task<DTO<bool>> Eliminar(HistorialDetalleTrabajo detalle);
        Task<DTO<HistorialDetalleTrabajo>> Obtener_por_id(int id);
        Task<DTO<IEnumerable<HistorialDetalleTrabajo>>> Obtener_todos();
        Task<DTO<Items_pagina<HistorialDetalleTrabajo>>> Obtener_paginado(Filtros_paginado filtros);
    }
}