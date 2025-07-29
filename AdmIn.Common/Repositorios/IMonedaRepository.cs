using AdmIn.Common.Entidades;

namespace AdmIn.Common.Repositorios
{
    public interface IMonedaRepository
    {
        Task<DTO<Moneda>> Crear(Moneda moneda);
        Task<DTO<Moneda>> Actualizar(Moneda moneda);
        Task<DTO<bool>> Eliminar(Moneda moneda);
        Task<DTO<Moneda>> Obtener_por_id(Moneda moneda);
        Task<DTO<IEnumerable<Moneda>>> Obtener_todos();
        Task<DTO<Items_pagina<Moneda>>> Obtener_paginado(Filtros_paginado filtros);
    }
}