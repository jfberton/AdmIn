using AdmIn.Common.Entidades;
using AdmIn.Common;

namespace AdmIn.UI.Services
{
    public interface IServ_Inmueble: IServicioBase<Inmueble>
    {
        Task<IEnumerable<string>> ObtenerEstadosInmueble();
        Task<string?> ObtenerEstadoInmueblePorId(int id);
        Task AgregarCaracteristica(int inmuebleId, CaracteristicaInmueble caracteristica);
        Task ActualizarCaracteristica(int inmuebleId, CaracteristicaInmueble caracteristica);
        Task EliminarCaracteristica(int inmuebleId, int caracteristicaId);
        Task AgregarImagen(int inmuebleId, Imagen imagen);
        Task EliminarImagen(int inmuebleId, Guid imagenId);
        Task EstablecerImagenPrincipal(int inmuebleId, Guid imagenId);
        Task<DTO<IEnumerable<CaracteristicaInmueble>>> ObtenerCaracteristicas(int inmuebleId);
    }
}
