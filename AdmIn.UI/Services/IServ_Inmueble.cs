using AdmIn.Common.Entidades;
using AdmIn.Common;

namespace AdmIn.UI.Services
{
    public interface IServ_Inmueble: IServicioBase<Inmueble>
    {
        Task<IEnumerable<string>> ObtenerEstadosInmueble();
        Task<string?> ObtenerEstadoInmueblePorId(int id);
        
        // Métodos de características
        Task<DTO<IEnumerable<CaracteristicaInmueble>>> ObtenerCaracteristicas(int inmuebleId);
        Task<DTO<CaracteristicaInmueble>> AgregarCaracteristica(int inmuebleId, CaracteristicaInmueble caracteristica);
        Task<DTO<CaracteristicaInmueble>> ActualizarCaracteristica(CaracteristicaInmueble caracteristica);
        Task<DTO<bool>> EliminarCaracteristica(int caracteristicaId);
        
        // Métodos de imágenes (opcional para futuro)
        Task AgregarImagen(int inmuebleId, Imagen imagen);
        Task EliminarImagen(int inmuebleId, Guid imagenId);
        Task EstablecerImagenPrincipal(int inmuebleId, Guid imagenId);
    }
}
