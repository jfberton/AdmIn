using AdmIn.Common.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdmIn.Common;

namespace AdmIn.Common.Repositorios
{
    public interface ICalificacionProveedorRepository
    {
        Task<DTO<CalificacionProveedor>> Crear(CalificacionProveedor calificacionProveedor);
        Task<DTO<CalificacionProveedor>> Actualizar(CalificacionProveedor calificacionProveedor);
        Task<DTO<bool>> Eliminar(CalificacionProveedor calificacionProveedor);
        Task<DTO<CalificacionProveedor>> Obtener_por_id(int calificacionProveedorId);
        Task<DTO<IEnumerable<CalificacionProveedor>>> Obtener_todos();
        Task<DTO<Items_pagina<CalificacionProveedor>>> Obtener_paginado(Filtros_paginado filtros);
    }
}
