using AdmIn.Common;
using AdmIn.Common.Entidades;

namespace AdmIn.Business.Servicios
{
    public interface IServ_InmuebleCondicion
    {
        Task<DTO<InmuebleCondicion>> Crear(InmuebleCondicion condicion);
        Task<DTO<InmuebleCondicion>> Actualizar(InmuebleCondicion condicion);
        Task<DTO<bool>> Eliminar(InmuebleCondicion condicion);
        Task<DTO<InmuebleCondicion>> Obtener_por_id(InmuebleCondicion condicion);
        Task<DTO<IEnumerable<InmuebleCondicion>>> Obtener_todos();
        Task<DTO<Items_pagina<InmuebleCondicion>>> Obtener_paginado(Filtros_paginado filtros);
        
        // Métodos específicos
        Task<DTO<IEnumerable<InmuebleCondicion>>> Obtener_activos();
        Task<DTO<InmuebleCondicion>> Obtener_por_nombre(string nombre);
        Task<DTO<InmuebleCondicion>> Obtener_por_defecto();
    }
}