using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace AdmIn.API.Controllers
{
    [Route("api/images")]
    [ApiController]
    public class StaticFilesController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IContentTypeProvider _contentTypeProvider;

        public StaticFilesController(IWebHostEnvironment environment)
        {
            _environment = environment;
            _contentTypeProvider = new FileExtensionContentTypeProvider();
        }

        [HttpGet("originales/{filename}")]
        public async Task<IActionResult> GetOriginalImage(string filename)
        {
            return await ServirArchivo("originales", filename);
        }

        [HttpGet("thumbnails/{filename}")]
        public async Task<IActionResult> GetThumbnailImage(string filename)
        {
            return await ServirArchivo("thumbnails", filename);
        }

        private async Task<IActionResult> ServirArchivo(string carpeta, string filename)
        {
            try
            {
                // Validar nombre de archivo para seguridad
                if (string.IsNullOrEmpty(filename) || filename.Contains("..") || filename.Contains("/") || filename.Contains("\\"))
                {
                    return BadRequest("Nombre de archivo inválido");
                }

                var rutaArchivo = Path.Combine(_environment.WebRootPath, "uploads", "imagenes", carpeta, filename);

                if (!System.IO.File.Exists(rutaArchivo))
                {
                    return NotFound("Imagen no encontrada");
                }

                // Determinar tipo de contenido
                if (!_contentTypeProvider.TryGetContentType(filename, out var contentType))
                {
                    contentType = "application/octet-stream";
                }

                // Leer archivo
                var fileBytes = await System.IO.File.ReadAllBytesAsync(rutaArchivo);

                // Configurar headers de caché
                Response.Headers.Append("Cache-Control", "public, max-age=86400"); // 24 horas
                Response.Headers.Append("ETag", $"\"{filename}\"");

                return File(fileBytes, contentType);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al servir archivo: {ex.Message}");
            }
        }
    }
}