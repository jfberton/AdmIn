using AdmIn.Common;
using AdmIn.Common.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdmIn.Business.Servicios
{
    public interface IServ_DetalleTrabajo
    {
        Task<DTO<DetalleTrabajo>> Crear(DetalleTrabajo detalleTrabajo);
        Task<DTO<DetalleTrabajo>> Actualizar(DetalleTrabajo detalleTrabajo);
        Task<DTO<bool>> Eliminar(DetalleTrabajo detalleTrabajo);
        Task<DTO<DetalleTrabajo>> Obtener_por_id(int detalleTrabajoId);
        Task<DTO<IEnumerable<DetalleTrabajo>>> Obtener_todos();
        Task<DTO<Items_pagina<DetalleTrabajo>>> Obtener_paginado(Filtros_paginado filtros);
    }
}
