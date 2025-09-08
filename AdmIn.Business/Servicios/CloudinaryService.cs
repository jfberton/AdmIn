using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using AdmIn.Common;
using AdmIn.Common.Entidades;
using Microsoft.Extensions.Configuration;

namespace AdmIn.Business.Servicios
{
    public interface ICloudinaryService
    {
        Task<DTO<Imagen>> SubirImagen(Stream archivoStream, string nombreArchivo, string? descripcion = null, string? carpeta = null);
        Task<DTO<bool>> EliminarImagen(string publicId);
        string ObtenerUrlOptimizada(string publicId, int width = 300, int height = 200, string cropMode = "fill");
        string ObtenerUrlThumbnail(string publicId, int width = 300, int height = 200);
    }

    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;
        private readonly IConfiguration _config;

        public CloudinaryService(IConfiguration config)
        {
            _config = config;
            
            var account = new Account(
                _config["Cloudinary:CloudName"],
                _config["Cloudinary:ApiKey"],
                _config["Cloudinary:ApiSecret"]
            );
            
            _cloudinary = new Cloudinary(account);
        }

        public async Task<DTO<Imagen>> SubirImagen(Stream archivoStream, string nombreArchivo, string? descripcion = null, string? carpeta = null)
        {
            try
            {
                // Generar un nombre único para evitar conflictos
                var publicId = $"{carpeta ?? "admin-app"}/{Guid.NewGuid()}";
                
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(nombreArchivo, archivoStream),
                    PublicId = publicId,
                    Transformation = new Transformation()
                        .Quality("auto")
                        .FetchFormat("auto")
                        .Crop("limit")
                        .Width(1920)
                        .Height(1080),
                    Overwrite = false
                };

                var result = await _cloudinary.UploadAsync(uploadParams);

                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    // Generar URL del thumbnail automáticamente
                    var thumbUrl = ObtenerUrlThumbnail(result.PublicId);

                    var imagen = new Imagen
                    {
                        Id = Guid.NewGuid(),
                        Nombre = Path.GetFileNameWithoutExtension(nombreArchivo),
                        Descripcion = descripcion ?? string.Empty,
                        Url = result.SecureUrl.ToString(),
                        UrlThumb = thumbUrl,
                        FechaCreacion = DateTime.Now
                    };

                    return new DTO<Imagen>
                    {
                        Correcto = true,
                        Datos = imagen,
                        Mensaje = "Imagen subida correctamente a Cloudinary"
                    };
                }
                else
                {
                    return new DTO<Imagen>
                    {
                        Correcto = false,
                        Mensaje = $"Error al subir a Cloudinary: {result.Error?.Message}"
                    };
                }
            }
            catch (Exception ex)
            {
                return new DTO<Imagen>
                {
                    Correcto = false,
                    Mensaje = $"Error al subir imagen: {ex.Message}"
                };
            }
        }

        public async Task<DTO<bool>> EliminarImagen(string publicId)
        {
            try
            {
                var deleteParams = new DeletionParams(publicId);
                var result = await _cloudinary.DestroyAsync(deleteParams);

                return new DTO<bool>
                {
                    Correcto = result.StatusCode == System.Net.HttpStatusCode.OK || result.Result == "ok",
                    Datos = result.StatusCode == System.Net.HttpStatusCode.OK || result.Result == "ok",
                    Mensaje = result.StatusCode == System.Net.HttpStatusCode.OK || result.Result == "ok" ? 
                             "Imagen eliminada correctamente de Cloudinary" : 
                             $"Error al eliminar: {result.Error?.Message}"
                };
            }
            catch (Exception ex)
            {
                return new DTO<bool>
                {
                    Correcto = false,
                    Mensaje = $"Error al eliminar imagen: {ex.Message}"
                };
            }
        }

        public string ObtenerUrlOptimizada(string publicId, int width = 300, int height = 200, string cropMode = "fill")
        {
            return _cloudinary.Api.UrlImgUp
                .Transform(new Transformation()
                    .Width(width)
                    .Height(height)
                    .Crop(cropMode)
                    .Quality("auto")
                    .FetchFormat("auto"))
                .BuildUrl(publicId);
        }

        public string ObtenerUrlThumbnail(string publicId, int width = 300, int height = 200)
        {
            return ObtenerUrlOptimizada(publicId, width, height, "fill");
        }
    }
}