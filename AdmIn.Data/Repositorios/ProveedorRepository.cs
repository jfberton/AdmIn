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
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            using var transaccion = conexion.BeginTransaction();

            try
            {
                var sqlProveedor = @"INSERT INTO Proveedor (Nombre, RFC, Email, Telefono, Direccion, UsuarioId, Activo, FechaCreacion, FechaModificacion, UsuarioCreadorId, UsuarioModificadorId) 
                                   OUTPUT INSERTED.ProveedorID as Id, INSERTED.Nombre, INSERTED.RFC, INSERTED.Email, INSERTED.Telefono, INSERTED.Direccion, INSERTED.UsuarioId, INSERTED.Activo, INSERTED.FechaCreacion, INSERTED.FechaModificacion, INSERTED.UsuarioCreadorId, INSERTED.UsuarioModificadorId
                                   VALUES (@Nombre, @RFC, @Email, @Telefono, @Direccion, @UsuarioId, @Activo, @FechaCreacion, @FechaModificacion, @UsuarioCreadorId, @UsuarioModificadorId);";

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

                // Asignar rol proveedor (RolID = 5) al usuario asociado
                if (proveedor.UsuarioId.HasValue)
                {
                    var sqlAsignarRol = @"INSERT INTO UsuarioRol (UsuarioID, RolID) VALUES (@UsuarioID, @RolID);";
                    await conexion.ExecuteAsync(sqlAsignarRol, new { UsuarioID = proveedor.UsuarioId.Value, RolID = 5 }, transaccion);
                }

                transaccion.Commit();

                return new DTO<Proveedor>
                {
                    Correcto = true,
                    Datos = proveedorCreado,
                    Mensaje = "Proveedor creado correctamente con rol de proveedor asignado."
                };
            }
            catch (Exception ex)
            {
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
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();

            var sqlProveedor = @"SELECT ProveedorID as Id, Nombre, RFC, Email, Telefono, Direccion, UsuarioId, Activo, FechaCreacion, FechaModificacion, UsuarioCreadorId, UsuarioModificadorId 
                               FROM Proveedor WHERE ProveedorID = @Id;";

            var proveedorEncontrado = await conexion.QuerySingleOrDefaultAsync<Proveedor>(sqlProveedor, new { proveedor.Id });
            if (proveedorEncontrado == null)
                return new DTO<Proveedor> { Correcto = false, Mensaje = "Proveedor no encontrado" };

            return new DTO<Proveedor>
            {
                Correcto = true,
                Datos = proveedorEncontrado,
                Mensaje = "Proveedor obtenido correctamente"
            };
        }

        public async Task<DTO<IEnumerable<Proveedor>>> Obtener_todos()
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();

            var sqlProveedores = @"SELECT ProveedorID as Id, Nombre, RFC, Email, Telefono, Direccion, UsuarioId, Activo, FechaCreacion, FechaModificacion, UsuarioCreadorId, UsuarioModificadorId 
                                 FROM Proveedor ORDER BY Nombre;";

            var proveedores = (await conexion.QueryAsync<Proveedor>(sqlProveedores)).ToList();

            return new DTO<IEnumerable<Proveedor>>
            {
                Correcto = true,
                Datos = proveedores,
                Mensaje = "Proveedores obtenidos correctamente"
            };
        }

        public async Task<DTO<Items_pagina<Proveedor>>> Obtener_paginado(Filtros_paginado filtros)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();

            var sql = @"SELECT 
                            COUNT(*) OVER() AS TotalItems,
                            ProveedorID as Id, Nombre, RFC, Email, Telefono, Direccion, UsuarioId, Activo, FechaCreacion, FechaModificacion, UsuarioCreadorId, UsuarioModificadorId
                        FROM Proveedor
                        WHERE 
                            (@FiltroBusqueda IS NULL OR 
                             Nombre LIKE '%' + @FiltroBusqueda + '%' OR 
                             RFC LIKE '%' + @FiltroBusqueda + '%' OR 
                             Email LIKE '%' + @FiltroBusqueda + '%')
                        ORDER BY
                            CASE WHEN @OrdenarPor = 'Nombre' THEN Nombre END,
                            CASE WHEN @OrdenarPor = 'RFC' THEN RFC END,
                            CASE WHEN @OrdenarPor = 'Email' THEN Email END,
                            CASE WHEN @OrdenarPor IS NULL THEN Nombre END
                        OFFSET @Skip ROWS FETCH NEXT @Top ROWS ONLY;";

            var lista = await conexion.QueryAsync<dynamic>(sql, new
            {
                FiltroBusqueda = (object?)filtros.Filter ?? DBNull.Value,
                OrdenarPor = (object?)filtros.OrderBy ?? "Nombre",
                Skip = filtros.Skip < 0 ? 0 : filtros.Skip,
                Top = filtros.Top <= 0 ? 10 : filtros.Top
            });

            var proveedores = new List<Proveedor>();
            int totalItems = 0;

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
                    UsuarioModificadorId = row.UsuarioModificadorId
                });
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

        public async Task<DTO<IEnumerable<Proveedor>>> Obtener_activos()
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();

            var sqlProveedores = @"SELECT ProveedorID as Id, Nombre, RFC, Email, Telefono, Direccion, UsuarioId, Activo, FechaCreacion, FechaModificacion, UsuarioCreadorId, UsuarioModificadorId 
                                 FROM Proveedor WHERE Activo = 1 ORDER BY Nombre;";

            var proveedores = await conexion.QueryAsync<Proveedor>(sqlProveedores);

            return new DTO<IEnumerable<Proveedor>>
            {
                Correcto = true,
                Datos = proveedores.ToList(),
                Mensaje = "Proveedores activos obtenidos correctamente"
            };
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
    }
}