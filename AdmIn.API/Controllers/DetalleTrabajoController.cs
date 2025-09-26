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
    public class DetalleTrabajoController : ControllerBase
    {
        private readonly IServ_DetalleTrabajo _servicio;
        public DetalleTrabajoController(IServ_DetalleTrabajo servicio)
        {
            _servicio = servicio;
        }

        [HttpPost("crear")]
        [Authorize]
        public async Task<DTO<DetalleTrabajo>> Crear([FromBody] DetalleTrabajo detalle)
            => await _servicio.Crear(detalle);

        [HttpPut("actualizar")]
        [Authorize]
        public async Task<DTO<DetalleTrabajo>> Actualizar([FromBody] DetalleTrabajo detalle)
            => await _servicio.Actualizar(detalle);

        [HttpDelete("eliminar/{id}")]
        [Authorize]
        public async Task<DTO<bool>> Eliminar(int id)
            => await _servicio.Eliminar(new DetalleTrabajo { DetalleTrabajoId = id });

        [HttpGet("obtener_por_id/{id}")]
        [Authorize]
        public async Task<DTO<DetalleTrabajo>> ObtenerPorId(int id)
            => await _servicio.Obtener_por_id(id);

        [HttpGet("obtener_todos")]
        [Authorize]
        public async Task<DTO<IEnumerable<DetalleTrabajo>>> ObtenerTodos()
            => await _servicio.Obtener_todos();

        [HttpPost("obtener_paginado")]
        [Authorize]
        public async Task<DTO<Items_pagina<DetalleTrabajo>>> ObtenerPaginado([FromBody] Filtros_paginado filtros)
            => await _servicio.Obtener_paginado(filtros);
    }
}
