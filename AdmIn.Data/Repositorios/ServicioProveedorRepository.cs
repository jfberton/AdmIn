using AdmIn.Common;
using AdmIn.Common.Entidades;
using AdmIn.Common.Repositorios;
using Dapper;
using Microsoft.Data.SqlClient;

namespace AdmIn.Data.Repositorios
{
    public class ServicioProveedorRepository : IServicioProveedorRepository
    {
        public ServicioProveedorRepository()
        {
        }

        public async Task<DTO<ServicioProveedor>> Crear(ServicioProveedor servicioProveedor)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            using var transaccion = conexion.BeginTransaction();

            try
            {
                var sql = @"INSERT INTO ServicioProveedor (ProveedorId, ServicioId, FechaAsignacion, Activo, UsuarioCreadorId, FechaCreacion, UsuarioModificadorId, FechaModificacion)
                           OUTPUT INSERTED.ServicioProveedorId as Id,
                                  INSERTED.ProveedorId,
                                  INSERTED.ServicioId,
                                  INSERTED.FechaAsignacion,
                                  INSERTED.Activo,
                                  INSERTED.UsuarioCreadorId,
                                  INSERTED.FechaCreacion,
                                  INSERTED.UsuarioModificadorId,
                                  INSERTED.FechaModificacion
                           VALUES (@ProveedorId, @ServicioId, @FechaAsignacion, @Activo, @UsuarioCreadorId, @FechaCreacion, @UsuarioModificadorId, @FechaModificacion);";

                var servicioProveedorCreado = await conexion.QuerySingleOrDefaultAsync<ServicioProveedor>(sql, new
                {
                    servicioProveedor.ProveedorId,
                    servicioProveedor.ServicioId,
                    servicioProveedor.FechaAsignacion,
                    servicioProveedor.Activo,
                    servicioProveedor.UsuarioCreadorId,
                    servicioProveedor.FechaCreacion,
                    servicioProveedor.UsuarioModificadorId,
                    servicioProveedor.FechaModificacion
                }, transaccion);

                if (servicioProveedorCreado == null)
                    throw new Exception("No se pudo crear la relación ServicioProveedor.");

                transaccion.Commit();

                return new DTO<ServicioProveedor>
                {
                    Correcto = true,
                    Datos = servicioProveedorCreado,
                    Mensaje = "Relación ServicioProveedor creada correctamente."
                };
            }
            catch (Exception ex)
            {
                transaccion.Rollback();
                return new DTO<ServicioProveedor>
                {
                    Correcto = false,
                    Mensaje = $"Error al crear ServicioProveedor: {ex.Message}"
                };
            }
        }

        public async Task<DTO<ServicioProveedor>> Actualizar(ServicioProveedor servicioProveedor)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            using var transaccion = conexion.BeginTransaction();

            try
            {
                var sql = @"UPDATE ServicioProveedor
                           SET ProveedorId = @ProveedorId,
                               ServicioId = @ServicioId,
                               FechaAsignacion = @FechaAsignacion,
                               Activo = @Activo,
                               UsuarioModificadorId = @UsuarioModificadorId,
                               FechaModificacion = @FechaModificacion
                           OUTPUT INSERTED.ServicioProveedorId as Id,
                                  INSERTED.ProveedorId,
                                  INSERTED.ServicioId,
                                  INSERTED.FechaAsignacion,
                                  INSERTED.Activo,
                                  INSERTED.UsuarioCreadorId,
                                  INSERTED.FechaCreacion,
                                  INSERTED.UsuarioModificadorId,
                                  INSERTED.FechaModificacion
                           WHERE ServicioProveedorId = @Id;";

                var servicioProveedorActualizado = await conexion.QuerySingleOrDefaultAsync<ServicioProveedor>(sql, new
                {
                    servicioProveedor.Id,
                    servicioProveedor.ProveedorId,
                    servicioProveedor.ServicioId,
                    servicioProveedor.FechaAsignacion,
                    servicioProveedor.Activo,
                    servicioProveedor.UsuarioModificadorId,
                    servicioProveedor.FechaModificacion
                }, transaccion);

                if (servicioProveedorActualizado == null)
                    throw new Exception("No se pudo actualizar la relación ServicioProveedor.");

                transaccion.Commit();

                return new DTO<ServicioProveedor>
                {
                    Correcto = true,
                    Datos = servicioProveedorActualizado,
                    Mensaje = "Relación ServicioProveedor actualizada correctamente."
                };
            }
            catch (Exception ex)
            {
                transaccion.Rollback();
                return new DTO<ServicioProveedor>
                {
                    Correcto = false,
                    Mensaje = $"Error al actualizar ServicioProveedor: {ex.Message}"
                };
            }
        }

        public async Task<DTO<bool>> Eliminar(ServicioProveedor servicioProveedor)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            using var transaccion = conexion.BeginTransaction();

            try
            {
                // Soft delete - cambiar Activo a false en lugar de eliminar físicamente
                var sql = @"UPDATE ServicioProveedor 
                           SET Activo = 0, 
                               FechaModificacion = GETDATE(),
                               UsuarioModificadorId = @UsuarioModificadorId
                           WHERE ServicioProveedorId = @Id;";

                var filasAfectadas = await conexion.ExecuteAsync(sql, new 
                { 
                    Id = servicioProveedor.Id,
                    UsuarioModificadorId = servicioProveedor.UsuarioModificadorId ?? 1
                }, transaccion);

                transaccion.Commit();

                return new DTO<bool>
                {
                    Correcto = filasAfectadas > 0,
                    Datos = filasAfectadas > 0,
                    Mensaje = filasAfectadas > 0 ? "Relación ServicioProveedor eliminada correctamente." : "No se encontró la relación ServicioProveedor."
                };
            }
            catch (Exception ex)
            {
                transaccion.Rollback();
                return new DTO<bool>
                {
                    Correcto = false,
                    Mensaje = $"Error al eliminar ServicioProveedor: {ex.Message}"
                };
            }
        }

        public async Task<DTO<ServicioProveedor>> Obtener_por_id(ServicioProveedor servicioProveedor)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();

            // ? MAPEO CORRECTO: ServicioProveedorId as Id
            var sql = @"SELECT 
                            ServicioProveedorId as Id, 
                            ProveedorId, 
                            ServicioId, 
                            FechaAsignacion, 
                            Activo, 
                            UsuarioCreadorId, 
                            FechaCreacion, 
                            UsuarioModificadorId, 
                            FechaModificacion
                        FROM ServicioProveedor 
                        WHERE ServicioProveedorId = @Id;";

            var servicioProveedorEncontrado = await conexion.QuerySingleOrDefaultAsync<ServicioProveedor>(sql, new { Id = servicioProveedor.Id });

            if (servicioProveedorEncontrado == null)
                return new DTO<ServicioProveedor> { Correcto = false, Mensaje = "Relación ServicioProveedor no encontrada" };

            return new DTO<ServicioProveedor>
            {
                Correcto = true,
                Datos = servicioProveedorEncontrado,
                Mensaje = "Relación ServicioProveedor obtenida correctamente"
            };
        }

        public async Task<DTO<IEnumerable<ServicioProveedor>>> Obtener_todos()
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();

            // ? MAPEO CORRECTO: ServicioProveedorId as Id
            var sql = @"SELECT 
                            ServicioProveedorId as Id, 
                            ProveedorId, 
                            ServicioId, 
                            FechaAsignacion, 
                            Activo, 
                            UsuarioCreadorId, 
                            FechaCreacion, 
                            UsuarioModificadorId, 
                            FechaModificacion
                        FROM ServicioProveedor 
                        WHERE Activo = 1
                        ORDER BY ProveedorId, ServicioId;";

            var serviciosProveedor = await conexion.QueryAsync<ServicioProveedor>(sql);

            return new DTO<IEnumerable<ServicioProveedor>>
            {
                Correcto = true,
                Datos = serviciosProveedor,
                Mensaje = "Relaciones ServicioProveedor obtenidas correctamente"
            };
        }

        public async Task<DTO<Items_pagina<ServicioProveedor>>> Obtener_paginado(Filtros_paginado filtros)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();

            var sql = @"SELECT 
                            COUNT(*) OVER() AS TotalItems,
                            ServicioProveedorId as Id,
                            ProveedorId,
                            ServicioId,
                            FechaAsignacion,
                            Activo,
                            UsuarioCreadorId,
                            FechaCreacion,
                            UsuarioModificadorId,
                            FechaModificacion
                        FROM ServicioProveedor
                        WHERE 
                            Activo = 1 AND
                            (@FiltroBusqueda IS NULL OR 
                             ProveedorId = TRY_CAST(@FiltroBusqueda AS INT) OR
                             ServicioId = TRY_CAST(@FiltroBusqueda AS INT))
                        ORDER BY
                            CASE WHEN @OrdenarPor = 'ProveedorId' THEN ProveedorId END,
                            CASE WHEN @OrdenarPor = 'ServicioId' THEN ServicioId END,
                            CASE WHEN @OrdenarPor = 'FechaAsignacion' THEN FechaAsignacion END,
                            CASE WHEN @OrdenarPor IS NULL THEN FechaCreacion END DESC
                        OFFSET @Skip ROWS FETCH NEXT @Top ROWS ONLY;";

            var lista = await conexion.QueryAsync<dynamic>(sql, new
            {
                FiltroBusqueda = (object?)filtros.Filter ?? DBNull.Value,
                OrdenarPor = (object?)filtros.OrderBy ?? "FechaCreacion",
                Skip = filtros.Skip < 0 ? 0 : filtros.Skip,
                Top = filtros.Top <= 0 ? 10 : filtros.Top
            });

            var serviciosProveedor = new List<ServicioProveedor>();
            int totalItems = 0;

            foreach (var row in lista)
            {
                totalItems = row.TotalItems;

                serviciosProveedor.Add(new ServicioProveedor
                {
                    Id = row.Id,
                    ProveedorId = row.ProveedorId,
                    ServicioId = row.ServicioId,
                    FechaAsignacion = row.FechaAsignacion,
                    Activo = row.Activo,
                    UsuarioCreadorId = row.UsuarioCreadorId,
                    FechaCreacion = row.FechaCreacion,
                    UsuarioModificadorId = row.UsuarioModificadorId,
                    FechaModificacion = row.FechaModificacion
                });
            }

            return new DTO<Items_pagina<ServicioProveedor>>
            {
                Correcto = true,
                Datos = new Items_pagina<ServicioProveedor>
                {
                    Total_items = totalItems,
                    Items = serviciosProveedor
                },
                Mensaje = "Relaciones ServicioProveedor paginadas correctamente."
            };
        }

        public async Task<DTO<IEnumerable<ServicioProveedor>>> Obtener_por_proveedor(int proveedorId)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();

            var sql = @"SELECT 
                            ServicioProveedorId as Id, 
                            ProveedorId, 
                            ServicioId, 
                            FechaAsignacion, 
                            Activo, 
                            UsuarioCreadorId, 
                            FechaCreacion, 
                            UsuarioModificadorId, 
                            FechaModificacion
                        FROM ServicioProveedor 
                        WHERE ProveedorId = @ProveedorId AND Activo = 1
                        ORDER BY ServicioId;";

            var serviciosProveedor = await conexion.QueryAsync<ServicioProveedor>(sql, new { ProveedorId = proveedorId });

            return new DTO<IEnumerable<ServicioProveedor>>
            {
                Correcto = true,
                Datos = serviciosProveedor,
                Mensaje = $"Servicios del proveedor {proveedorId} obtenidos correctamente"
            };
        }

        public async Task<DTO<IEnumerable<ServicioProveedor>>> Obtener_por_servicio(int servicioId)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();

            var sql = @"SELECT 
                            ServicioProveedorId as Id, 
                            ProveedorId, 
                            ServicioId, 
                            FechaAsignacion, 
                            Activo, 
                            UsuarioCreadorId, 
                            FechaCreacion, 
                            UsuarioModificadorId, 
                            FechaModificacion
                        FROM ServicioProveedor 
                        WHERE ServicioId = @ServicioId AND Activo = 1
                        ORDER BY ProveedorId;";

            var serviciosProveedor = await conexion.QueryAsync<ServicioProveedor>(sql, new { ServicioId = servicioId });

            return new DTO<IEnumerable<ServicioProveedor>>
            {
                Correcto = true,
                Datos = serviciosProveedor,
                Mensaje = $"Proveedores que ofrecen el servicio {servicioId} obtenidos correctamente"
            };
        }
    }
}