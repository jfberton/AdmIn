using AdmIn.Common.Entidades;

namespace AdmIn.Common.Repositorios
{
    public interface ITipoServicioRepository
    {
        Task<DTO<TipoServicio>> Crear(TipoServicio tipoServicio);
        Task<DTO<TipoServicio>> Actualizar(TipoServicio tipoServicio);
        Task<DTO<bool>> Eliminar(TipoServicio tipoServicio);
        Task<DTO<TipoServicio>> Obtener_por_id(TipoServicio tipoServicio);
        Task<DTO<IEnumerable<TipoServicio>>> Obtener_todos();
        Task<DTO<Items_pagina<TipoServicio>>> Obtener_paginado(Filtros_paginado filtros);
    }
}