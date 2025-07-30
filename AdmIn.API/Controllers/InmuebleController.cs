using AdmIn.Common;
using AdmIn.Common.Entidades;
using AdmIn.Business.Servicios;
using AdmIn.API.Utilitarios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdmIn.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InmuebleController : ControllerBase
    {
        private readonly IServ_Inmueble _servicio;

        public InmuebleController(IServ_Inmueble servicio)
        {
            _servicio = servicio;
        }

        [HttpPost("obtener_paginado")]
        [Authorize(Roles = "admin_propiedad")]
        public async Task<DTO<Items_pagina<Inmueble>>> Obtener_inmuebles(dynamic filtros_paginado)
        {
            try
            {
                var filtros = JsonHelper.Deserialize<Filtros_paginado>(filtros_paginado);
                return await _servicio.Obtener_paginado(filtros);
            }
            catch (Exception ex)
            {
                return new DTO<Items_pagina<Inmueble>>
                {
                    Correcto = false,
                    Mensaje = $"Error procesando los filtros: {ex.Message}"
                };
            }
        }

        [HttpGet("obtener_todos")]
        public async Task<DTO<IEnumerable<Inmueble>>> Obtener_todos()
        {
            return await _servicio.Obtener_todos();
        }

        [HttpGet("obtener_por_id/{inmuebleId}")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<Inmueble>> Obtener_por_id(int inmuebleId)
        {
            return await _servicio.Obtener_por_id(new Inmueble { Id = inmuebleId });
        }

        [HttpGet("obtener_estados")]
        public async Task<DTO<IEnumerable<string>>> Obtener_estados()
        {
            return await _servicio.Obtener_estados();
        }

        [HttpGet("obtener_por_estado/{estado}")]
        public async Task<DTO<IEnumerable<Inmueble>>> Obtener_por_estado(string estado)
        {
            return await _servicio.Obtener_por_estado(estado);
        }

        [HttpGet("obtener_por_administrador/{administradorId}")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<IEnumerable<Inmueble>>> Obtener_por_administrador(int administradorId)
        {
            return await _servicio.Obtener_por_administrador(administradorId);
        }

        [HttpGet("obtener_por_ubicacion")]
        public async Task<DTO<IEnumerable<Inmueble>>> Obtener_por_ubicacion(
            [FromQuery] string? pais, 
            [FromQuery] string? estado, 
            [FromQuery] string? ciudad)
        {
            return await _servicio.Obtener_por_ubicacion(pais, estado, ciudad);
        }

        [HttpGet("obtener_por_rango_precio")]
        public async Task<DTO<IEnumerable<Inmueble>>> Obtener_por_rango_precio(
            [FromQuery] decimal precioMin, 
            [FromQuery] decimal precioMax)
        {
            return await _servicio.Obtener_por_rango_precio(precioMin, precioMax);
        }

        [HttpPost("nuevo")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<Inmueble>> Crear_inmueble(dynamic inmueble)
        {
            try
            {
                var oInmueble = JsonHelper.Deserialize<Inmueble>(inmueble);
                return await _servicio.Crear(oInmueble);
            }
            catch (Exception ex)
            {
                return new DTO<Inmueble>
                {
                    Correcto = false,
                    Mensaje = $"Error al crear el inmueble: {ex.Message}"
                };
            }
        }

        [HttpPost("modificar")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<Inmueble>> Modificar_inmueble(dynamic inmueble)
        {
            try
            {
                var oInmueble = JsonHelper.Deserialize<Inmueble>(inmueble);
                return await _servicio.Actualizar(oInmueble);
            }
            catch (Exception ex)
            {
                return new DTO<Inmueble>
                {
                    Correcto = false,
                    Mensaje = $"Error al modificar el inmueble: {ex.Message}"
                };
            }
        }

        [HttpDelete("eliminar/{inmuebleId}")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<bool>> Eliminar_inmueble(int inmuebleId)
        {
            var inmueble = new Inmueble { Id = inmuebleId };
            return await _servicio.Eliminar(inmueble);
        }

        [HttpGet("{inmuebleId}/caracteristicas")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<IEnumerable<CaracteristicaInmueble>>> Obtener_caracteristicas(int inmuebleId)
        {
            return await _servicio.Obtener_caracteristicas(inmuebleId);
        }

        [HttpPost("{inmuebleId}/caracteristicas")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<CaracteristicaInmueble>> Agregar_caracteristica(int inmuebleId, dynamic caracteristica)
        {
            try
            {
                var oCaracteristica = JsonHelper.Deserialize<CaracteristicaInmueble>(caracteristica);
                return await _servicio.Agregar_caracteristica(inmuebleId, oCaracteristica);
            }
            catch (Exception ex)
            {
                return new DTO<CaracteristicaInmueble>
                {
                    Correcto = false,
                    Mensaje = $"Error al agregar la característica: {ex.Message}"
                };
            }
        }

        [HttpPut("caracteristicas/{caracteristicaId}")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<CaracteristicaInmueble>> Actualizar_caracteristica(int caracteristicaId, dynamic caracteristica)
        {
            try
            {
                var oCaracteristica = JsonHelper.Deserialize<CaracteristicaInmueble>(caracteristica);
                oCaracteristica.Id = caracteristicaId; // Asegurar que el ID esté correcto
                return await _servicio.Actualizar_caracteristica(oCaracteristica);
            }
            catch (Exception ex)
            {
                return new DTO<CaracteristicaInmueble>
                {
                    Correcto = false,
                    Mensaje = $"Error al actualizar la característica: {ex.Message}"
                };
            }
        }

        [HttpDelete("caracteristicas/{caracteristicaId}")]
        [Authorize(Roles = "admin_usuario")]
        public async Task<DTO<bool>> Eliminar_caracteristica(int caracteristicaId)
        {
            return await _servicio.Eliminar_caracteristica(caracteristicaId);
        }
    }
}