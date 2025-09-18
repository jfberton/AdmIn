using AdmIn.Common;
using AdmIn.Common.Entidades;

namespace AdmIn.Business.Servicios
{
    public interface IServ_Inmueble : IServicioBase<Inmueble>
    {
        Task<DTO<IEnumerable<string>>> Obtener_estados();
        Task<DTO<IEnumerable<Inmueble>>> Obtener_por_estado(string estado);
        Task<DTO<IEnumerable<Inmueble>>> Obtener_por_administrador(int administradorId);
        Task<DTO<IEnumerable<Inmueble>>> Obtener_por_ubicacion(string pais, string estado, string ciudad);
        Task<DTO<IEnumerable<Inmueble>>> Obtener_por_rango_precio(decimal precioMin, decimal precioMax);
        
        // Métodos para características
        Task<DTO<IEnumerable<CaracteristicaInmueble>>> Obtener_caracteristicas(int inmuebleId);
        Task<DTO<CaracteristicaInmueble>> Agregar_caracteristica(int inmuebleId, CaracteristicaInmueble caracteristica);
        Task<DTO<CaracteristicaInmueble>> Actualizar_caracteristica(CaracteristicaInmueble caracteristica);
        Task<DTO<bool>> Eliminar_caracteristica(int caracteristicaId);
    }
}