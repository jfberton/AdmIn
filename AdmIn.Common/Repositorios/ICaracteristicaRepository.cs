using AdmIn.Common.Entidades;

namespace AdmIn.Common.Repositorios
{
    public interface ICaracteristicaRepository
    {
        Task<DTO<Caracteristica>> Crear(Caracteristica caracteristica);
        Task<DTO<Caracteristica>> Actualizar(Caracteristica caracteristica);
        Task<DTO<bool>> Eliminar(Caracteristica caracteristica);
        Task<DTO<Caracteristica>> Obtener_por_id(Caracteristica caracteristica);
        Task<DTO<IEnumerable<Caracteristica>>> Obtener_todos();
        Task<DTO<Items_pagina<Caracteristica>>> Obtener_paginado(Filtros_paginado filtros);
        Task<DTO<IEnumerable<Caracteristica>>> Obtener_por_tipo(string tipo);
        Task<DTO<IEnumerable<string>>> Obtener_tipos();
    }
}