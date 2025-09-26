using AdmIn.Business.Servicios;
using AdmIn.Common;
using AdmIn.Common.Entidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdmIn.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalificacionProveedorController : ControllerBase
    {
        private readonly IServ_CalificacionProveedor _servicio;
        public CalificacionProveedorController(IServ_CalificacionProveedor servicio)
        {
            _servicio = servicio;
        }

        [HttpPost("crear")]
        [Authorize]
        public async Task<DTO<CalificacionProveedor>> Crear([FromBody] CalificacionProveedor calificacion)
            => await _servicio.Crear(calificacion);

        [HttpPut("actualizar")]
        [Authorize]
        public async Task<DTO<CalificacionProveedor>> Actualizar([FromBody] CalificacionProveedor calificacion)
            => await _servicio.Actualizar(calificacion);

        [HttpDelete("eliminar/{id}")]
        [Authorize]
        public async Task<DTO<bool>> Eliminar(int id)
            => await _servicio.Eliminar(new CalificacionProveedor { CalificacionProveedorId = id });

        [HttpGet("obtener_por_id/{id}")]
        [Authorize]
        public async Task<DTO<CalificacionProveedor>> ObtenerPorId(int id)
            => await _servicio.Obtener_por_id(id);

        [HttpGet("obtener_todos")]
        [Authorize]
        public async Task<DTO<IEnumerable<CalificacionProveedor>>> ObtenerTodos()
            => await _servicio.Obtener_todos();

        [HttpPost("obtener_paginado")]
        [Authorize]
        public async Task<DTO<Items_pagina<CalificacionProveedor>>> ObtenerPaginado([FromBody] Filtros_paginado filtros)
            => await _servicio.Obtener_paginado(filtros);
    }
}
