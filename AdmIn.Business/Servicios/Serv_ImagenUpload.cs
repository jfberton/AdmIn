using AdmIn.Common;
using AdmIn.Common.Entidades;
using AdmIn.Common.Repositorios;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace AdmIn.Business.Servicios
{
    public class Serv_ImagenUpload : IServ_ImagenUpload
    {
        private readonly IImagenRepository _imagenRepo;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IConfiguration _config;
        private readonly string _uploadsPath;
        private readonly string _originalesPath;
        private readonly string _thumbnailsPath;
        
        // Configuración de imágenes
        private readonly string[] _formatosPermitidos = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        private readonly long _tamañoMaximo = 10 * 1024 * 1024; // 10MB
        private readonly int _anchoThumbnail = 300;
        private readonly int _altoThumbnail = 200;

        public Serv_ImagenUpload(IImagenRepository imagenRepository, ICloudinaryService cloudinaryService, IConfiguration config)
        {
            _imagenRepo = imagenRepository;
            _cloudinaryService = cloudinaryService;
            _config = config;
            
            // Configurar rutas usando el directorio actual como base (para almacenamiento local)
            var baseDirectory = Directory.GetCurrentDirectory();
            var wwwrootPath = Path.Combine(baseDirectory, "wwwroot");
            _uploadsPath = Path.Combine(wwwrootPath, "uploads", "imagenes");
            _originalesPath = Path.Combine(_uploadsPath, "originales");
            _thumbnailsPath = Path.Combine(_uploadsPath, "thumbnails");
            
            // Crear directorios si no existen (siempre para almacenamiento local)
            CrearDirectoriosNecesarios();
        }

        public async Task<DTO<Imagen>> SubirImagen(IFormFile archivo, string? descripcion = null)
        {
            try
            {
                // Validaciones
                var validacion = ValidarArchivo(archivo);
                if (!validacion.Correcto)
                    return validacion;

                // Decidir entre Cloudinary o almacenamiento local
                var useCloudinaryValue = _config.GetSection("ImageStorage:UseCloudinary")?.Value;
                var useCloudinary = !string.IsNullOrEmpty(useCloudinaryValue) && bool.Parse(useCloudinaryValue);
                
                if (useCloudinary)
                {
                    return await SubirACloudinary(archivo, descripcion);
                }
                else
                {
                    return await SubirLocal(archivo, descripcion);
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

        private async Task<DTO<Imagen>> SubirACloudinary(IFormFile archivo, string? descripcion)
        {
            using var stream = archivo.OpenReadStream();
            
            // Subir a Cloudinary
            var resultadoCloudinary = await _cloudinaryService.SubirImagen(
                stream, 
                archivo.FileName, 
                descripcion, 
                "admin-app"
            );

            if (resultadoCloudinary.Correcto)
            {
                // Guardar en base de datos
                return await _imagenRepo.Crear(resultadoCloudinary.Datos);
            }

            return resultadoCloudinary;
        }

        private async Task<DTO<Imagen>> SubirLocal(IFormFile archivo, string? descripcion)
        {
            // Tu código actual de almacenamiento local
            var guidUnico = Guid.NewGuid();
            var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();
            
            // Nombres de archivos
            var nombreOriginal = $"{guidUnico}{extension}";
            var nombreThumbnail = $"{guidUnico}-thumb{extension}";
            
            // Rutas físicas
            var rutaOriginal = Path.Combine(_originalesPath, nombreOriginal);
            var rutaThumbnail = Path.Combine(_thumbnailsPath, nombreThumbnail);
            
            // Procesar y guardar imagen original
            using (var streamOriginal = new FileStream(rutaOriginal, FileMode.Create))
            {
                await archivo.CopyToAsync(streamOriginal);
            }
            
            // Crear thumbnail
            await CrearThumbnail(rutaOriginal, rutaThumbnail);
            
            // Crear entidad imagen
            var imagen = new Imagen
            {
                Id = guidUnico,
                Nombre = Path.GetFileNameWithoutExtension(archivo.FileName),
                Descripcion = descripcion ?? string.Empty,
                Url = $"/api/images/originales/{nombreOriginal}",
                UrlThumb = $"/api/images/thumbnails/{nombreThumbnail}",
                FechaCreacion = DateTime.Now
            };
            
            // Guardar en base de datos
            var resultado = await _imagenRepo.Crear(imagen);
            
            if (!resultado.Correcto)
            {
                // Si falla la BD, eliminar archivos creados
                EliminarArchivos(rutaOriginal, rutaThumbnail);
                return new DTO<Imagen>
                {
                    Correcto = false,
                    Mensaje = $"Error al guardar en base de datos: {resultado.Mensaje}"
                };
            }
            
            return resultado;
        }

        public async Task<DTO<bool>> EliminarImagen(Guid imagenId)
        {
            try
            {
                // Obtener imagen de BD
                var imagenResult = await _imagenRepo.Obtener_por_id(new Imagen { Id = imagenId });
                if (!imagenResult.Correcto || imagenResult.Datos == null)
                {
                    return new DTO<bool>
                    {
                        Correcto = false,
                        Mensaje = "Imagen no encontrada"
                    };
                }

                var imagen = imagenResult.Datos;
                
                // Eliminar de base de datos
                var eliminacionResult = await _imagenRepo.Eliminar(imagen);
                if (!eliminacionResult.Correcto)
                    return eliminacionResult;

                // Eliminar archivos físicos según el tipo de almacenamiento
                var useCloudinaryValue = _config.GetSection("ImageStorage:UseCloudinary")?.Value;
                var useCloudinary = !string.IsNullOrEmpty(useCloudinaryValue) && bool.Parse(useCloudinaryValue);
                
                if (useCloudinary)
                {
                    // Extraer publicId de la URL de Cloudinary y eliminar
                    var publicId = ExtraerPublicIdDeUrl(imagen.Url);
                    if (!string.IsNullOrEmpty(publicId))
                    {
                        await _cloudinaryService.EliminarImagen(publicId);
                    }
                }
                else
                {
                    EliminarArchivosDeImagen(imagen);
                }
                
                return new DTO<bool>
                {
                    Correcto = true,
                    Datos = true,
                    Mensaje = "Imagen eliminada correctamente"
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

        public async Task<DTO<Imagen>> ActualizarImagen(Imagen imagen)
        {
            return await _imagenRepo.Actualizar(imagen);
        }

        public async Task<DTO<bool>> EstablecerImagenPrincipal(Guid imagenId, int inmuebleId)
        {
            return await _imagenRepo.Establecer_como_principal(imagenId, inmuebleId);
        }

        public async Task<DTO<bool>> EstablecerImagenPerfilUsuario(Guid imagenId, int usuarioId)
        {
            return await _imagenRepo.Establecer_imagen_perfil_usuario(imagenId, usuarioId);
        }

        public async Task<DTO<IEnumerable<Imagen>>> ObtenerImagenesInmueble(int inmuebleId)
        {
            return await _imagenRepo.Obtener_por_inmueble(inmuebleId);
        }

        #region Métodos privados

        private void CrearDirectoriosNecesarios()
        {
            Directory.CreateDirectory(_uploadsPath);
            Directory.CreateDirectory(_originalesPath);
            Directory.CreateDirectory(_thumbnailsPath);
        }

        private DTO<Imagen> ValidarArchivo(IFormFile archivo)
        {
            if (archivo == null || archivo.Length == 0)
            {
                return new DTO<Imagen>
                {
                    Correcto = false,
                    Mensaje = "No se ha seleccionado ningún archivo"
                };
            }

            if (archivo.Length > _tamañoMaximo)
            {
                return new DTO<Imagen>
                {
                    Correcto = false,
                    Mensaje = $"El archivo es demasiado grande. Tamaño máximo: {_tamañoMaximo / (1024 * 1024)}MB"
                };
            }

            var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();
            if (!_formatosPermitidos.Contains(extension))
            {
                return new DTO<Imagen>
                {
                    Correcto = false,
                    Mensaje = $"Formato no permitido. Formatos válidos: {string.Join(", ", _formatosPermitidos)}"
                };
            }

            return new DTO<Imagen> { Correcto = true };
        }

        private async Task CrearThumbnail(string rutaOriginal, string rutaThumbnail)
        {
            try
            {
                using var image = await Image.LoadAsync(rutaOriginal);
                
                // Redimensionar manteniendo aspecto
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(_anchoThumbnail, _altoThumbnail),
                    Mode = ResizeMode.Max
                }));
                
                // Guardar como JPEG con calidad optimizada
                await image.SaveAsJpegAsync(rutaThumbnail, new JpegEncoder
                {
                    Quality = 80
                });
            }
            catch (Exception ex)
            {
                // Log error pero no fallar el proceso principal
                Console.WriteLine($"Error creando thumbnail: {ex.Message}");
                
                // Crear copia simple si falla el redimensionamiento
                File.Copy(rutaOriginal, rutaThumbnail, true);
            }
        }

        private void EliminarArchivos(string rutaOriginal, string rutaThumbnail)
        {
            try
            {
                if (File.Exists(rutaOriginal))
                    File.Delete(rutaOriginal);
                    
                if (File.Exists(rutaThumbnail))
                    File.Delete(rutaThumbnail);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error eliminando archivos: {ex.Message}");
            }
        }

        private void EliminarArchivosDeImagen(Imagen imagen)
        {
            try
            {
                // Extraer nombres de archivo de las URLs
                var nombreOriginal = Path.GetFileName(imagen.Url);
                var nombreThumbnail = Path.GetFileName(imagen.UrlThumb);
                
                var rutaOriginal = Path.Combine(_originalesPath, nombreOriginal);
                var rutaThumbnail = Path.Combine(_thumbnailsPath, nombreThumbnail);
                
                EliminarArchivos(rutaOriginal, rutaThumbnail);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error eliminando archivos de imagen: {ex.Message}");
            }
        }

        private string ExtraerPublicIdDeUrl(string url)
        {
            try
            {
                // Las URLs de Cloudinary tienen el formato:
                // https://res.cloudinary.com/cloud_name/image/upload/v1234567890/admin-app/guid.jpg
                // El publicId sería: admin-app/guid
                
                if (url.Contains("cloudinary.com"))
                {
                    var uri = new Uri(url);
                    var segments = uri.Segments;
                    
                    // Buscar el segmento después de "upload/"
                    for (int i = 0; i < segments.Length; i++)
                    {
                        if (segments[i].TrimEnd('/') == "upload" && i + 1 < segments.Length)
                        {
                            // Tomar desde el siguiente segmento hasta el final (sin extensión)
                            var publicIdParts = segments.Skip(i + 2).ToArray(); // Saltar "upload/" y version
                            var publicId = string.Join("", publicIdParts).TrimEnd('/');
                            
                            // Remover extensión del archivo
                            return Path.ChangeExtension(publicId, null)?.TrimEnd('.');
                        }
                    }
                }
                
                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        #endregion
    }
}