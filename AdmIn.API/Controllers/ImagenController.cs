using AdmIn.Common;
using AdmIn.Common.Entidades;
using AdmIn.Common.Repositorios;
using AdmIn.Business.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace AdmIn.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagenController : ControllerBase
    {
        private readonly IServ_ImagenUpload _servicioUpload;
        private readonly IImagenRepository _imagenRepo;
        private readonly IWebHostEnvironment _environment;

        public ImagenController(
            IServ_ImagenUpload servicioUpload, 
            IImagenRepository imagenRepository,
            IWebHostEnvironment environment)
        {
            _servicioUpload = servicioUpload;
            _imagenRepo = imagenRepository;
            _environment = environment;
        }

        [HttpPost("upload")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<Imagen>> SubirImagen([FromForm] IFormFile archivo, [FromForm] string? descripcion = null)
        {
            try
            {
                return await _servicioUpload.SubirImagen(archivo, descripcion);
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

        [HttpPost("upload-for-property/{inmuebleId}")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<Imagen>> SubirImagenParaInmueble(
            int inmuebleId,
            [FromForm] IFormFile archivo, 
            [FromForm] string? descripcion = null,
            [FromForm] bool establecerComoPrincipal = false)
        {
            try
            {
                var resultado = await _servicioUpload.SubirImagen(archivo, descripcion);
                
                if (resultado.Correcto && establecerComoPrincipal)
                {
                    await _servicioUpload.EstablecerImagenPrincipal(resultado.Datos.Id, inmuebleId);
                }
                
                return resultado;
            }
            catch (Exception ex)
            {
                return new DTO<Imagen>
                {
                    Correcto = false,
                    Mensaje = $"Error al subir imagen para inmueble: {ex.Message}"
                };
            }
        }

        [HttpPost("upload-for-user/{usuarioId}")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<Imagen>> SubirImagenParaUsuario(int usuarioId, IFormFile archivo, string? descripcion = null, bool establecerComoPerfil = true)
        {
            try
            {
                var resultado = await _servicioUpload.SubirImagen(archivo, descripcion);
                
                if (resultado.Correcto && establecerComoPerfil)
                {
                    await _servicioUpload.EstablecerImagenPerfilUsuario(resultado.Datos.Id, usuarioId);
                }
                
                return resultado;
            }
            catch (Exception ex)
            {
                return new DTO<Imagen>
                {
                    Correcto = false,
                    Mensaje = $"Error al subir imagen para usuario: {ex.Message}"
                };
            }
        }

        [HttpGet("obtener_por_id/{imagenId}")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<Imagen>> ObtenerPorId(Guid imagenId)
        {
            return await _imagenRepo.Obtener_por_id(new Imagen { Id = imagenId });
        }

        [HttpGet("obtener_todos")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<IEnumerable<Imagen>>> ObtenerTodas()
        {
            return await _imagenRepo.Obtener_todos();
        }

        [HttpPost("obtener_paginado")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<Items_pagina<Imagen>>> ObtenerPaginado([FromBody] Filtros_paginado filtros)
        {
            return await _imagenRepo.Obtener_paginado(filtros);
        }

        [HttpGet("obtener_por_inmueble/{inmuebleId}")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<IEnumerable<Imagen>>> ObtenerPorInmueble(int inmuebleId)
        {
            return await _servicioUpload.ObtenerImagenesInmueble(inmuebleId);
        }

        [HttpPut("actualizar/{imagenId}")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<Imagen>> Actualizar(Guid imagenId, [FromBody] Imagen imagen)
        {
            try
            {
                imagen.Id = imagenId; // Asegurar que el ID está correcto
                return await _servicioUpload.ActualizarImagen(imagen);
            }
            catch (Exception ex)
            {
                return new DTO<Imagen>
                {
                    Correcto = false,
                    Mensaje = $"Error al actualizar imagen: {ex.Message}"
                };
            }
        }

        [HttpPost("establecer-principal/{imagenId}/inmueble/{inmuebleId}")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<bool>> EstablecerComoPrincipal(Guid imagenId, int inmuebleId)
        {
            return await _servicioUpload.EstablecerImagenPrincipal(imagenId, inmuebleId);
        }

        [HttpPost("establecer-perfil/{imagenId}/usuario/{usuarioId}")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<bool>> EstablecerImagenPerfil(Guid imagenId, int usuarioId)
        {
            return await _servicioUpload.EstablecerImagenPerfilUsuario(imagenId, usuarioId);
        }

        [HttpDelete("eliminar/{imagenId}")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<bool>> Eliminar(Guid imagenId)
        {
            return await _servicioUpload.EliminarImagen(imagenId);
        }
    }
}