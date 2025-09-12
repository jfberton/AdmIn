using AdmIn.Common.Entidades;

namespace AdmIn.Common.Repositorios
{
    public interface IProveedorRepository
    {
        Task<DTO<Proveedor>> Crear(Proveedor proveedor);
        Task<DTO<Proveedor>> Actualizar(Proveedor proveedor);
        Task<DTO<bool>> Eliminar(Proveedor proveedor);
        Task<DTO<Proveedor>> Obtener_por_id(Proveedor proveedor);
        Task<DTO<IEnumerable<Proveedor>>> Obtener_todos();
        Task<DTO<Items_pagina<Proveedor>>> Obtener_paginado(Filtros_paginado filtros);
        Task<DTO<IEnumerable<Proveedor>>> Obtener_activos();
        Task<DTO<Proveedor>> Obtener_por_rfc(string rfc);
        Task<DTO<Proveedor>> Obtener_por_email(string email);
    }
}