using AdmIn.Common;
using AdmIn.Common.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdmIn.Common.Repositorios
{
    public interface ITrabajoProveedorRepository
    {
        Task<DTO<TrabajoProveedor>> Crear(TrabajoProveedor trabajo);
        Task<DTO<TrabajoProveedor>> Actualizar(TrabajoProveedor trabajo);
        Task<DTO<bool>> Eliminar(TrabajoProveedor trabajo);
        Task<DTO<TrabajoProveedor>> Obtener_por_id(TrabajoProveedor trabajo);
        Task<DTO<IEnumerable<TrabajoProveedor>>> Obtener_todos();
        Task<DTO<Items_pagina<TrabajoProveedor>>> Obtener_paginado(Filtros_paginado filtros);
    }
}
