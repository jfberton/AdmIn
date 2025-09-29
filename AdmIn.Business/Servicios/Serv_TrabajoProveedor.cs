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
                // No bloqueamos, devolvemos error
                return new DTO<bool> { Correcto = false, Mensaje = "El usuario no está autorizado para aceptar este trabajo" };
            }

            // 2. Actualizar fecha inicio, costo aproximado y estado a 'En ejecución'
            trabajo.FechaInicio = request.FechaInicio;
            trabajo.CostoAproximado = request.CostoAproximado;
            trabajo.Estado = "En ejecución";

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
    }
}
