using AdmIn.Common.Entidades;
using AdmIn.Common;

namespace AdmIn.Common.Repositorios
{
    public interface IProveedorRepository
    {
        // Métodos CRUD básicos
        Task<DTO<Proveedor>> Crear(Proveedor proveedor);
        Task<DTO<Proveedor>> Actualizar(Proveedor proveedor);
        Task<DTO<bool>> Eliminar(Proveedor proveedor);
        Task<DTO<Proveedor>> Obtener_por_id(Proveedor proveedor);
        Task<DTO<IEnumerable<Proveedor>>> Obtener_todos();
        Task<DTO<Items_pagina<Proveedor>>> Obtener_paginado(Filtros_paginado filtros);
        
        // Métodos específicos de proveedor
        Task<DTO<IEnumerable<Proveedor>>> Obtener_activos();
        Task<DTO<Proveedor>> Obtener_por_rfc(string rfc);
        Task<DTO<Proveedor>> Obtener_por_email(string email);
        
        // Método para paginación con filtro de estado
        Task<DTO<Items_pagina<Proveedor>>> Obtener_paginado_por_estado(Filtros_paginado filtros, bool? soloActivos = null);
        
        // Métodos para gestión de servicios de proveedor
        Task<DTO<IEnumerable<TipoServicio>>> Obtener_servicios_proveedor(int proveedorId);
        Task<DTO<bool>> Actualizar_servicios_proveedor(int proveedorId, List<int> serviciosIds);
        Task<DTO<bool>> Eliminar_servicios_proveedor(int proveedorId);
    }
}