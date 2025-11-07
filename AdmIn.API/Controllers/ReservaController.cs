using AdmIn.Business.Servicios;
using AdmIn.Common;
using AdmIn.Common.Entidades;
using AdmIn.Common.Repositorios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Security.Claims;

namespace AdmIn.API.Controllers
{
 [ApiController]
 [Route("api/[controller]")]
 public class ReservaController : ControllerBase
 {
 private readonly IServ_Reserva _servReserva;
 private readonly IReservaRepository _reservaRepo;
 private readonly IInmuebleRepository _inmuebleRepo;
 private readonly IServ_Usuario _servUsuario;
 private readonly IServ_Rol _servRol;

 public ReservaController(IServ_Reserva servReserva, IReservaRepository reservaRepo, IInmuebleRepository inmuebleRepo, IServ_Usuario servUsuario, IServ_Rol servRol)
 {
 _servReserva = servReserva;
 _reservaRepo = reservaRepo;
 _inmuebleRepo = inmuebleRepo;
 _servUsuario = servUsuario;
 _servRol = servRol;
 }

 public record ReservaCreateRequest(int InmuebleId, int UsuarioReservadorId, decimal Costo, DateTime? FechaVencimiento, int? MonedaId);
 public record ReservaUpdateRequest(decimal? Costo, DateTime? FechaVencimiento, int? UsuarioModificadorId);
 public record UsuarioPorMailRequest(string Email, int InmuebleId);
 public record AsegurarInquilinoRequest(int UsuarioId, int InmuebleId);
 public record BuscarOCrearUsuarioRequest(string Email, string Nombre, int InmuebleId);

 // Helper: obtiene id de usuario desde claims
 private int? GetCurrentUserId()
 {
 var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("Id")?.Value;
 if (int.TryParse(idClaim, out var id)) return id;
 return null;
 }

 // Helper: verifica si el caller es admin o propietario del inmueble
 private async Task<bool> IsAdminOrPropietario(int inmuebleId)
 {
 var user = User;
 if (user == null) return false;

 if (user.IsInRole("admin_usuario") || user.IsInRole("admin_propiedad")) return true;

 var uid = GetCurrentUserId();
 if (!uid.HasValue) return false;

 var inmRes = await _inmuebleRepo.Obtener_por_id(new Inmueble { Id = inmuebleId });
 if (!inmRes.Correcto || inmRes.Datos == null) return false;

 return inmRes.Datos.PropietarioId.HasValue && inmRes.Datos.PropietarioId.Value == uid.Value;
 }

 // POST: api/reserva/usuario_por_mail
 [HttpPost("usuario_por_mail")]
 [Authorize]
 public async Task<IActionResult> ObtenerUsuarioPorMail([FromBody] UsuarioPorMailRequest req)
 {
 if (!await IsAdminOrPropietario(req.InmuebleId)) return Forbid();

 var userRes = await _servUsuario.Obtener_por_mail(req.Email);
 if (userRes == null || !userRes.Correcto || userRes.Datos == null)
 return NotFound(new { message = userRes?.Mensaje ?? "Usuario no encontrado" });

 return Ok(userRes.Datos);
 }

 // POST: api/reserva/asegurar_inquilino
 [HttpPost("asegurar_inquilino")]
 [Authorize]
 public async Task<IActionResult> AsegurarInquilino([FromBody] AsegurarInquilinoRequest req)
 {
 if (!await IsAdminOrPropietario(req.InmuebleId)) return Forbid();

 var usuarioRes = await _servUsuario.Obtener_por_id(new Usuario { Id = req.UsuarioId });
 if (usuarioRes == null || !usuarioRes.Correcto || usuarioRes.Datos == null)
 return NotFound(new { message = usuarioRes?.Mensaje ?? "Usuario no encontrado" });

 var usuario = usuarioRes.Datos;

 // Obtener rol "inquilino"
 var rolesRes = await _servRol.Obtener_todos();
 if (rolesRes == null || !rolesRes.Correcto || rolesRes.Datos == null)
 return BadRequest(new { message = "No se pudieron obtener roles" });

 Rol? rolInquilino = null;
 foreach (var r in rolesRes.Datos)
 {
 if (string.Equals(r.Nombre, "inquilino", StringComparison.OrdinalIgnoreCase))
 {
 rolInquilino = r;
 break;
 }
 }

 if (rolInquilino == null)
 {
 return BadRequest(new { message = "Rol 'inquilino' no encontrado en el sistema" });
 }

 // Añadir rol si no está ya
 usuario.Roles ??= new List<Rol>();
 if (!usuario.Roles.Any(x => x.Id == rolInquilino.Id))
 {
 usuario.Roles.Add(rolInquilino);
 var upd = await _servUsuario.Actualizar(usuario);
 if (upd == null || !upd.Correcto)
 return BadRequest(new { message = upd?.Mensaje ?? "No se pudo asignar rol inquilino" });

 return Ok(upd.Datos);
 }

 // Ya tenía el rol
 return Ok(usuario);
 }

 // POST: api/reserva/buscar_o_crear_usuario
 [HttpPost("buscar_o_crear_usuario")]
 [Authorize]
 public async Task<IActionResult> BuscarOCrearUsuario([FromBody] BuscarOCrearUsuarioRequest req)
 {
 if (!await IsAdminOrPropietario(req.InmuebleId)) return Forbid();

 // Intentar buscar por email
 var porMail = await _servUsuario.Obtener_por_mail(req.Email);
 if (porMail != null && porMail.Correcto && porMail.Datos != null)
 {
 return Ok(porMail.Datos);
 }

 // No existe: crear nuevo usuario con rol inquilino
 var nuevo = new Usuario
 {
 Nombre = string.IsNullOrWhiteSpace(req.Nombre) ? req.Email : req.Nombre,
 Email = req.Email,
 Password = Guid.NewGuid().ToString("N").Substring(0,8), // contraseña temporal
 Activo = true,
 FechaCreacion = DateTime.Now,
 FechaModificacion = DateTime.Now
 };

 // Obtener rol inquilino
 var rolesAll = await _servRol.Obtener_todos();
 Rol? rolInquilino = null;
 if (rolesAll != null && rolesAll.Correcto && rolesAll.Datos != null)
 {
 rolInquilino = rolesAll.Datos.FirstOrDefault(r => string.Equals(r.Nombre, "inquilino", StringComparison.OrdinalIgnoreCase));
 }

 if (rolInquilino != null)
 {
 nuevo.Roles = new List<Rol> { rolInquilino };
 }

 var creado = await _servUsuario.Crear(nuevo);
 if (creado == null || !creado.Correcto || creado.Datos == null)
 {
 return BadRequest(new { message = creado?.Mensaje ?? "Error al crear usuario" });
 }

 return Ok(creado.Datos);
 }

 // POST: api/reserva
 [HttpPost]
 [Authorize]
 public async Task<IActionResult> Crear([FromBody] ReservaCreateRequest req)
 {
 // Authorization: only admin or propietario of the inmueble
 if (!await IsAdminOrPropietario(req.InmuebleId))
 return Forbid();

 var res = await _servReserva.CrearReserva(req.InmuebleId, req.UsuarioReservadorId, req.Costo, req.FechaVencimiento, req.MonedaId, usuarioCreadorId: GetCurrentUserId() ??0);
 if (!res.Correcto)
 return BadRequest(new { message = res.Mensaje });

 // Return created resource
 return CreatedAtAction(nameof(ObtenerPorId), new { id = res.Datos.Id }, res.Datos);
 }

 // GET: api/reserva/{id}
 [HttpGet("{id}")]
 [Authorize]
 public async Task<IActionResult> ObtenerPorId(int id)
 {
 var res = await _reservaRepo.Obtener_por_id(id);
 if (!res.Correcto) return NotFound(new { message = res.Mensaje });

 // Only admin or propietario of the inmueble can view
 if (!await IsAdminOrPropietario(res.Datos.InmuebleId)) return Forbid();

 return Ok(res.Datos);
 }

 // GET: api/reserva/inmueble/{inmuebleId}
 [HttpGet("inmueble/{inmuebleId}")]
 [Authorize]
 public async Task<IActionResult> ObtenerActivasPorInmueble(int inmuebleId)
 {
 if (!await IsAdminOrPropietario(inmuebleId)) return Forbid();

 var res = await _reservaRepo.Obtener_activas_por_inmueble(inmuebleId);
 if (!res.Correcto) return BadRequest(new { message = res.Mensaje });
 return Ok(res.Datos);
 }

 // PUT: api/reserva/{id}
 [HttpPut("{id}")]
 [Authorize]
 public async Task<IActionResult> Actualizar(int id, [FromBody] ReservaUpdateRequest req)
 {
 var current = await _reservaRepo.Obtener_por_id(id);
 if (!current.Correcto || current.Datos == null)
 return NotFound(new { message = current.Mensaje });

 if (!await IsAdminOrPropietario(current.Datos.InmuebleId)) return Forbid();

 var reserva = current.Datos;
 if (req.Costo.HasValue) reserva.Costo = req.Costo.Value;
 if (req.FechaVencimiento.HasValue) reserva.FechaVencimiento = req.FechaVencimiento.Value;
 if (req.UsuarioModificadorId.HasValue) reserva.UsuarioModificadorId = req.UsuarioModificadorId.Value;
 reserva.FechaModificacion = DateTime.Now;

 var upd = await _reservaRepo.Actualizar(reserva);
 if (!upd.Correcto) return BadRequest(new { message = upd.Mensaje });
 return Ok(upd.Datos);
 }

 // POST: api/reserva/{id}/cancelar
 [HttpPost("{id}/cancelar")]
 [Authorize]
 public async Task<IActionResult> Cancelar(int id, [FromQuery] int? usuarioModificadorId)
 {
 var current = await _reservaRepo.Obtener_por_id(id);
 if (!current.Correcto || current.Datos == null)
 return NotFound(new { message = current.Mensaje });

 if (!await IsAdminOrPropietario(current.Datos.InmuebleId)) return Forbid();

 var reserva = current.Datos;
 reserva.Estado = ReservaEstado.Cancelada;
 if (usuarioModificadorId.HasValue) reserva.UsuarioModificadorId = usuarioModificadorId.Value;
 reserva.FechaModificacion = DateTime.Now;

 var upd = await _reservaRepo.Actualizar(reserva);
 if (!upd.Correcto) return BadRequest(new { message = upd.Mensaje });

 return Ok(upd.Datos);
 }

 // POST: api/reserva/{id}/convertir
 [HttpPost("{id}/convertir")]
 [Authorize]
 public async Task<IActionResult> ConvertirAContrato(int id, [FromQuery] bool descontarCostoAlContrato = false)
 {
 var current = await _reservaRepo.Obtener_por_id(id);
 if (!current.Correcto || current.Datos == null)
 return NotFound(new { message = current.Mensaje });

 if (!await IsAdminOrPropietario(current.Datos.InmuebleId)) return Forbid();

 // For conversion, client should supply contract parameters. For simplicity, use minimal contract and expect caller to update contract later.
 var contrato = new ContratoRenta
 {
 InmuebleId = current.Datos.InmuebleId,
 InquilinoId = current.Datos.UsuarioReservadorId,
 FechaCreacion = DateTime.Now,
 FechaModificacion = DateTime.Now,
 Estado = "Activo",
 MonedaId = current.Datos.MonedaId ??1
 };

 var res = await _servReserva.ConvertirAContrato(id, descontarCostoAlContrato, contrato, usuarioId: GetCurrentUserId() ??0);
 if (!res.Correcto) return BadRequest(new { message = res.Mensaje });
 return Ok(res.Datos);
 }
 }
}
