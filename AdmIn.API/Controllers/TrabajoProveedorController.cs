using AdmIn.Business.Servicios;
using AdmIn.Common;
using AdmIn.Common.Entidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace AdmIn.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrabajoProveedorController : ControllerBase
    {
        private readonly IServ_TrabajoProveedor _servicio;
        public TrabajoProveedorController(IServ_TrabajoProveedor servicio)
        {
            _servicio = servicio;
        }

        [HttpPost("nuevo")]
        [Authorize]
        public async Task<DTO<TrabajoProveedor>> Crear([FromBody] TrabajoProveedor trabajo)
            => await _servicio.Crear(trabajo);

        [HttpPost("modificar")]
        [Authorize]
        public async Task<DTO<TrabajoProveedor>> Modificar([FromBody] TrabajoProveedor trabajo)
            => await _servicio.Actualizar(trabajo);

        [HttpDelete("eliminar/{id}")]
        [Authorize]
        public async Task<DTO<bool>> Eliminar(int id)
            => await _servicio.Eliminar(new TrabajoProveedor { Id = id });

        [HttpGet("obtener_por_id/{id}")]
        [Authorize]
        public async Task<DTO<TrabajoProveedor>> ObtenerPorId(int id)
            => await _servicio.Obtener_por_id(id);

        [HttpGet("obtener_todos")]
        [Authorize]
        public async Task<DTO<IEnumerable<TrabajoProveedor>>> ObtenerTodos()
            => await _servicio.Obtener_todos();

        [HttpPost("obtener_paginado")]
        [Authorize]
        public async Task<DTO<Items_pagina<TrabajoProveedor>>> ObtenerPaginado([FromBody] Filtros_paginado filtros)
            => await _servicio.Obtener_paginado(filtros);

        [HttpPost("aceptar")]
        [Authorize(Roles = "proveedor")]
        public async Task<DTO<bool>> AceptarTrabajo([FromBody] AceptarTrabajoRequest request)
            => await _servicio.AceptarTrabajo(request);

        [HttpPost("marcar_finalizado")]
        [Authorize(Roles = "proveedor")]
        public async Task<DTO<bool>> MarcarFinalizado([FromBody] TrabajoAccionRequest request)
            => await _servicio.MarcarFinalizado(request);

        [HttpPost("revisar_finalizacion")]
        [Authorize]
        public async Task<DTO<bool>> RevisarFinalizacion([FromBody] RevisarFinalizacionRequest request)
            => await _servicio.RevisarFinalizacion(request);

        [HttpPost("cancelar")]
        [Authorize]
        public async Task<DTO<bool>> CancelarTrabajo([FromBody] TrabajoAccionRequest request)
            => await _servicio.CancelarTrabajo(request);

        // New endpoints: solicitar and rechazar
        [HttpPost("solicitar")]
        [Authorize]
        public async Task<DTO<bool>> SolicitarTrabajo([FromBody] TrabajoAccionRequest request)
            => await _servicio.SolicitarTrabajo(request);

        [HttpPost("rechazar")]
        [Authorize(Roles = "proveedor")]
        public async Task<DTO<bool>> RechazarTrabajo([FromBody] TrabajoAccionRequest request)
            => await _servicio.RechazarTrabajo(request);

        // Documents endpoints
        [HttpPost("{trabajoId}/documentos")]
        [Authorize]
        public async Task<IActionResult> SubirDocumento(int trabajoId, IFormFile file, [FromForm] string descripcion)
        {
            if (file == null || file.Length == 0) return BadRequest(new DTO<TrabajoProveedorDocumento> { Correcto = false, Mensaje = "Archivo no proporcionado" });

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            var bytes = ms.ToArray();

            var doc = new TrabajoProveedorDocumento
            {
                TrabajoProveedorId = trabajoId,
                NombreSubidor = User?.Identity?.Name ?? "",
                Descripcion = descripcion ?? string.Empty,
                FechaSubida = System.DateTime.Now,
                NombreArchivo = file.FileName,
                ContentType = file.ContentType ?? "application/octet-stream",
                Contenido = bytes
            };

            var res = await _servicio.CrearDocumento(doc);
            if (res != null && res.Correcto) return Ok(res);
            return BadRequest(res ?? new DTO<TrabajoProveedorDocumento> { Correcto = false, Mensaje = "Error subiendo documento" });
        }

        // New: accept JSON payload with base64/bytes so UI can post document as JSON
        [HttpPost("{trabajoId}/documentos/json")]
        [Authorize]
        public async Task<IActionResult> SubirDocumentoJson(int trabajoId, [FromBody] TrabajoProveedorDocumento doc)
        {
            if (doc == null) return BadRequest(new DTO<TrabajoProveedorDocumento> { Correcto = false, Mensaje = "Payload vacío" });

            // Ensure TrabajoProveedorId matches route
            doc.TrabajoProveedorId = trabajoId;
            doc.NombreSubidor = string.IsNullOrEmpty(doc.NombreSubidor) ? User?.Identity?.Name ?? string.Empty : doc.NombreSubidor;
            doc.FechaSubida = doc.FechaSubida == default ? System.DateTime.Now : doc.FechaSubida;

            var res = await _servicio.CrearDocumento(doc);
            if (res != null && res.Correcto) return Ok(res);
            return BadRequest(res ?? new DTO<TrabajoProveedorDocumento> { Correcto = false, Mensaje = "Error subiendo documento" });
        }

        [HttpGet("{trabajoId}/documentos")]
        [Authorize]
        public async Task<DTO<IEnumerable<TrabajoProveedorDocumento>>> ListarDocumentos(int trabajoId)
            => await _servicio.ObtenerDocumentosPorTrabajo(trabajoId);

        [HttpGet("documentos/{documentoId}")]
        [Authorize]
        public async Task<IActionResult> DescargarDocumento(int documentoId)
        {
            var res = await _servicio.ObtenerDocumentoPorId(documentoId);
            if (res == null || !res.Correcto || res.Datos == null) return NotFound(res ?? new DTO<TrabajoProveedorDocumento> { Correcto = false, Mensaje = "Documento no encontrado" });

            var doc = res.Datos;
            if (doc.Contenido == null || doc.Contenido.Length == 0) return NotFound(new DTO<TrabajoProveedorDocumento> { Correcto = false, Mensaje = "Contenido no disponible" });

            return File(doc.Contenido, doc.ContentType ?? "application/octet-stream", doc.NombreArchivo);
        }

        // New JSON-returning endpoint for UI preview that returns DTO with bytes
        [HttpGet("documentos/json/{documentoId}")]
        [Authorize]
        public async Task<DTO<TrabajoProveedorDocumento>> ObtenerDocumentoJson(int documentoId)
        {
            var res = await _servicio.ObtenerDocumentoPorId(documentoId);
            if (res == null) return new DTO<TrabajoProveedorDocumento> { Correcto = false, Mensaje = "Error obteniendo documento" };
            return res;
        }

        [HttpDelete("documentos/{documentoId}")]
        [Authorize]
        public async Task<IActionResult> EliminarDocumento(int documentoId)
        {
            // Get document to verify owner
            var docRes = await _servicio.ObtenerDocumentoPorId(documentoId);
            if (docRes == null || !docRes.Correcto || docRes.Datos == null)
            {
                return NotFound(docRes ?? new DTO<TrabajoProveedorDocumento> { Correcto = false, Mensaje = "Documento no encontrado" });
            }

            var doc = docRes.Datos;

            // Get current user identity name
            var userName = User?.Identity?.Name ?? string.Empty;

            // Only allow deletion if the current user is the uploader
            if (string.IsNullOrWhiteSpace(userName) || !string.Equals(userName, doc.NombreSubidor, StringComparison.OrdinalIgnoreCase))
            {
                return Forbid();
            }

            var res = await _servicio.EliminarDocumento(documentoId);
            if (res == null)
            {
                return StatusCode(500, new DTO<bool> { Correcto = false, Mensaje = "Error eliminando documento" });
            }

            return Ok(res);
        }
    }
}
