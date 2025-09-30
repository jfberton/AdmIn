using AdmIn.Common;
using AdmIn.Common.Entidades;
using AdmIn.Common.Repositorios;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

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

        public Serv_TrabajoProveedor(ITrabajoProveedorRepository repo, IHistorialTrabajoRepository repoHistorial, IServ_Notificacion servNotificacion, IServ_Inmueble servInmueble, IServ_InmuebleCondicion servCondicion, IServ_Proveedor servProveedor)
        {
            _repo = repo;
            _repoHistorial = repoHistorial;
            _servNotificacion = servNotificacion;
            _servInmueble = servInmueble;
            _servCondicion = servCondicion;
            _servProveedor = servProveedor;
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

                    var historialCreado = new HistorialTrabajo
                    {
                        TrabajoProveedorId = resultado.Datos.Id,
                        Fecha = DateTime.Now,
                        Estado = TrabajoEstados.Creado,
                        UsuarioId = creadorId,
                        Comentario = "Trabajo creado"
                    };

                    var h1 = await _repoHistorial.Crear(historialCreado);
                    // No bloquear si falla
                }
                catch { /* no bloquear si falla historial creado */ }

                if (trabajo.ProveedorId != 0)
                {
                    try
                    {
                        var solicitanteId = trabajo.UsuarioCreador ?? 0;
                        var historialSolicitado = new HistorialTrabajo
                        {
                            TrabajoProveedorId = resultado.Datos.Id,
                            Fecha = DateTime.Now,
                            Estado = TrabajoEstados.Solicitado,
                            UsuarioId = solicitanteId,
                            Comentario = $"Trabajo solicitado y asignado al proveedor {trabajo.ProveedorId}"
                        };

                        var h2 = await _repoHistorial.Crear(historialSolicitado);
                    }
                    catch { /* no bloquear si falla historial solicitado */ }
                }

                try
                {
                    // Intentar cambiar la condición del inmueble a 'En reparacion'
                    var condRes = await _servCondicion.Obtener_por_nombre("En reparacion");
                    if (condRes != null && condRes.Correcto && condRes.Datos != null)
                    {
                        var inmuebleDto = await _servInmueble.Obtener_por_id(new Inmueble { Id = trabajo.InmuebleId });
                        if (inmuebleDto != null && inmuebleDto.Correcto && inmuebleDto.Datos != null)
                        {
                            var inmueble = inmuebleDto.Datos;
                            inmueble.CondicionId = condRes.Datos.Id;
                            // Intentar actualizar sin bloquear la creación del trabajo si falla
                            await _servInmueble.Actualizar(inmueble);
                        }
                    }
                }
                catch { /* No bloquear si falla */ }

                // Notificar al proveedor asignado (si tiene usuario asociado)
                try
                {
                    var provRes = await _servProveedor.Obtener_por_id(new Proveedor { Id = trabajo.ProveedorId });
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
                catch { /* no bloquear si falla la notificación */ }
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

            // validar que el proveedor que acepta coincida
            if (trabajo.ProveedorId != request.UsuarioId)
            {
                // Asumimos UsuarioId es Id del proveedor asociado al Usuario; en modelos reales haría otra comprobación
                // pero dejar mensaje claro
                // No bloquear, devolvemos error
                return new DTO<bool> { Correcto = false, Mensaje = "El usuario no está autorizado para aceptar este trabajo" };
            }

            // 2. Actualizar fecha inicio, costo aproximado y estado a 'En ejecución'
            trabajo.FechaInicio = request.FechaInicio;
            trabajo.CostoAproximado = request.CostoAproximado;
            trabajo.Estado = TrabajoEstados.EnEjecucion;

            var upd = await _repo.Actualizar(trabajo);
            if (!upd.Correcto)
                return new DTO<bool> { Correcto = false, Mensaje = "No se pudo actualizar el trabajo" };

            // 3. Generar historial
            var historial = new HistorialTrabajo
            {
                TrabajoProveedorId = trabajo.Id,
                Fecha = DateTime.Now,
                Estado = trabajo.Estado,
                UsuarioId = request.UsuarioId,
                Comentario = "Proveedor aceptó el trabajo y estableció fecha inicio y costo aproximado"
            };

            var h = await _repoHistorial.Crear(historial);

            if (!h.Correcto)
            {
                // Registramos pero no rollbackamos la actualización del trabajo
                return new DTO<bool> { Correcto = false, Mensaje = "Trabajo aceptado pero no se pudo guardar historial" };
            }

            // 4. Crear notificación para el usuario (inquilino/administrador)
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

            if (trabajo.ProveedorId == 0)
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
                var provRes = await _servProveedor.Obtener_por_id(new Proveedor { Id = trabajo.ProveedorId });
                if (provRes != null && provRes.Correcto && provRes.Datos != null && provRes.Datos.UsuarioId.HasValue)
                {
                    await _servNotificacion.Crear(new Notificacion { UsuarioId = provRes.Datos.UsuarioId.Value, Tipo = "TrabajoSolicitado", Mensaje = $"Se te ha solicitado el trabajo ID {trabajo.Id}", Payload = System.Text.Json.JsonSerializer.Serialize(new { TrabajoId = trabajo.Id }) });
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

            // Solo el proveedor asignado puede rechazar
            if (trabajo.ProveedorId != request.UsuarioId)
                return new DTO<bool> { Correcto = false, Mensaje = "El usuario no está autorizado para rechazar este trabajo" };

            if (trabajo.Estado != TrabajoEstados.Solicitado)
                return new DTO<bool> { Correcto = false, Mensaje = "Solo se puede rechazar cuando está en estado Solicitado" };

            trabajo.Estado = TrabajoEstados.Rechazado;
            var upd = await _repo.Actualizar(trabajo);
            if (!upd.Correcto) return new DTO<bool> { Correcto = false, Mensaje = "No se pudo actualizar trabajo" };

            var historial = new HistorialTrabajo { TrabajoProveedorId = trabajo.Id, Fecha = DateTime.Now, Estado = TrabajoEstados.Rechazado, UsuarioId = request.UsuarioId, Comentario = request.Comentario };
            await _repoHistorial.Crear(historial);

            return new DTO<bool> { Correcto = true, Datos = true, Mensaje = "Trabajo rechazado" };
        }

        public async Task<DTO<bool>> MarcarFinalizado(TrabajoAccionRequest request)
        {
            var tDto = await _repo.Obtener_por_id(new TrabajoProveedor { Id = request.TrabajoId });
            if (!tDto.Correcto || tDto.Datos == null)
                return new DTO<bool> { Correcto = false, Mensaje = "Trabajo no encontrado" };

            var trabajo = tDto.Datos;

            // Solo proveedor puede marcar finalizado y debe estar en ejecución
            if (trabajo.ProveedorId != request.UsuarioId)
                return new DTO<bool> { Correcto = false, Mensaje = "El usuario no está autorizado para marcar finalizado" };

            if (trabajo.Estado != TrabajoEstados.EnEjecucion)
                return new DTO<bool> { Correcto = false, Mensaje = "Solo se puede marcar finalizado cuando está en ejecución" };

            trabajo.Estado = TrabajoEstados.FinalizadoPorAprobar;
            var upd = await _repo.Actualizar(trabajo);
            if (!upd.Correcto) return new DTO<bool> { Correcto = false, Mensaje = "No se pudo actualizar trabajo" };

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
                    var provRes = await _servProveedor.Obtener_por_id(new Proveedor { Id = trabajo.ProveedorId });
                    if (provRes != null && provRes.Correcto && provRes.Datos != null && provRes.Datos.UsuarioId.HasValue)
                    {
                        await _servNotificacion.Crear(new Notificacion { UsuarioId = provRes.Datos.UsuarioId.Value, Tipo = "FinalizacionAprobada", Mensaje = $"La finalización del trabajo ID {trabajo.Id} fue aprobada.", Payload = System.Text.Json.JsonSerializer.Serialize(new { TrabajoId = trabajo.Id }) });
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
    }
}
