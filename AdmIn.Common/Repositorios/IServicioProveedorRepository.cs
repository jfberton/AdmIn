using AdmIn.Common.Entidades;
using AdmIn.Common;

namespace AdmIn.Common.Repositorios
{
    public interface IServicioProveedorRepository
    {
        // Métodos CRUD básicos
        Task<DTO<ServicioProveedor>> Crear(ServicioProveedor servicioProveedor);
        Task<DTO<ServicioProveedor>> Actualizar(ServicioProveedor servicioProveedor);
        Task<DTO<bool>> Eliminar(ServicioProveedor servicioProveedor);
        Task<DTO<ServicioProveedor>> Obtener_por_id(ServicioProveedor servicioProveedor);
        Task<DTO<IEnumerable<ServicioProveedor>>> Obtener_todos();
        Task<DTO<Items_pagina<ServicioProveedor>>> Obtener_paginado(Filtros_paginado filtros);
        
        // Métodos específicos de búsqueda
        Task<DTO<IEnumerable<ServicioProveedor>>> Obtener_por_proveedor(int proveedorId);
        Task<DTO<IEnumerable<ServicioProveedor>>> Obtener_por_servicio(int servicioId);
    }
}