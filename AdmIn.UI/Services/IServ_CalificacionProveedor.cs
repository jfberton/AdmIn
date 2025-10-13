using AdmIn.Common;
using AdmIn.Common.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdmIn.UI.Services
{
    public interface IServ_CalificacionProveedor
    {
        Task<DTO<CalificacionProveedor>> Crear(CalificacionProveedor calificacionProveedor);
        Task<DTO<CalificacionProveedor>> Actualizar(CalificacionProveedor calificacionProveedor);
        Task<DTO<bool>> Eliminar(int calificacionProveedorId);
        Task<DTO<CalificacionProveedor>> Obtener_por_id(int calificacionProveedorId);
        Task<DTO<IEnumerable<CalificacionProveedor>>> Obtener_todos();
        Task<DTO<Items_pagina<CalificacionProveedor>>> Obtener_paginado(Filtros_paginado filtros);
    }
}
