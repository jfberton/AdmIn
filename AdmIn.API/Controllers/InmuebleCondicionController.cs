using AdmIn.Business.Servicios;
using AdmIn.Common;
using AdmIn.Common.Entidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdmIn.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InmuebleCondicionController : ControllerBase
    {
        private readonly IServ_InmuebleCondicion _servCondicion;

        public InmuebleCondicionController(IServ_InmuebleCondicion servCondicion)
        {
            _servCondicion = servCondicion;
        }

        [HttpPost("crear")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<InmuebleCondicion>> Crear([FromBody] InmuebleCondicion condicion)
        {
            try
            {
                return await _servCondicion.Crear(condicion);
            }
            catch (Exception ex)
            {
                return new DTO<InmuebleCondicion>
                {
                    Correcto = false,
                    Mensaje = $"Error al crear condición: {ex.Message}"
                };
            }
        }

        [HttpPut("actualizar")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<InmuebleCondicion>> Actualizar([FromBody] InmuebleCondicion condicion)
        {
            try
            {
                return await _servCondicion.Actualizar(condicion);
            }
            catch (Exception ex)
            {
                return new DTO<InmuebleCondicion>
                {
                    Correcto = false,
                    Mensaje = $"Error al actualizar condición: {ex.Message}"
                };
            }
        }

        [HttpDelete("eliminar/{id}")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<bool>> Eliminar(int id)
        {
            try
            {
                return await _servCondicion.Eliminar(new InmuebleCondicion { Id = id });
            }
            catch (Exception ex)
            {
                return new DTO<bool>
                {
                    Correcto = false,
                    Mensaje = $"Error al eliminar condición: {ex.Message}"
                };
            }
        }

        [HttpGet("obtener_por_id/{id}")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<InmuebleCondicion>> ObtenerPorId(int id)
        {
            try
            {
                return await _servCondicion.Obtener_por_id(new InmuebleCondicion { Id = id });
            }
            catch (Exception ex)
            {
                return new DTO<InmuebleCondicion>
                {
                    Correcto = false,
                    Mensaje = $"Error al obtener condición: {ex.Message}"
                };
            }
        }

        [HttpGet("obtener_todos")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<IEnumerable<InmuebleCondicion>>> ObtenerTodos()
        {
            try
            {
                return await _servCondicion.Obtener_todos();
            }
            catch (Exception ex)
            {
                return new DTO<IEnumerable<InmuebleCondicion>>
                {
                    Correcto = false,
                    Mensaje = $"Error al obtener condiciones: {ex.Message}"
                };
            }
        }

        [HttpGet("obtener_activos")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<IEnumerable<InmuebleCondicion>>> ObtenerActivos()
        {
            try
            {
                return await _servCondicion.Obtener_activos();
            }
            catch (Exception ex)
            {
                return new DTO<IEnumerable<InmuebleCondicion>>
                {
                    Correcto = false,
                    Mensaje = $"Error al obtener condiciones activas: {ex.Message}"
                };
            }
        }

        [HttpGet("obtener_por_defecto")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<InmuebleCondicion>> ObtenerPorDefecto()
        {
            try
            {
                return await _servCondicion.Obtener_por_defecto();
            }
            catch (Exception ex)
            {
                return new DTO<InmuebleCondicion>
                {
                    Correcto = false,
                    Mensaje = $"Error al obtener condición por defecto: {ex.Message}"
                };
            }
        }

        [HttpGet("obtener_por_nombre/{nombre}")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<InmuebleCondicion>> ObtenerPorNombre(string nombre)
        {
            try
            {
                return await _servCondicion.Obtener_por_nombre(nombre);
            }
            catch (Exception ex)
            {
                return new DTO<InmuebleCondicion>
                {
                    Correcto = false,
                    Mensaje = $"Error al obtener condición por nombre: {ex.Message}"
                };
            }
        }

        [HttpPost("obtener_paginado")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<Items_pagina<InmuebleCondicion>>> ObtenerPaginado([FromBody] Filtros_paginado filtros)
        {
            try
            {
                return await _servCondicion.Obtener_paginado(filtros);
            }
            catch (Exception ex)
            {
                return new DTO<Items_pagina<InmuebleCondicion>>
                {
                    Correcto = false,
                    Mensaje = $"Error al obtener condiciones paginadas: {ex.Message}"
                };
            }
        }
    }
}