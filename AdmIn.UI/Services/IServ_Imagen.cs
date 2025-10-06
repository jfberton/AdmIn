using AdmIn.Common;
using AdmIn.Common.Entidades;

namespace AdmIn.UI.Services
{
    public interface IServ_Imagen
    {
        Task<DTO<Imagen>> SubirImagen(Stream archivoStream, string nombreArchivo, string? descripcion = null);
        Task<DTO<Imagen>> SubirImagenParaInmueble(Stream archivoStream, string nombreArchivo, int inmuebleId, string? descripcion = null, bool establecerComoPrincipal = false);
        Task<DTO<Imagen>> SubirImagenParaUsuario(Stream archivoStream, string nombreArchivo, int usuarioId, string? descripcion = null, bool establecerComoPerfil = true);
        Task<DTO<Imagen>> SubirImagenParaTrabajo(Stream archivoStream, string nombreArchivo, int trabajoId, string? descripcion = null);
        Task<DTO<Imagen>> SubirImagenParaDetalle(Stream archivoStream, string nombreArchivo, int detalleId, string? descripcion = null);
        Task<DTO<Imagen>> ObtenerPorId(Guid imagenId);
        Task<DTO<IEnumerable<Imagen>>> ObtenerTodas();
        Task<DTO<Items_pagina<Imagen>>> ObtenerPaginado(Filtros_paginado filtros);
        Task<DTO<IEnumerable<Imagen>>> ObtenerPorInmueble(int inmuebleId);
        Task<DTO<Imagen>> Actualizar(Imagen imagen);
        Task<DTO<bool>> EstablecerComoPrincipal(Guid imagenId, int inmuebleId);
        Task<DTO<bool>> EstablecerImagenPerfil(Guid imagenId, int usuarioId);
        Task<DTO<bool>> Eliminar(Guid imagenId);
        Task<DTO<bool>> AsociarA_Detalle(Guid imagenId, int detalleId);
    }
}