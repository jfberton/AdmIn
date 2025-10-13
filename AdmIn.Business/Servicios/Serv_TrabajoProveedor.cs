using AdmIn.Common;
using AdmIn.Common.Entidades;
using AdmIn.Common.Repositorios;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace AdmIn.Business.Servicios
{
    public class Serv_TrabajoProveedor : IServ_TrabajoProveedor
    {
        private readonly ITrabajoProveedorRepository _repo;
        private readonly IHistorialTrabajoRepository _repoHistorial;
        private readonly IServ_Notificacion _servNotificacion;
        private readonly IServ_Inmueble _servInmueble;
        private readonly IServ_InmuebleCondicion _servCondicion;
        private readonly IServ_Proveedor _servProveedor;
        private readonly IUsuarioRepository _usuarioRepo;
        private readonly ILogger<Serv_TrabajoProveedor> _logger;

        public Serv_TrabajoProveedor(ITrabajoProveedorRepository repo, IHistorialTrabajoRepository repoHistorial, IServ_Notificacion servNotificacion, IServ_Inmueble servInmueble, IServ_InmuebleCondicion servCondicion, IServ_Proveedor servProveedor, IUsuarioRepository usuarioRepo
            , ILogger<Serv_TrabajoProveedor> logger)
        {
            _repo = repo;
            _repoHistorial = repoHistorial;
            _servNotificacion = servNotificacion;
            _servInmueble = servInmueble;
            _servCondicion = servCondicion;
            _servProveedor = servProveedor;
            _usuarioRepo = usuarioRepo;
            _logger = logger;
        }

        private async Task<string?> GetUserNameByIdAsync(int? userId)
        {
            try
            {
                if (!userId.HasValue || userId.Value == 0) return null;
                var ures = await _usuarioRepo.Obtener_por_id(new Usuario { Id = userId.Value });
                if (ures != null && ures.Correcto && ures.Datos != null)
                    return ures.Datos.Nombre;
            }
            catch { /* no bloquear */ }
            return null;
        }

        private async Task<string?> GetProviderUserOrNameAsync(int? proveedorId)
        {
            try
            {
                if (!proveedorId.HasValue || proveedorId.Value == 0) return null;
                var provRes = await _servProveedor.Obtener_por_id(new Proveedor { Id = proveedorId.Value });
                if (provRes != null && provRes.Correcto && provRes.Datos != null)
                {
                    var prov = provRes.Datos;
                    if (prov.UsuarioId.HasValue)
                    {
                        var uname = await GetUserNameByIdAsync(prov.UsuarioId);
                        if (!string.IsNullOrWhiteSpace(uname)) return uname;
                    }
                    // Fallback a nombre del proveedor
                    if (!string.IsNullOrWhiteSpace(prov.Nombre)) return prov.Nombre;
                }
            }
            catch { /* no bloquear */ }
            return null;
        }

        public async Task<DTO<TrabajoProveedor>> Crear(TrabajoProveedor trabajo)
        {
            var resultado = await _repo.Crear(trabajo);

            if (resultado != null && resultado.Correcto && resultado.Datos != null)
            {
                // Crear historial 'Creado' y, si hay proveedor, crear historial 'Solicitado'
                try
                {
                    var creadorId = trabajo.UsuarioCreador ?? 0;
                    var creadorNombre = await GetUserNameByIdAsync(creadorId == 0 ? null : (int?)creadorId);

                    var historialCreado = new HistorialTrabajo
                    {
                        TrabajoProveedorId = resultado.Datos.Id,
                        Fecha = DateTime.Now,
                        Estado = TrabajoEstados.Creado,
                        UsuarioId = creadorId,
                        Comentario = string.IsNullOrWhiteSpace(creadorNombre) ? "Trabajo creado" : $"Creado por {creadorNombre}"
                    };

                    var h1 = await _repoHistorial.Crear(historialCreado);
                    // No bloquear si falla
                }
                catch { /* no bloquear si falla historial creado */ }

                if (trabajo.ProveedorId.HasValue && trabajo.ProveedorId.Value != 0)
                {
                    try
                    {
                        var solicitanteId = trabajo.UsuarioCreador ?? 0;
                        var proveedorNombre = await GetProviderUserOrNameAsync(trabajo.ProveedorId);

                        var historialSolicitado = new HistorialTrabajo
                        {
                            TrabajoProveedorId = resultado.Datos.Id,
                            Fecha = DateTime.Now,
                            Estado = TrabajoEstados.Solicitado,
                            UsuarioId = solicitanteId,
                            Comentario = string.IsNullOrWhiteSpace(proveedorNombre) ? $"Trabajo solicitado y asignado al proveedor {trabajo.ProveedorId.Value}" : $"Solicitado a {proveedorNombre}"
                        };

                        var h2 = await _repoHistorial.Crear(historialSolicitado);
                    }
                    catch { /* no bloquear si falla historial solicitado */ }
                }

                try
                {
                    // Intentar cambiar la condici�n del inmueble a 'En reparacion'
                    var condRes = await _servCondicion.Obtener_por_nombre("En reparacion");
                    if (condRes != null && condRes.Correcto && condRes.Datos != null)
                    {
                        var inmuebleDto = await _servInmueble.Obtener_por_id(new Inmueble { Id = trabajo.InmuebleId });
                        if (inmuebleDto != null && inmuebleDto.Correcto && inmuebleDto.Datos != null)
                        {
                            var inmueble = inmuebleDto.Datos;
                            inmueble.CondicionId = condRes.Datos.Id;
                            // Intentar actualizar sin bloquear la creaci�n del trabajo si falla
                            await _servInmueble.Actualizar(inmueble);
                        }
                    }
                }
                catch { /* No bloquear si falla */ }

                // Notificar al proveedor asignado (si tiene usuario asociado)
                try
                {
                    if (trabajo.ProveedorId.HasValue && trabajo.ProveedorId.Value != 0)
                    {
                        var provRes = await _servProveedor.Obtener_por_id(new Proveedor { Id = trabajo.ProveedorId.Value });
                        if (provRes != null && provRes.Correcto && provRes.Datos != null)
                        {
                            var proveedor = provRes.Datos;
                            if (proveedor.UsuarioId.HasValue)
                            {
                                var not = new Notificacion
                                {
                                    UsuarioId = proveedor.UsuarioId.Value,
                                    Tipo = "TrabajoAsignado",
                                    Mensaje = $"Se te ha asignado un nuevo trabajo (ID: {resultado.Datos.Id}) para el inmueble {trabajo.InmuebleId}.",
                                    Payload = System.Text.Json.JsonSerializer.Serialize(new { TrabajoId = resultado.Datos.Id })
                                };

                                await _servNotificacion.Crear(not);
                            }
                        }
                    }
                }
                catch { /* no bloquear si falla la notificaci�n */ }
            }

            return resultado;
        }

        public Task<DTO<TrabajoProveedor>> Actualizar(TrabajoProveedor trabajo) => _repo.Actualizar(trabajo);
        public Task<DTO<bool>> Eliminar(TrabajoProveedor trabajo) => _repo.Eliminar(trabajo);
        public Task<DTO<TrabajoProveedor>> Obtener_por_id(int trabajoId) => _repo.Obtener_por_id(new TrabajoProveedor { Id = trabajoId });
        public Task<DTO<IEnumerable<TrabajoProveedor>>> Obtener_todos() => _repo.Obtener_todos();
        public Task<DTO<Items_pagina<TrabajoProveedor>>> Obtener_paginado(Filtros_paginado filtros) => _repo.Obtener_paginado(filtros);

        public async Task<DTO<bool>> AceptarTrabajo(AceptarTrabajoRequest request)
        {
            // 1. Obtener trabajo
            var tDto = await _repo.Obtener_por_id(new TrabajoProveedor { Id = request.TrabajoId });
            if (!tDto.Correcto || tDto.Datos == null)
                return new DTO<bool> { Correcto = false, Mensaje = "Trabajo no encontrado" };

            var trabajo = tDto.Datos;

            // Validar que exista proveedor asignado
            if (!trabajo.ProveedorId.HasValue || trabajo.ProveedorId.Value == 0)
                return new DTO<bool> { Correcto = false, Mensaje = "No hay proveedor asignado para aceptar el trabajo" };

            // Validar que el usuario que acepta est� asociado al proveedor asignado
            var provRes = await _servProveedor.Obtener_por_id(new Proveedor { Id = trabajo.ProveedorId.Value });
            if (provRes == null || !provRes.Correcto || provRes.Datos == null)
            {
                return new DTO<bool> { Correcto = false, Mensaje = "Proveedor no encontrado" };
            }

            var proveedor = provRes.Datos;
            
            // Verificar que el usuario que acepta sea el usuario asociado al proveedor
            if (!proveedor.UsuarioId.HasValue || proveedor.UsuarioId.Value != request.UsuarioId)
            {
                return new DTO<bool> { Correcto = false, Mensaje = "No está autorizado para aceptar este trabajo. Solo el proveedor asignado puede aceptarlo." };
            }

            // 2. Actualizar fecha inicio, costo aproximado y estado a 'En ejecuci�n'
            trabajo.FechaInicio = request.FechaInicio;
            trabajo.CostoAproximado = request.CostoAproximado;
            trabajo.Estado = TrabajoEstados.EnEjecucion;

            var upd = await _repo.Actualizar(trabajo);
            if (!upd.Correcto)
                return new DTO<bool> { Correcto = false, Mensaje = "No se pudo actualizar el trabajo" };

            // 3. Generar historial
            var proveedorNombre = await GetProviderUserOrNameAsync(trabajo.ProveedorId);
            var comentarioHistorial = string.IsNullOrWhiteSpace(proveedorNombre) ? "Proveedor aceptó el trabajo y estableció fecha inicio y costo aproximado" : $"Trabajo aceptado por {proveedorNombre}";

            var historial = new HistorialTrabajo
            {
                TrabajoProveedorId = trabajo.Id,
                Fecha = DateTime.Now,
                Estado = trabajo.Estado,
                UsuarioId = request.UsuarioId,
                Comentario = comentarioHistorial
            };

            var h = await _repoHistorial.Crear(historial);

            if (!h.Correcto)
            {
                // Registramos pero no rollbackamos la actualizaci�n del trabajo
                return new DTO<bool> { Correcto = false, Mensaje = "Trabajo aceptado pero no se pudo guardar historial" };
            }

            // 4. Crear notificaci�n para el usuario (inquilino/administrador)
            var not = new Notificacion
            {
                UsuarioId = trabajo.UsuarioCreador ?? 0,
                Tipo = "TrabajoAceptado",
                Mensaje = $"El proveedor ha aceptado el trabajo solicitado para el inmueble {trabajo.InmuebleId}.",
                Payload = System.Text.Json.JsonSerializer.Serialize(new { TrabajoId = trabajo.Id })
            };

            await _servNotificacion.Crear(not);

            return new DTO<bool> { Correcto = true, Mensaje = "Trabajo aceptado correctamente" };
        }

        // New actions
        public async Task<DTO<bool>> SolicitarTrabajo(TrabajoAccionRequest request)
        {
            var tDto = await _repo.Obtener_por_id(new TrabajoProveedor { Id = request.TrabajoId });
            if (!tDto.Correcto || tDto.Datos == null)
                return new DTO<bool> { Correcto = false, Mensaje = "Trabajo no encontrado" };

            var trabajo = tDto.Datos;

            if (!trabajo.ProveedorId.HasValue || trabajo.ProveedorId.Value == 0)
                return new DTO<bool> { Correcto = false, Mensaje = "No hay proveedor asignado para solicitar." };

            // Sólo permitir solicitar si está en Creado o Rechazado
            if (trabajo.Estado != TrabajoEstados.Creado && trabajo.Estado != TrabajoEstados.Rechazado)
                return new DTO<bool> { Correcto = false, Mensaje = $"No se puede solicitar desde el estado actual: {trabajo.Estado}" };

            trabajo.Estado = TrabajoEstados.Solicitado;
            var upd = await _repo.Actualizar(trabajo);
            if (!upd.Correcto) return new DTO<bool> { Correcto = false, Mensaje = "No se pudo actualizar trabajo" };

            var historial = new HistorialTrabajo { TrabajoProveedorId = trabajo.Id, Fecha = DateTime.Now, Estado = TrabajoEstados.Solicitado, UsuarioId = request.UsuarioId, Comentario = request.Comentario };
            await _repoHistorial.Crear(historial);

            try
            {
                if (trabajo.ProveedorId.HasValue)
                {
                    var provRes = await _servProveedor.Obtener_por_id(new Proveedor { Id = trabajo.ProveedorId.Value });
                    if (provRes != null && provRes.Correcto && provRes.Datos != null && provRes.Datos.UsuarioId.HasValue)
                    {
                        await _servNotificacion.Crear(new Notificacion { UsuarioId = provRes.Datos.UsuarioId.Value, Tipo = "TrabajoSolicitado", Mensaje = $"Se te ha solicitado el trabajo ID {trabajo.Id}", Payload = System.Text.Json.JsonSerializer.Serialize(new { TrabajoId = trabajo.Id }) });
                    }
                }
            }
            catch { }

            return new DTO<bool> { Correcto = true, Datos = true, Mensaje = "Trabajo solicitado correctamente" };
        }

        public async Task<DTO<bool>> RechazarTrabajo(TrabajoAccionRequest request)
        {
            var tDto = await _repo.Obtener_por_id(new TrabajoProveedor { Id = request.TrabajoId });
            if (!tDto.Correcto || tDto.Datos == null)
                return new DTO<bool> { Correcto = false, Mensaje = "Trabajo no encontrado" };

            var trabajo = tDto.Datos;

            if (!trabajo.ProveedorId.HasValue || trabajo.ProveedorId.Value == 0)
                return new DTO<bool> { Correcto = false, Mensaje = "No hay proveedor asignado a este trabajo" };

            // Validar que el usuario que rechaza esté asociado al proveedor asignado
            var provRes = await _servProveedor.Obtener_por_id(new Proveedor { Id = trabajo.ProveedorId.Value });
            if (provRes == null || !provRes.Correcto || provRes.Datos == null)
            {
                return new DTO<bool> { Correcto = false, Mensaje = "Proveedor no encontrado" };
            }

            var proveedor = provRes.Datos;
            
            // Verificar que el usuario que rechaza sea el usuario asociado al proveedor
            if (!proveedor.UsuarioId.HasValue || proveedor.UsuarioId.Value != request.UsuarioId)
            {
                return new DTO<bool> { Correcto = false, Mensaje = "No está autorizado para rechazar este trabajo. Solo el proveedor asignado puede rechazarlo." };
            }

            if (trabajo.Estado != TrabajoEstados.Solicitado)
                return new DTO<bool> { Correcto = false, Mensaje = "Solo se puede rechazar cuando está en estado Solicitado" };

            var motivo = string.IsNullOrWhiteSpace(request.Comentario) ? "Proveedor rechazó la solicitud" : "Motivo: " + request.Comentario;

            // 1) Registrar en historial el rechazo con el motivo
            var historialRechazo = new HistorialTrabajo { TrabajoProveedorId = trabajo.Id, Fecha = DateTime.Now, Estado = TrabajoEstados.Rechazado, UsuarioId = request.UsuarioId, Comentario = motivo };
            await _repoHistorial.Crear(historialRechazo);

            // 2) Quitar asignación del proveedor y volver el trabajo a 'Creado'
            trabajo.ProveedorId = null; // remover proveedor asignado
            trabajo.FechaInicio = null;
            trabajo.CostoAproximado = null;
            trabajo.Estado = TrabajoEstados.Creado;

            DTO<TrabajoProveedor> upd = null;
            try
            {
                upd = await _repo.Actualizar(trabajo);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error ejecutando _repo.Actualizar en RechazarTrabajo para TrabajoId={TrabajoId}", trabajo.Id);
                return new DTO<bool> { Correcto = false, Mensaje = $"Excepción al actualizar trabajo: {ex.Message}" };
            }

            if (upd == null || !upd.Correcto)
            {
                var mensajeRepo = upd == null ? "Respuesta nula del repositorio al actualizar" : upd.Mensaje;
                _logger?.LogWarning("Actualizar fallo en RechazarTrabajo. TrabajoId={TrabajoId} MensajeRepo={MensajeRepo}", trabajo.Id, mensajeRepo);
                return new DTO<bool> { Correcto = false, Mensaje = $"No se pudo actualizar trabajo: {mensajeRepo}" };
            }

            // 3) Registrar en historial que el trabajo volvió a Creado y se removió el proveedor
            var historialCreado = new HistorialTrabajo { TrabajoProveedorId = trabajo.Id, Fecha = DateTime.Now, Estado = TrabajoEstados.Creado, UsuarioId = request.UsuarioId, Comentario = $"Vuelve a creado. Rechazado por proveedor." };
            await _repoHistorial.Crear(historialCreado);

            // 4) Notificar al creador/usuario responsable que el proveedor rechazó la solicitud
            try
            {
                var not = new Notificacion { UsuarioId = trabajo.UsuarioCreador ?? 0, Tipo = "TrabajoRechazadoPorProveedor", Mensaje = $"El proveedor asignado rechazó la solicitud del trabajo ID {trabajo.Id}. Motivo: {motivo}. El trabajo quedó sin proveedor asignado.", Payload = System.Text.Json.JsonSerializer.Serialize(new { TrabajoId = trabajo.Id }) };
                await _servNotificacion.Crear(not);
            }
            catch { }

            return new DTO<bool> { Correcto = true, Datos = true, Mensaje = "Solicitud rechazada; el trabajo volvió a estado 'Creado' y se desasignó el proveedor" };
        }

        public async Task<DTO<bool>> MarcarFinalizado(TrabajoAccionRequest request)
        {
            try
            {
                _logger?.LogInformation("MarcarFinalizado. TrabajoId={TrabajoId} UsuarioId={UsuarioId} Comentario={Comentario}", request.TrabajoId, request.UsuarioId, request.Comentario);
            }
            catch { }

            var tDto = await _repo.Obtener_por_id(new TrabajoProveedor { Id = request.TrabajoId });
            if (!tDto.Correcto || tDto.Datos == null)
                return new DTO<bool> { Correcto = false, Mensaje = "Trabajo no encontrado" };

            var trabajo = tDto.Datos;

            if (!trabajo.ProveedorId.HasValue || trabajo.ProveedorId.Value == 0)
                return new DTO<bool> { Correcto = false, Mensaje = "No hay proveedor asignado a este trabajo" };

            // Validar que el usuario que marca finalizado esté asociado al proveedor asignado
            var provRes = await _servProveedor.Obtener_por_id(new Proveedor { Id = trabajo.ProveedorId.Value });
            if (provRes == null || !provRes.Correcto || provRes.Datos == null)
            {
                return new DTO<bool> { Correcto = false, Mensaje = "Proveedor no encontrado" };
            }

            var proveedor = provRes.Datos;
            
            // Verificar que el usuario que marca finalizado sea el usuario asociado al proveedor
            if (!proveedor.UsuarioId.HasValue || proveedor.UsuarioId.Value != request.UsuarioId)
            {
                return new DTO<bool> { Correcto = false, Mensaje = "No está autorizado para marcar finalizado este trabajo. Solo el proveedor asignado puede hacerlo." };
            }

            if (trabajo.Estado != TrabajoEstados.EnEjecucion)
                return new DTO<bool> { Correcto = false, Mensaje = "Solo se puede marcar finalizado cuando está en ejecución" };

            trabajo.Estado = TrabajoEstados.FinalizadoPorAprobar;
            var upd = await _repo.Actualizar(trabajo);
            if (upd == null)
            {
                _logger?.LogError("Actualizar returned null for trabajo {TrabajoId}", trabajo.Id);
                return new DTO<bool> { Correcto = false, Mensaje = "No se pudo actualizar trabajo" };
            }
            if (!upd.Correcto)
            {
                _logger?.LogWarning("Actualizar falló para trabajo {TrabajoId}: {Mensaje}", trabajo.Id, upd.Mensaje);
                return new DTO<bool> { Correcto = false, Mensaje = upd.Mensaje ?? "No se pudo actualizar trabajo" };
            }

            var historial = new HistorialTrabajo { TrabajoProveedorId = trabajo.Id, Fecha = DateTime.Now, Estado = TrabajoEstados.FinalizadoPorAprobar, UsuarioId = request.UsuarioId, Comentario = request.Comentario };
            await _repoHistorial.Crear(historial);

            // Notificar al creador/usuario responsable
            try
            {
                var not = new Notificacion { UsuarioId = trabajo.UsuarioCreador ?? 0, Tipo = "TrabajoFinalizado", Mensaje = $"El proveedor ha marcado como finalizado el trabajo ID {trabajo.Id}", Payload = System.Text.Json.JsonSerializer.Serialize(new { TrabajoId = trabajo.Id }) };
                await _servNotificacion.Crear(not);
            }
            catch { }

            return new DTO<bool> { Correcto = true, Datos = true, Mensaje = "Trabajo marcado como finalizado y pendiente de aprobación" };
        }

        public async Task<DTO<bool>> RevisarFinalizacion(RevisarFinalizacionRequest request)
        {
            var tDto = await _repo.Obtener_por_id(new TrabajoProveedor { Id = request.TrabajoId });
            if (!tDto.Correcto || tDto.Datos == null)
                return new DTO<bool> { Correcto = false, Mensaje = "Trabajo no encontrado" };

            var trabajo = tDto.Datos;

            if (trabajo.Estado != TrabajoEstados.FinalizadoPorAprobar)
                return new DTO<bool> { Correcto = false, Mensaje = "No hay finalización pendiente de revisión" };

            if (request.Aprobado)
            {
                if (!string.IsNullOrEmpty(request.Comentario))
                {
                    trabajo.Estado = TrabajoEstados.FinalizacionAceptadaConObservaciones;
                }
                else
                {
                    trabajo.Estado = TrabajoEstados.Finalizado;
                }

                var upd = await _repo.Actualizar(trabajo);
                if (!upd.Correcto) return new DTO<bool> { Correcto = false, Mensaje = "No se pudo actualizar trabajo" };

                var historial = new HistorialTrabajo { TrabajoProveedorId = trabajo.Id, Fecha = DateTime.Now, Estado = trabajo.Estado, UsuarioId = request.UsuarioId, Comentario = request.Comentario };
                await _repoHistorial.Crear(historial);

                // Notificar proveedor
                try
                {
                    if (trabajo.ProveedorId.HasValue)
                    {
                        var provRes = await _servProveedor.Obtener_por_id(new Proveedor { Id = trabajo.ProveedorId.Value });
                        if (provRes != null && provRes.Correcto && provRes.Datos != null && provRes.Datos.UsuarioId.HasValue)
                        {
                            await _servNotificacion.Crear(new Notificacion { UsuarioId = provRes.Datos.UsuarioId.Value, Tipo = "FinalizacionAprobada", Mensaje = $"La finalización del trabajo ID {trabajo.Id} fue aprobada.", Payload = System.Text.Json.JsonSerializer.Serialize(new { TrabajoId = trabajo.Id }) });
                        }
                    }
                }
                catch { }

                return new DTO<bool> { Correcto = true, Datos = true, Mensaje = "Finalización aprobada" };
            }
            else
            {
                // Rechazado -> registrar y volver a En ejecución automáticamente
                var historialRechazo = new HistorialTrabajo { TrabajoProveedorId = trabajo.Id, Fecha = DateTime.Now, Estado = TrabajoEstados.FinalizacionRechazada, UsuarioId = request.UsuarioId, Comentario = request.Comentario };
                await _repoHistorial.Crear(historialRechazo);

                trabajo.Estado = TrabajoEstados.EnEjecucion;
                var upd = await _repo.Actualizar(trabajo);
                if (!upd.Correcto) return new DTO<bool> { Correcto = false, Mensaje = "No se pudo actualizar trabajo tras rechazo" };

                var historialResume = new HistorialTrabajo { TrabajoProveedorId = trabajo.Id, Fecha = DateTime.Now, Estado = TrabajoEstados.EnEjecucion, UsuarioId = request.UsuarioId, Comentario = "Reanudar trabajo tras rechazo de finalización" };
                await _repoHistorial.Crear(historialResume);

                return new DTO<bool> { Correcto = true, Datos = true, Mensaje = "Finalización rechazada; trabajo reanudado" };
            }
        }

        public async Task<DTO<bool>> CancelarTrabajo(TrabajoAccionRequest request)
        {
            var tDto = await _repo.Obtener_por_id(new TrabajoProveedor { Id = request.TrabajoId });
            if (!tDto.Correcto || tDto.Datos == null)
                return new DTO<bool> { Correcto = false, Mensaje = "Trabajo no encontrado" };

            var trabajo = tDto.Datos;

            // Permitimos cancelar desde cualquier estado excepto Finalizado
            if (trabajo.Estado == TrabajoEstados.Finalizado)
                return new DTO<bool> { Correcto = false, Mensaje = "No se puede cancelar un trabajo finalizado" };

            trabajo.Estado = TrabajoEstados.Cancelado;
            var upd = await _repo.Actualizar(trabajo);
            if (!upd.Correcto) return new DTO<bool> { Correcto = false, Mensaje = "No se pudo cancelar trabajo" };

            var historial = new HistorialTrabajo { TrabajoProveedorId = trabajo.Id, Fecha = DateTime.Now, Estado = TrabajoEstados.Cancelado, UsuarioId = request.UsuarioId, Comentario = request.Comentario };
            await _repoHistorial.Crear(historial);

            return new DTO<bool> { Correcto = true, Datos = true, Mensaje = "Trabajo cancelado" };
        }

        // Document management - pass-through to repository
        public Task<DTO<TrabajoProveedorDocumento>> CrearDocumento(TrabajoProveedorDocumento doc) => _repo.CrearDocumento(doc);
        public Task<DTO<TrabajoProveedorDocumento>> ObtenerDocumentoPorId(int documentoId) => _repo.ObtenerDocumentoPorId(documentoId);
        public Task<DTO<IEnumerable<TrabajoProveedorDocumento>>> ObtenerDocumentosPorTrabajo(int trabajoId) => _repo.ObtenerDocumentosPorTrabajo(trabajoId);
        public Task<DTO<bool>> EliminarDocumento(int documentoId) => _repo.EliminarDocumento(documentoId);
    }
}
