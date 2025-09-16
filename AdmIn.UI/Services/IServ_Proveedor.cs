using AdmIn.Common;
using AdmIn.Common.Entidades;

namespace AdmIn.UI.Services
{
    public interface IServ_Proveedor : IServicioBase<Proveedor>
    {
        Task<DTO<IEnumerable<Proveedor>>> Obtener_activos();
        Task<DTO<Items_pagina<Proveedor>>> Obtener_paginado_por_estado(Filtros_paginado filtros, bool? soloActivos = null);
        Task<DTO<Proveedor>> Obtener_por_rfc(string rfc);
        Task<DTO<Proveedor>> Obtener_por_email(string email);
        Task<DTO<bool>> Validar_rfc_unico(string rfc, int? proveedorId = null);
        Task<DTO<bool>> Validar_email_unico(string email, int? proveedorId = null);
        Task<DTO<IEnumerable<TipoServicio>>> Obtener_servicios_proveedor(int proveedorId);
        Task<DTO<bool>> Actualizar_servicios_proveedor(int proveedorId, List<int> serviciosIds);
        
        // Métodos temporales de diagnóstico
        Task<object> PruebaEndpointSimple();
        Task<object> PruebaEndpointFiltros();
        Task<object> PruebaEndpointJsonRaw();
    }
}