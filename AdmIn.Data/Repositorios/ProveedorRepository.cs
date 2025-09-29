using AdmIn.Common;
using AdmIn.Common.Entidades;
using AdmIn.Common.Repositorios;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AdmIn.Data.Repositorios
{
    public class ProveedorRepository : IProveedorRepository
    {
        public ProveedorRepository()
        {
        }

        public async Task<DTO<Proveedor>> Crear(Proveedor proveedor)
        {
            Console.WriteLine($"[REPOSITORY] ===== CREAR PROVEEDOR =====");
            Console.WriteLine($"[REPOSITORY] Datos del proveedor - Nombre: {proveedor.Nombre}, RFC: {proveedor.RFC}, Email: {proveedor.Email}");
            Console.WriteLine($"[REPOSITORY] Connection String: {InfoSQL.Conexion?.Substring(0, Math.Min(80, InfoSQL.Conexion?.Length ?? 0)) + "..."}");
            
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            using var transaccion = conexion.BeginTransaction();

            try
            {
                var sqlProveedor = @"INSERT INTO Proveedor (Nombre, RFC, Email, Telefono, Direccion, UsuarioId, Activo, FechaCreacion, FechaModificacion, UsuarioCreadorId, UsuarioModificadorId) 
                                   OUTPUT INSERTED.ProveedorID as Id, INSERTED.Nombre, INSERTED.RFC, INSERTED.Email, INSERTED.Telefono, INSERTED.Direccion, INSERTED.UsuarioId, INSERTED.Activo, INSERTED.FechaCreacion, INSERTED.FechaModificacion, INSERTED.UsuarioCreadorId, INSERTED.UsuarioModificadorId
                                   VALUES (@Nombre, @RFC, @Email, @Telefono, @Direccion, @UsuarioId, @Activo, @FechaCreacion, @FechaModificacion, @UsuarioCreadorId, @UsuarioModificadorId);";

                Console.WriteLine($"[REPOSITORY] Ejecutando SQL INSERT para proveedor...");
                
                var proveedorCreado = await conexion.QuerySingleOrDefaultAsync<Proveedor>(sqlProveedor, new
                {
                    proveedor.Nombre,
                    proveedor.RFC,
                    proveedor.Email,
                    proveedor.Telefono,
                    proveedor.Direccion,
                    proveedor.UsuarioId,
                    proveedor.Activo,
                    proveedor.FechaCreacion,
                    proveedor.FechaModificacion,
                    proveedor.UsuarioCreadorId,
                    proveedor.UsuarioModificadorId
                }, transaccion);

                if (proveedorCreado == null)
                    throw new Exception("No se pudo crear el proveedor.");

                Console.WriteLine($"[REPOSITORY] Proveedor creado con ID: {proveedorCreado.Id}");

                // Asignar rol proveedor (RolID = 5) al usuario asociado
                if (proveedor.UsuarioId.HasValue)
                {
                    Console.WriteLine($"[REPOSITORY] Asignando rol proveedor (ID=5) al usuario {proveedor.UsuarioId.Value}");
                    var sqlAsignarRol = @"INSERT INTO UsuarioRol (UsuarioID, RolID) VALUES (@UsuarioID, @RolID);";
                    await conexion.ExecuteAsync(sqlAsignarRol, new { UsuarioID = proveedor.UsuarioId.Value, RolID = 5 }, transaccion);
                    Console.WriteLine("[REPOSITORY] Rol asignado exitosamente");
                }

                transaccion.Commit();
                Console.WriteLine("[REPOSITORY] Transacción confirmada");

                return new DTO<Proveedor>
                {
                    Correcto = true,
                    Datos = proveedorCreado,
                    Mensaje = "Proveedor creado correctamente con rol de proveedor asignado."
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[REPOSITORY] ERROR en Crear: {ex.Message}");
                Console.WriteLine($"[REPOSITORY] StackTrace: {ex.StackTrace}");
                
                transaccion.Rollback();
                return new DTO<Proveedor>
                {
                    Correcto = false,
                    Mensaje = $"Error al crear proveedor: {ex.Message}"
                };
            }
        }

        public async Task<DTO<Proveedor>> Actualizar(Proveedor proveedor)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            using var transaccion = conexion.BeginTransaction();

            try
            {
                var sqlActualizarProveedor = @"UPDATE Proveedor
                                             SET Nombre = @Nombre,
                                                 RFC = @RFC,
                                                 Email = @Email,
                                                 Telefono = @Telefono,
                                                 Direccion = @Direccion,
                                                 UsuarioId = @UsuarioId,
                                                 Activo = @Activo,
                                                 FechaModificacion = @FechaModificacion,
                                                 UsuarioModificadorId = @UsuarioModificadorId
                                             OUTPUT INSERTED.ProveedorID as Id, INSERTED.Nombre, INSERTED.RFC, INSERTED.Email, INSERTED.Telefono, INSERTED.Direccion, INSERTED.UsuarioId, INSERTED.Activo, INSERTED.FechaCreacion, INSERTED.FechaModificacion, INSERTED.UsuarioCreadorId, INSERTED.UsuarioModificadorId
                                             WHERE ProveedorID = @Id;";

                var proveedorActualizado = await conexion.QuerySingleOrDefaultAsync<Proveedor>(sqlActualizarProveedor, new
                {
                    proveedor.Id,
                    proveedor.Nombre,
                    proveedor.RFC,
                    proveedor.Email,
                    proveedor.Telefono,
                    proveedor.Direccion,
                    proveedor.UsuarioId,
                    proveedor.Activo,
                    proveedor.FechaModificacion,
                    proveedor.UsuarioModificadorId
                }, transaccion);

                if (proveedorActualizado == null)
                    throw new Exception("No se pudo actualizar el proveedor.");

                transaccion.Commit();

                return new DTO<Proveedor>
                {
                    Correcto = true,
                    Datos = proveedorActualizado,
                    Mensaje = "Proveedor actualizado correctamente."
                };
            }
            catch (Exception ex)
            {
                transaccion.Rollback();
                return new DTO<Proveedor>
                {
                    Correcto = false,
                    Mensaje = $"Error al actualizar proveedor: {ex.Message}"
                };
            }
        }

        public async Task<DTO<bool>> Eliminar(Proveedor proveedor)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            using var transaccion = conexion.BeginTransaction();

            try
            {
                // Primero verificar si hay trabajos asociados
                var sqlVerificarTrabajos = @"SELECT COUNT(*) FROM TrabajoProveedor WHERE ProveedorId = @Id;";
                var cantidadTrabajos = await conexion.QuerySingleAsync<int>(sqlVerificarTrabajos, new { proveedor.Id }, transaccion);

                if (cantidadTrabajos > 0)
                {
                    // En lugar de eliminar físicamente, desactivar
                    var sqlDesactivar = @"UPDATE Proveedor SET Activo = 0 WHERE ProveedorID = @Id;";
                    var filasAfectadas = await conexion.ExecuteAsync(sqlDesactivar, new { proveedor.Id }, transaccion);
                    
                    transaccion.Commit();
                    
                    return new DTO<bool>
                    {
                        Correcto = filasAfectadas > 0,
                        Datos = filasAfectadas > 0,
                        Mensaje = filasAfectadas > 0 ? "Proveedor desactivado correctamente (tiene trabajos asociados)." : "No se encontró el proveedor."
                    };
                }

                // Si no hay trabajos, eliminar físicamente
                var sqlEliminarProveedor = @"DELETE FROM Proveedor WHERE ProveedorID = @Id;";
                var filasEliminadas = await conexion.ExecuteAsync(sqlEliminarProveedor, new { proveedor.Id }, transaccion);

                transaccion.Commit();

                return new DTO<bool>
                {
                    Correcto = filasEliminadas > 0,
                    Datos = filasEliminadas > 0,
                    Mensaje = filasEliminadas > 0 ? "Proveedor eliminado correctamente." : "No se encontró el proveedor."
                };
            }
            catch (Exception ex)
            {
                transaccion.Rollback();
                return new DTO<bool>
                {
                    Correcto = false,
                    Mensaje = $"Error al eliminar proveedor: {ex.Message}"
                };
            }
        }

        public async Task<DTO<Proveedor>> Obtener_por_id(Proveedor proveedor)
        {
            Console.WriteLine($"[REPOSITORY] ===== OBTENER POR ID: {proveedor.Id} =====");
            Console.WriteLine($"[REPOSITORY] Connection String: {InfoSQL.Conexion?.Substring(0, Math.Min(80, InfoSQL.Conexion?.Length ?? 0)) + "..."}");
            
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            
            try
            {
                Console.WriteLine("[REPOSITORY] Abriendo conexión a base de datos...");
                await conexion.OpenAsync();
                Console.WriteLine("[REPOSITORY] Conexión abierta exitosamente");

                var sqlProveedor = @"SELECT p.ProveedorID as Id, p.Nombre, p.RFC, p.Email, p.Telefono, p.Direccion, p.UsuarioId, p.Activo, p.FechaCreacion, p.FechaModificacion, p.UsuarioCreadorId, p.UsuarioModificadorId,
                                       ISNULL(a.CalificacionPromedio, 0) as CalificacionPromedio,
                                       ISNULL(a.TrabajosRealizados, 0) as TrabajosRealizados
                                   FROM Proveedor p
                                   LEFT JOIN (
                                       SELECT ProveedorId, AVG(CAST(Valor AS DECIMAL(18,2))) AS CalificacionPromedio, COUNT(*) AS TrabajosRealizados
                                       FROM CalificacionProveedor
                                       GROUP BY ProveedorId
                                   ) a ON a.ProveedorId = p.ProveedorID
                                   WHERE p.ProveedorID = @Id;";

                Console.WriteLine($"[REPOSITORY] Ejecutando query: {sqlProveedor}");
                Console.WriteLine($"[REPOSITORY] Parámetro ID: {proveedor.Id}");

                var proveedorEncontrado = await conexion.QuerySingleOrDefaultAsync<Proveedor>(sqlProveedor, new { proveedor.Id });
                
                if (proveedorEncontrado == null)
                {
                    Console.WriteLine("[REPOSITORY] No se encontró el proveedor");
                    return new DTO<Proveedor> { Correcto = false, Mensaje = "Proveedor no encontrado" };
                }

                Console.WriteLine($"[REPOSITORY] Proveedor encontrado: {proveedorEncontrado.Nombre} (ID: {proveedorEncontrado.Id})");

                return new DTO<Proveedor>
                {
                    Correcto = true,
                    Datos = proveedorEncontrado,
                    Mensaje = "Proveedor obtenido correctamente"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[REPOSITORY] ERROR en Obtener_por_id: {ex.Message}");
                Console.WriteLine($"[REPOSITORY] StackTrace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<DTO<IEnumerable<Proveedor>>> Obtener_todos()
        {
            Console.WriteLine($"[REPOSITORY] ===== OBTENER TODOS =====");
            Console.WriteLine($"[REPOSITORY] Connection String: {InfoSQL.Conexion?.Substring(0, Math.Min(80, InfoSQL.Conexion?.Length ?? 0)) + "..."}");
            
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            
            try
            {
                Console.WriteLine("[REPOSITORY] Abriendo conexión a base de datos...");
                await conexion.OpenAsync();
                Console.WriteLine("[REPOSITORY] Conexión abierta exitosamente");

                var sqlProveedores = @"SELECT p.ProveedorID as Id, p.Nombre, p.RFC, p.Email, p.Telefono, p.Direccion, p.UsuarioId, p.Activo, p.FechaCreacion, p.FechaModificacion, p.UsuarioCreadorId, p.UsuarioModificadorId,
                                           ISNULL(a.CalificacionPromedio, 0) as CalificacionPromedio,
                                           ISNULL(a.TrabajosRealizados, 0) as TrabajosRealizados
                                     FROM Proveedor p
                                     LEFT JOIN (
                                         SELECT ProveedorId, AVG(CAST(Valor AS DECIMAL(18,2))) AS CalificacionPromedio, COUNT(*) AS TrabajosRealizados
                                         FROM CalificacionProveedor
                                         GROUP BY ProveedorId
                                     ) a ON a.ProveedorId = p.ProveedorID
                                     ORDER BY p.Nombre;";

                Console.WriteLine($"[REPOSITORY] Ejecutando query: {sqlProveedores}");

                var proveedores = (await conexion.QueryAsync<Proveedor>(sqlProveedores)).ToList();

                Console.WriteLine($"[REPOSITORY] Query ejecutada exitosamente. Proveedores encontrados: {proveedores.Count}");
                
                if (proveedores.Any())
                {
                    foreach (var prov in proveedores.Take(3)) // Solo los primeros 3 para no saturar los logs
                    {
                        Console.WriteLine($"[REPOSITORY] Proveedor: ID={prov.Id}, Nombre={prov.Nombre}, RFC={prov.RFC}, Activo={prov.Activo}");
                    }
                    if (proveedores.Count > 3)
                    {
                        Console.WriteLine($"[REPOSITORY] ...y {proveedores.Count - 3} proveedores más");
                    }
                }

                return new DTO<IEnumerable<Proveedor>>
                {
                    Correcto = true,
                    Datos = proveedores,
                    Mensaje = "Proveedores obtenidos correctamente"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[REPOSITORY] ERROR en Obtener_todos: {ex.Message}");
                Console.WriteLine($"[REPOSITORY] StackTrace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<DTO<Items_pagina<Proveedor>>> Obtener_paginado(Filtros_paginado filtros)
        {
            Console.WriteLine($"[REPOSITORY] ===== OBTENER PAGINADO =====");
            Console.WriteLine($"[REPOSITORY] Filtros - Skip: {filtros.Skip}, Top: {filtros.Top}, Filter: {filtros.Filter ?? "null"}, OrderBy: {filtros.OrderBy ?? "null"}");
            Console.WriteLine($"[REPOSITORY] Connection String: {InfoSQL.Conexion?.Substring(0, Math.Min(80, InfoSQL.Conexion?.Length ?? 0)) + "..."}");
            
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            
            try
            {
                Console.WriteLine("[REPOSITORY] Abriendo conexión a base de datos...");
                await conexion.OpenAsync();
                Console.WriteLine("[REPOSITORY] Conexión abierta exitosamente");

                var sql = @"SELECT 
                                COUNT(*) OVER() AS TotalItems,
                                p.ProveedorID as Id, p.Nombre, p.RFC, p.Email, p.Telefono, p.Direccion, p.UsuarioId, p.Activo, p.FechaCreacion, p.FechaModificacion, p.UsuarioCreadorId, p.UsuarioModificadorId,
                                ISNULL(a.CalificacionPromedio, 0) as CalificacionPromedio,
                                ISNULL(a.TrabajosRealizados, 0) as TrabajosRealizados
                            FROM Proveedor p
                            LEFT JOIN (
                                SELECT ProveedorId, AVG(CAST(Valor AS DECIMAL(18,2))) AS CalificacionPromedio, COUNT(*) AS TrabajosRealizados
                                FROM CalificacionProveedor
                                GROUP BY ProveedorId
                            ) a ON a.ProveedorId = p.ProveedorID
                            WHERE (@FiltroBusqueda IS NULL OR p.Nombre LIKE '%' + @FiltroBusqueda + '%' OR p.RFC LIKE '%' + @FiltroBusqueda + '%' OR p.Email LIKE '%' + @FiltroBusqueda + '%')
                            ORDER BY 
                                CASE WHEN @OrdenarPor = 'Nombre' THEN p.Nombre END,
                                CASE WHEN @OrdenarPor = 'RFC' THEN p.RFC END,
                                CASE WHEN @OrdenarPor = 'Email' THEN p.Email END,
                                CASE WHEN @OrdenarPor IS NULL THEN p.Nombre END
                            OFFSET @Skip ROWS FETCH NEXT @Top ROWS ONLY;";

                Console.WriteLine($"[REPOSITORY] Ejecutando query paginado...");
                Console.WriteLine($"[REPOSITORY] Parámetros - FiltroBusqueda: {filtros.Filter ?? "NULL"}, OrdenarPor: {filtros.OrderBy ?? "NULL"}, Skip: {filtros.Skip}, Top: {filtros.Top}");

                var lista = await conexion.QueryAsync<dynamic>(sql, new
                {
                    FiltroBusqueda = (object?)filtros.Filter ?? DBNull.Value,
                    OrdenarPor = (object?)filtros.OrderBy ?? "Nombre",
                    Skip = filtros.Skip < 0 ? 0 : filtros.Skip,
                    Top = filtros.Top <= 0 ? 10 : filtros.Top
                });

                var proveedores = new List<Proveedor>();
                int totalItems = 0;

                Console.WriteLine($"[REPOSITORY] Query ejecutada. Resultados obtenidos: {lista.Count()}");

                foreach (var row in lista)
                {
                    totalItems = row.TotalItems;

                    proveedores.Add(new Proveedor
                    {
                        Id = row.Id,
                        Nombre = row.Nombre,
                        RFC = row.RFC,
                        Email = row.Email,
                        Telefono = row.Telefono,
                        Direccion = row.Direccion,
                        UsuarioId = row.UsuarioId,
                        Activo = row.Activo,
                        FechaCreacion = row.FechaCreacion,
                        FechaModificacion = row.FechaModificacion,
                        UsuarioCreadorId = row.UsuarioCreadorId,
                        UsuarioModificadorId = row.UsuarioModificadorId,
                        CalificacionPromedio = row.CalificacionPromedio,
                        TrabajosRealizados = row.TrabajosRealizados
                    });
                }

                Console.WriteLine($"[REPOSITORY] Procesados {proveedores.Count} proveedores de un total de {totalItems}");

                if (proveedores.Any())
                {
                    foreach (var prov in proveedores.Take(3))
                    {
                        Console.WriteLine($"[REPOSITORY] Proveedor paginado: ID={prov.Id}, Nombre={prov.Nombre}, RFC={prov.RFC}, Activo={prov.Activo}");
                    }
                    if (proveedores.Count > 3)
                    {
                        Console.WriteLine($"[REPOSITORY] ...y {proveedores.Count - 3} proveedores más en esta página");
                    }
                }

                return new DTO<Items_pagina<Proveedor>>
                {
                    Correcto = true,
                    Datos = new Items_pagina<Proveedor>
                    {
                        Total_items = totalItems,
                        Items = proveedores
                    },
                    Mensaje = "Proveedores paginados correctamente."
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[REPOSITORY] ERROR en Obtener_paginado: {ex.Message}");
                Console.WriteLine($"[REPOSITORY] StackTrace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<DTO<IEnumerable<Proveedor>>> Obtener_activos()
        {
            Console.WriteLine($"[REPOSITORY] ===== OBTENER ACTIVOS =====");
            Console.WriteLine($"[REPOSITORY] Connection String: {InfoSQL.Conexion?.Substring(0, Math.Min(80, InfoSQL.Conexion?.Length ?? 0)) + "..."}");
            
            using var conexion = new SqlConnection(InfoSQL.Conexion);

            try
            {
                Console.WriteLine("[REPOSITORY] Abriendo conexión a base de datos...");
                await conexion.OpenAsync();
                Console.WriteLine("[REPOSITORY] Conexión abierta exitosamente");

                var sqlProveedores = @"SELECT p.ProveedorID as Id, p.Nombre, p.RFC, p.Email, p.Telefono, p.Direccion, p.UsuarioId, p.Activo, p.FechaCreacion, p.FechaModificacion, p.UsuarioCreadorId, p.UsuarioModificadorId,
                                           ISNULL(a.CalificacionPromedio, 0) as CalificacionPromedio,
                                           ISNULL(a.TrabajosRealizados, 0) as TrabajosRealizados
                                     FROM Proveedor p
                                     LEFT JOIN (
                                         SELECT ProveedorId, AVG(CAST(Valor AS DECIMAL(18,2))) AS CalificacionPromedio, COUNT(*) AS TrabajosRealizados
                                         FROM CalificacionProveedor
                                         GROUP BY ProveedorId
                                     ) a ON a.ProveedorId = p.ProveedorID
                                     WHERE p.Activo = 1
                                     ORDER BY p.Nombre;";

                Console.WriteLine($"[REPOSITORY] Ejecutando query: {sqlProveedores}");

                var proveedores = await conexion.QueryAsync<Proveedor>(sqlProveedores);

                Console.WriteLine($"[REPOSITORY] Query ejecutada exitosamente. Proveedores activos encontrados: {proveedores.Count()}");

                if (proveedores.Any())
                {
                    foreach (var prov in proveedores.Take(3)) // Solo los primeros 3 para no saturar los logs
                    {
                        Console.WriteLine($"[REPOSITORY] Proveedor activo: ID={prov.Id}, Nombre={prov.Nombre}, RFC={prov.RFC}, Activo={prov.Activo}");
                    }
                    if (proveedores.Count() > 3)
                    {
                        Console.WriteLine($"[REPOSITORY] ...y {proveedores.Count() - 3} proveedores activos más");
                    }
                }

                return new DTO<IEnumerable<Proveedor>>
                {
                    Correcto = true,
                    Datos = proveedores.ToList(),
                    Mensaje = "Proveedores activos obtenidos correctamente"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[REPOSITORY] ERROR en Obtener_activos: {ex.Message}");
                Console.WriteLine($"[REPOSITORY] StackTrace: {ex.StackTrace}");
                Console.WriteLine($"[REPOSITORY] InnerException: {ex.InnerException?.Message}");
                
                if (ex is SqlException sqlEx)
                {
                    Console.WriteLine($"[REPOSITORY] SQL Error Number: {sqlEx.Number}");
                    Console.WriteLine($"[REPOSITORY] SQL Error Severity: {sqlEx.Class}");
                    Console.WriteLine($"[REPOSITORY] SQL Error State: {sqlEx.State}");
                    Console.WriteLine($"[REPOSITORY] SQL Error Procedure: {sqlEx.Procedure}");
                    Console.WriteLine($"[REPOSITORY] SQL Error Line: {sqlEx.LineNumber}");
                    Console.WriteLine($"[REPOSITORY] SQL Server: {sqlEx.Server}");
                }
                
                throw;
            }
        }

        public async Task<DTO<Proveedor>> Obtener_por_rfc(string rfc)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();

            var sqlProveedor = @"SELECT ProveedorID as Id, Nombre, RFC, Email, Telefono, Direccion, UsuarioId, Activo, FechaCreacion, FechaModificacion, UsuarioCreadorId, UsuarioModificadorId 
                               FROM Proveedor WHERE RFC = @RFC;";

            var proveedor = await conexion.QuerySingleOrDefaultAsync<Proveedor>(sqlProveedor, new { RFC = rfc });

            return new DTO<Proveedor>
            {
                Correcto = proveedor != null,
                Datos = proveedor,
                Mensaje = proveedor != null ? "Proveedor encontrado" : "Proveedor no encontrado"
            };
        }

        public async Task<DTO<Proveedor>> Obtener_por_email(string email)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();

            var sqlProveedor = @"SELECT ProveedorID as Id, Nombre, RFC, Email, Telefono, Direccion, UsuarioId, Activo, FechaCreacion, FechaModificacion, UsuarioCreadorId, UsuarioModificadorId 
                               FROM Proveedor WHERE Email = @Email;";

            var proveedor = await conexion.QuerySingleOrDefaultAsync<Proveedor>(sqlProveedor, new { Email = email });

            return new DTO<Proveedor>
            {
                Correcto = proveedor != null,
                Datos = proveedor,
                Mensaje = proveedor != null ? "Proveedor encontrado" : "Proveedor no encontrado"
            };
        }

        // Métodos para gestión de servicios de proveedor
        public async Task<DTO<IEnumerable<TipoServicio>>> Obtener_servicios_proveedor(int proveedorId)
        {
            Console.WriteLine($"[REPOSITORY] ===== OBTENER SERVICIOS PROVEEDOR: {proveedorId} =====");
            Console.WriteLine($"[REPOSITORY] Connection String: {InfoSQL.Conexion?.Substring(0, Math.Min(80, InfoSQL.Conexion?.Length ?? 0)) + "..."}");
            
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();

            var sql = @"SELECT 
                            ts.TipoServicioID as Id, 
                            ts.Nombre, 
                            ts.Descripcion
                        FROM ServicioProveedor sp
                        INNER JOIN TipoServicio ts ON sp.ServicioId = ts.TipoServicioID
                        WHERE sp.ProveedorId = @ProveedorId 
                          AND sp.Activo = 1
                        ORDER BY ts.Nombre;";

            try
            {
                Console.WriteLine($"[REPOSITORY] Ejecutando query para obtener servicios del proveedor {proveedorId}");
                Console.WriteLine($"[REPOSITORY] SQL: {sql}");
                
                var servicios = await conexion.QueryAsync<TipoServicio>(sql, new { ProveedorId = proveedorId });
                var serviciosList = servicios.ToList();
                
                Console.WriteLine($"[REPOSITORY] Query ejecutada. Servicios encontrados: {serviciosList.Count}");
                
                if (serviciosList.Any())
                {
                    foreach (var servicio in serviciosList)
                    {
                        Console.WriteLine($"[REPOSITORY] Servicio encontrado: ID={servicio.Id}, Nombre={servicio.Nombre}");
                    }
                }
                else
                {
                    Console.WriteLine($"[REPOSITORY] No se encontraron servicios para el proveedor {proveedorId}");
                    
                    // Verificar si existe el proveedor
                    var sqlVerificarProveedor = "SELECT COUNT(*) FROM Proveedor WHERE ProveedorID = @ProveedorId";
                    var proveedorExiste = await conexion.QuerySingleAsync<int>(sqlVerificarProveedor, new { ProveedorId = proveedorId });
                    Console.WriteLine($"[REPOSITORY] ¿Existe el proveedor? {proveedorExiste > 0}");
                    
                    // Verificar si hay registros en ServicioProveedor para este proveedor
                    var sqlVerificarServicios = "SELECT COUNT(*) FROM ServicioProveedor WHERE ProveedorId = @ProveedorId";
                    var serviciosExisten = await conexion.QuerySingleAsync<int>(sqlVerificarServicios, new { ProveedorId = proveedorId });
                    Console.WriteLine($"[REPOSITORY] Registros en ServicioProveedor: {serviciosExisten}");
                    
                    // Verificar si hay registros activos
                    var sqlVerificarActivos = "SELECT COUNT(*) FROM ServicioProveedor WHERE ProveedorId = @ProveedorId AND Activo = 1";
                    var serviciosActivos = await conexion.QuerySingleAsync<int>(sqlVerificarActivos, new { ProveedorId = proveedorId });
                    Console.WriteLine($"[REPOSITORY] Registros activos en ServicioProveedor: {serviciosActivos}");
                }

                return new DTO<IEnumerable<TipoServicio>>
                {
                    Correcto = true,
                    Datos = serviciosList,
                    Mensaje = $"Servicios del proveedor {proveedorId} obtenidos correctamente"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[REPOSITORY] ERROR en Obtener_servicios_proveedor: {ex.Message}");
                Console.WriteLine($"[REPOSITORY] StackTrace: {ex.StackTrace}");
                
                return new DTO<IEnumerable<TipoServicio>>
                {
                    Correcto = false,
                    Mensaje = $"Error al obtener servicios del proveedor: {ex.Message}"
                };
            }
        }

        public async Task<DTO<bool>> Actualizar_servicios_proveedor(int proveedorId, List<int> serviciosIds)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            using var transaccion = conexion.BeginTransaction();

            try
            {
                // Verificar que el proveedor exists
                var sqlVerificarProveedor = @"SELECT COUNT(*) FROM Proveedor WHERE ProveedorID = @ProveedorId AND Activo = 1;";
                var proveedorExiste = await conexion.QuerySingleAsync<int>(sqlVerificarProveedor, new { ProveedorId = proveedorId }, transaccion);

                if (proveedorExiste == 0)
                {
                    return new DTO<bool>
                    {
                        Correcto = false,
                        Mensaje = "El proveedor especificado no existe o no está activo."
                    };
                }

                // 1. Desactivar todos los servicios actuales del proveedor
                var sqlDesactivar = @"UPDATE ServicioProveedor 
                                     SET Activo = 0, 
                                         FechaModificacion = GETDATE(),
                                         UsuarioModificadorId = @UsuarioModificadorId
                                     WHERE ProveedorId = @ProveedorId;";

                await conexion.ExecuteAsync(sqlDesactivar, new { 
                    ProveedorId = proveedorId,
                    UsuarioModificadorId = 1 // TODO: Obtener del contexto del usuario actual
                }, transaccion);

                // 2. Insertar o reactivar los servicios seleccionados
                if (serviciosIds != null && serviciosIds.Any())
                {
                    foreach (var servicioId in serviciosIds)
                    {
                        // Verificar que el tipo de servicio existe
                        var sqlVerificarServicio = @"SELECT COUNT(*) FROM TipoServicio WHERE TipoServicioID = @ServiceId;";
                        var servicioExiste = await conexion.QuerySingleAsync<int>(sqlVerificarServicio, new { ServiceId = servicioId }, transaccion);

                        Console.WriteLine($"[REPOSITORY] ¿Existe el tipo de servicio {servicioId}? {servicioExiste > 0}");

                        if (servicioExiste == 0)
                        {
                            Console.WriteLine($"[REPOSITORY] ERROR: El tipo de servicio con ID {servicioId} no existe");
                            transaccion.Rollback();
                            return new DTO<bool>
                            {
                                Correcto = false,
                                Mensaje = $"El tipo de servicio con ID {servicioId} no existe."
                            };
                        }

                        // Insertar o reactivar el servicio (usando ServicioId en lugar de TipoServicioId)
                        var sqlUpsert = @"
                            IF EXISTS (SELECT 1 FROM ServicioProveedor WHERE ProveedorId = @ProveedorId AND ServicioId = @ServicioId)
                            BEGIN
                                UPDATE ServicioProveedor 
                                SET Activo = 1,
                                    FechaModificacion = GETDATE(),
                                    UsuarioModificadorId = @UsuarioModificadorId
                                WHERE ProveedorId = @ProveedorId AND ServicioId = @ServicioId
                            END
                            ELSE
                            BEGIN
                                INSERT INTO ServicioProveedor (ProveedorId, ServicioId, FechaAsignacion, Activo, UsuarioCreadorId, FechaCreacion)
                                VALUES (@ProveedorId, @ServicioId, GETDATE(), 1, @UsuarioCreadorId, GETDATE())
                            END";

                        await conexion.ExecuteAsync(sqlUpsert, new
                        {
                            ProveedorId = proveedorId,
                            ServicioId = servicioId, // Usar ServicioId en lugar de TipoServicioId
                            UsuarioCreadorId = 1, // TODO: Obtener del contexto del usuario actual
                            UsuarioModificadorId = 1 // TODO: Obtener del contexto del usuario actual
                        }, transaccion);
                    }
                }

                transaccion.Commit();

                return new DTO<bool>
                {
                    Correcto = true,
                    Datos = true,
                    Mensaje = $"Servicios del proveedor actualizados correctamente. {serviciosIds?.Count ?? 0} servicios asignados."
                };
            }
            catch (Exception ex)
            {
                transaccion.Rollback();
                return new DTO<bool>
                {
                    Correcto = false,
                    Mensaje = $"Error al actualizar servicios del proveedor: {ex.Message}"
                };
            }
        }

        public async Task<DTO<bool>> Eliminar_servicios_proveedor(int proveedorId)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();

            try
            {
                var sql = @"UPDATE ServicioProveedor 
                           SET Activo = 0, 
                               FechaModificacion = GETDATE(),
                               UsuarioModificadorId = @UsuarioModificadorId
                           WHERE ProveedorId = @ProveedorId;";

                var filasAfectadas = await conexion.ExecuteAsync(sql, new { 
                    ProveedorId = proveedorId,
                    UsuarioModificadorId = 1 // TODO: Obtener del contexto del usuario actual
                });

                return new DTO<bool>
                {
                    Correcto = true,
                    Datos = filasAfectadas > 0,
                    Mensaje = filasAfectadas > 0 ? 
                        $"Servicios del proveedor {proveedorId} eliminados correctamente." : 
                        $"No se encontraron servicios para el proveedor {proveedorId}."
                };
            }
            catch (Exception ex)
            {
                return new DTO<bool>
                {
                    Correcto = false,
                    Mensaje = $"Error al eliminar servicios del proveedor: {ex.Message}"
                };
            }
        }

        public async Task<DTO<Items_pagina<Proveedor>>> Obtener_paginado_por_estado(Filtros_paginado filtros, bool? soloActivos = null)
        {
            Console.WriteLine($"[REPOSITORY] ===== OBTENER PAGINADO POR ESTADO =====");
            Console.WriteLine($"[REPOSITORY] Filtros - Skip: {filtros.Skip}, Top: {filtros.Top}, Filter: {filtros.Filter ?? "null"}, OrderBy: {filtros.OrderBy ?? "null"}, SoloActivos: {soloActivos?.ToString() ?? "null"}");
            Console.WriteLine($"[REPOSITORY] Connection String: {InfoSQL.Conexion?.Substring(0, Math.Min(80, InfoSQL.Conexion?.Length ?? 0)) + "..."}");
            
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            
            try
            {
                Console.WriteLine("[REPOSITORY] Abriendo conexión a base de datos...");
                await conexion.OpenAsync();
                Console.WriteLine("[REPOSITORY] Conexión abierta exitosamente");

                var whereClause = new List<string>();
                var parametros = new Dictionary<string, object>();

                // Filtro de estado
                if (soloActivos.HasValue)
                {
                    whereClause.Add("Activo = @Activo");
                    parametros.Add("Activo", soloActivos.Value);
                }

                // Filtro de búsqueda
                if (!string.IsNullOrEmpty(filtros.Filter))
                {
                    whereClause.Add("(Nombre LIKE '%' + @FiltroBusqueda + '%' OR RFC LIKE '%' + @FiltroBusqueda + '%' OR Email LIKE '%' + @FiltroBusqueda + '%')");
                    parametros.Add("FiltroBusqueda", filtros.Filter);
                }

                var whereClauseStr = whereClause.Any() ? "WHERE " + string.Join(" AND ", whereClause) : "";

                // Validar campo de ordenamiento
                var camposPermitidos = new[] { "Nombre", "RFC", "Email", "FechaCreacion" };
                var ordenarPor = camposPermitidos.Contains(filtros.OrderBy) ? filtros.OrderBy : "Nombre";

                var sql = $@"SELECT 
                                COUNT(*) OVER() AS TotalItems,
                                p.ProveedorID as Id, p.Nombre, p.RFC, p.Email, p.Telefono, p.Direccion, p.UsuarioId, p.Activo, p.FechaCreacion, p.FechaModificacion, p.UsuarioCreadorId, p.UsuarioModificadorId,
                                ISNULL(a.CalificacionPromedio, 0) as CalificacionPromedio,
                                ISNULL(a.TrabajosRealizados, 0) as TrabajosRealizados
                            FROM Proveedor p
                            LEFT JOIN (
                                SELECT ProveedorId, AVG(CAST(Valor AS DECIMAL(18,2))) AS CalificacionPromedio, COUNT(*) AS TrabajosRealizados
                                FROM CalificacionProveedor
                                GROUP BY ProveedorId
                            ) a ON a.ProveedorId = p.ProveedorID
                            {whereClauseStr}
                            ORDER BY {ordenarPor}
                            OFFSET @Skip ROWS FETCH NEXT @Top ROWS ONLY;";

                parametros.Add("Skip", filtros.Skip < 0 ? 0 : filtros.Skip);
                parametros.Add("Top", filtros.Top <= 0 ? 10 : filtros.Top);

                Console.WriteLine($"[REPOSITORY] Ejecutando query paginado con estado...");
                Console.WriteLine($"[REPOSITORY] SQL: {sql}");
                Console.WriteLine($"[REPOSITORY] Parámetros: {string.Join(", ", parametros.Select(p => $"{p.Key}={p.Value}"))}");

                var lista = await conexion.QueryAsync<dynamic>(sql, parametros);

                var proveedores = new List<Proveedor>();
                int totalItems = 0;

                Console.WriteLine($"[REPOSITORY] Query ejecutada. Resultados obtenidos: {lista.Count()}");

                foreach (var row in lista)
                {
                    totalItems = row.TotalItems;

                    proveedores.Add(new Proveedor
                    {
                        Id = row.Id,
                        Nombre = row.Nombre,
                        RFC = row.RFC,
                        Email = row.Email,
                        Telefono = row.Telefono,
                        Direccion = row.Direccion,
                        UsuarioId = row.UsuarioId,
                        Activo = row.Activo,
                        FechaCreacion = row.FechaCreacion,
                        FechaModificacion = row.FechaModificacion,
                        UsuarioCreadorId = row.UsuarioCreadorId,
                        UsuarioModificadorId = row.UsuarioModificadorId,
                        CalificacionPromedio = row.CalificacionPromedio,
                        TrabajosRealizados = row.TrabajosRealizados
                    });
                }

                Console.WriteLine($"[REPOSITORY] Procesados {proveedores.Count} proveedores de un total de {totalItems}");

                if (proveedores.Any())
                {
                    foreach (var prov in proveedores.Take(3))
                    {
                        Console.WriteLine($"[REPOSITORY] Proveedor paginado: ID={prov.Id}, Nombre={prov.Nombre}, RFC={prov.RFC}, Activo={prov.Activo}");
                    }
                    if (proveedores.Count > 3)
                    {
                        Console.WriteLine($"[REPOSITORY] ...y {proveedores.Count - 3} proveedores más en esta página");
                    }
                }

                return new DTO<Items_pagina<Proveedor>>
                {
                    Correcto = true,
                    Datos = new Items_pagina<Proveedor>
                    {
                        Total_items = totalItems,
                        Items = proveedores
                    },
                    Mensaje = "Proveedores paginados por estado correctamente."
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[REPOSITORY] ERROR en Obtener_paginado_por_estado: {ex.Message}");
                Console.WriteLine($"[REPOSITORY] StackTrace: {ex.StackTrace}");
                throw;
            }
        }
    }
}