using AdmIn.Common.Entidades;

namespace AdmIn.Common.Repositorios
{
    public interface IRolRepository
    {
        Task<DTO<Rol>> Crear(Rol rol);
        Task<DTO<Rol>> Actualizar(Rol rol);
        Task<DTO<bool>> Eliminar(Rol rol);
        Task<DTO<Rol>> Obtener_por_id(Rol rol);
        Task<DTO<IEnumerable<Rol>>> Obtener_todos();
        Task<DTO<Items_pagina<Rol>>> Obtener_paginado(Filtros_paginado filtros);
        Task<DTO<IEnumerable<Rol>>> Obtener_por_usuario(int usuarioId);
    }
}