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
    }
}
