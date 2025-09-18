using AdmIn.Common;
using AdmIn.Common.Entidades;
using Microsoft.AspNetCore.Http;

namespace AdmIn.Business.Servicios
{
    public interface IServ_ImagenUpload
    {
        Task<DTO<Imagen>> SubirImagen(IFormFile archivo, string? descripcion = null);
        Task<DTO<bool>> EliminarImagen(Guid imagenId);
        Task<DTO<Imagen>> ActualizarImagen(Imagen imagen);
        Task<DTO<bool>> EstablecerImagenPrincipal(Guid imagenId, int inmuebleId);
        Task<DTO<bool>> EstablecerImagenPerfilUsuario(Guid imagenId, int usuarioId);
        Task<DTO<IEnumerable<Imagen>>> ObtenerImagenesInmueble(int inmuebleId);
    }
}