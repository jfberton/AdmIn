using AdmIn.Business.Entidades;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdmIn.UI.Services.Mock
{
    public interface IInmuebleService
    {
        Task<IEnumerable<Inmueble>> ObtenerInmuebles();
        Task<Inmueble?> ObtenerInmueblePorId(int id);
        Task<int> CrearInmueble(Inmueble inmueble);
        Task ActualizarInmueble(Inmueble inmueble);
        Task EliminarInmueble(int id);
        Task<IEnumerable<EstadoInmueble>> ObtenerEstadosInmueble();
        Task<EstadoInmueble?> ObtenerEstadoInmueblePorId(int id);
        Task AgregarCaracteristica(int inmuebleId, CaracteristicaInmueble caracteristica);
        Task ActualizarCaracteristica(int inmuebleId, CaracteristicaInmueble caracteristica);
        Task EliminarCaracteristica(int inmuebleId, int caracteristicaId);
        Task AgregarImagen(int inmuebleId, Imagen imagen);
        Task EliminarImagen(int inmuebleId, Guid imagenId);
        Task EstablecerImagenPrincipal(int inmuebleId, Guid imagenId);
    }
}
