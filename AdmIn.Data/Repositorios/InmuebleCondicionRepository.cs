using AdmIn.Common;
using AdmIn.Common.Entidades;
using AdmIn.Common.Repositorios;
using Dapper;
using Microsoft.Data.SqlClient;

namespace AdmIn.Data.Repositorios
{
    public class InmuebleCondicionRepository : IInmuebleCondicionRepository
    {
        public async Task<DTO<InmuebleCondicion>> Crear(InmuebleCondicion condicion)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            using var transaccion = conexion.BeginTransaction();

            try
            {
                var sql = @"INSERT INTO InmuebleCondicion (Nombre, Descripcion, Color, Activo, Orden, UsuarioCreadorId, UsuarioModificadorId)
                           OUTPUT INSERTED.Id,
                                  INSERTED.Nombre,
                                  INSERTED.Descripcion,
                                  INSERTED.Color,
                                  INSERTED.Activo,
                                  INSERTED.Orden,
                                  INSERTED.FechaCreacion,
                                  INSERTED.FechaModificacion,
                                  INSERTED.UsuarioCreadorId,
                                  INSERTED.UsuarioModificadorId
                           VALUES (@Nombre, @Descripcion, @Color, @Activo, @Orden, @UsuarioCreadorId, @UsuarioModificadorId);";

                var condicionCreada = await conexion.QuerySingleOrDefaultAsync<InmuebleCondicion>(sql, new
                {
                    condicion.Nombre,
                    condicion.Descripcion,
                    condicion.Color,
                    condicion.Activo,
                    condicion.Orden,
                    condicion.UsuarioCreadorId,
                    condicion.UsuarioModificadorId
                }, transaccion);

                if (condicionCreada == null)
                    throw new Exception("No se pudo crear la condición del inmueble.");

                transaccion.Commit();

                return new DTO<InmuebleCondicion>
                {
                    Correcto = true,
                    Datos = condicionCreada,
                    Mensaje = "Condición del inmueble creada correctamente."
                };
            }
            catch (Exception ex)
            {
                transaccion.Rollback();
                return new DTO<InmuebleCondicion>
                {
                    Correcto = false,
                    Mensaje = $"Error al crear condición del inmueble: {ex.Message}"
                };
            }
        }

        public async Task<DTO<InmuebleCondicion>> Actualizar(InmuebleCondicion condicion)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            using var transaccion = conexion.BeginTransaction();

            try
            {
                var sql = @"UPDATE InmuebleCondicion
                           SET Nombre = @Nombre,
                               Descripcion = @Descripcion,
                               Color = @Color,
                               Activo = @Activo,
                               Orden = @Orden,
                               FechaModificacion = GETDATE(),
                               UsuarioModificadorId = @UsuarioModificadorId
                           OUTPUT INSERTED.Id,
                                  INSERTED.Nombre,
                                  INSERTED.Descripcion,
                                  INSERTED.Color,
                                  INSERTED.Activo,
                                  INSERTED.Orden,
                                  INSERTED.FechaCreacion,
                                  INSERTED.FechaModificacion,
                                  INSERTED.UsuarioCreadorId,
                                  INSERTED.UsuarioModificadorId
                           WHERE Id = @Id;";

                var condicionActualizada = await conexion.QuerySingleOrDefaultAsync<InmuebleCondicion>(sql, new
                {
                    condicion.Id,
                    condicion.Nombre,
                    condicion.Descripcion,
                    condicion.Color,
                    condicion.Activo,
                    condicion.Orden,
                    condicion.UsuarioModificadorId
                }, transaccion);

                if (condicionActualizada == null)
                    throw new Exception("No se pudo actualizar la condición del inmueble.");

                transaccion.Commit();

                return new DTO<InmuebleCondicion>
                {
                    Correcto = true,
                    Datos = condicionActualizada,
                    Mensaje = "Condición del inmueble actualizada correctamente."
                };
            }
            catch (Exception ex)
            {
                transaccion.Rollback();
                return new DTO<InmuebleCondicion>
                {
                    Correcto = false,
                    Mensaje = $"Error al actualizar condición del inmueble: {ex.Message}"
                };
            }
        }

        public async Task<DTO<bool>> Eliminar(InmuebleCondicion condicion)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            using var transaccion = conexion.BeginTransaction();

            try
            {
                // Verificar si está siendo usada por algún inmueble
                var sqlVerificar = @"SELECT COUNT(*) FROM Inmueble WHERE CondicionId = @Id;";
                var enUso = await conexion.QuerySingleAsync<int>(sqlVerificar, new { Id = condicion.Id }, transaccion);

                if (enUso > 0)
                {
                    return new DTO<bool>
                    {
                        Correcto = false,
                        Datos = false,
                        Mensaje = "No se puede eliminar la condición porque está siendo utilizada por uno o más inmuebles."
                    };
                }

                var sql = @"DELETE FROM InmuebleCondicion WHERE Id = @Id;";
                var filasAfectadas = await conexion.ExecuteAsync(sql, new { Id = condicion.Id }, transaccion);

                transaccion.Commit();

                return new DTO<bool>
                {
                    Correcto = filasAfectadas > 0,
                    Datos = filasAfectadas > 0,
                    Mensaje = filasAfectadas > 0 ? "Condición del inmueble eliminada correctamente." : "No se encontró la condición."
                };
            }
            catch (Exception ex)
            {
                transaccion.Rollback();
                return new DTO<bool>
                {
                    Correcto = false,
                    Mensaje = $"Error al eliminar condición del inmueble: {ex.Message}"
                };
            }
        }

        public async Task<DTO<InmuebleCondicion>> Obtener_por_id(InmuebleCondicion condicion)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();

            var sql = @"SELECT Id, Nombre, Descripcion, Color, Activo, Orden, FechaCreacion, FechaModificacion, UsuarioCreadorId, UsuarioModificadorId
                        FROM InmuebleCondicion 
                        WHERE Id = @Id;";

            var condicionEncontrada = await conexion.QuerySingleOrDefaultAsync<InmuebleCondicion>(sql, new { Id = condicion.Id });

            if (condicionEncontrada == null)
                return new DTO<InmuebleCondicion> { Correcto = false, Mensaje = "Condición del inmueble no encontrada" };

            return new DTO<InmuebleCondicion>
            {
                Correcto = true,
                Datos = condicionEncontrada,
                Mensaje = "Condición del inmueble obtenida correctamente"
            };
        }

        public async Task<DTO<IEnumerable<InmuebleCondicion>>> Obtener_todos()
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();

            var sql = @"SELECT Id, Nombre, Descripcion, Color, Activo, Orden, FechaCreacion, FechaModificacion, UsuarioCreadorId, UsuarioModificadorId
                        FROM InmuebleCondicion 
                        ORDER BY Orden, Nombre;";

            var condiciones = await conexion.QueryAsync<InmuebleCondicion>(sql);

            return new DTO<IEnumerable<InmuebleCondicion>>
            {
                Correcto = true,
                Datos = condiciones,
                Mensaje = "Condiciones del inmueble obtenidas correctamente"
            };
        }

        public async Task<DTO<Items_pagina<InmuebleCondicion>>> Obtener_paginado(Filtros_paginado filtros)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();

            var sql = @"SELECT 
                            COUNT(*) OVER() AS TotalItems,
                            Id,
                            Nombre,
                            Descripcion,
                            Color,
                            Activo,
                            Orden,
                            FechaCreacion,
                            FechaModificacion,
                            UsuarioCreadorId,
                            UsuarioModificadorId
                        FROM InmuebleCondicion
                        WHERE 
                            (@FiltroBusqueda IS NULL OR Nombre LIKE '%' + @FiltroBusqueda + '%' 
                             OR Descripcion LIKE '%' + @FiltroBusqueda + '%')
                        ORDER BY
                            CASE WHEN @OrdenarPor = 'Nombre' THEN Nombre END,
                            CASE WHEN @OrdenarPor = 'Orden' THEN Orden END,
                            CASE WHEN @OrdenarPor = 'FechaCreacion' THEN FechaCreacion END
                        OFFSET @Skip ROWS FETCH NEXT @Top ROWS ONLY;";

            var lista = await conexion.QueryAsync<dynamic>(sql, new
            {
                FiltroBusqueda = (object?)filtros.Filter ?? DBNull.Value,
                OrdenarPor = (object?)filtros.OrderBy ?? "Orden",
                Skip = filtros.Skip < 0 ? 0 : filtros.Skip,
                Top = filtros.Top <= 0 ? 10 : filtros.Top
            });

            var condiciones = new List<InmuebleCondicion>();
            int totalItems = 0;

            foreach (var row in lista)
            {
                totalItems = row.TotalItems;

                var condicion = new InmuebleCondicion
                {
                    Id = row.Id,
                    Nombre = row.Nombre,
                    Descripcion = row.Descripcion,
                    Color = row.Color,
                    Activo = row.Activo,
                    Orden = row.Orden,
                    FechaCreacion = row.FechaCreacion,
                    FechaModificacion = row.FechaModificacion,
                    UsuarioCreadorId = row.UsuarioCreadorId,
                    UsuarioModificadorId = row.UsuarioModificadorId
                };

                condiciones.Add(condicion);
            }

            return new DTO<Items_pagina<InmuebleCondicion>>
            {
                Correcto = true,
                Datos = new Items_pagina<InmuebleCondicion>
                {
                    Total_items = totalItems,
                    Items = condiciones
                },
                Mensaje = "Condiciones del inmueble paginadas correctamente."
            };
        }

        public async Task<DTO<IEnumerable<InmuebleCondicion>>> Obtener_activos()
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();

            var sql = @"SELECT Id, Nombre, Descripcion, Color, Activo, Orden, FechaCreacion, FechaModificacion, UsuarioCreadorId, UsuarioModificadorId
                        FROM InmuebleCondicion 
                        WHERE Activo = 1
                        ORDER BY Orden, Nombre;";

            var condiciones = await conexion.QueryAsync<InmuebleCondicion>(sql);

            return new DTO<IEnumerable<InmuebleCondicion>>
            {
                Correcto = true,
                Datos = condiciones,
                Mensaje = "Condiciones activas del inmueble obtenidas correctamente"
            };
        }

        public async Task<DTO<InmuebleCondicion>> Obtener_por_nombre(string nombre)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();

            var sql = @"SELECT Id, Nombre, Descripcion, Color, Activo, Orden, FechaCreacion, FechaModificacion, UsuarioCreadorId, UsuarioModificadorId
                        FROM InmuebleCondicion 
                        WHERE Nombre = @Nombre;";

            var condicion = await conexion.QuerySingleOrDefaultAsync<InmuebleCondicion>(sql, new { Nombre = nombre });

            if (condicion == null)
                return new DTO<InmuebleCondicion> { Correcto = false, Mensaje = "Condición del inmueble no encontrada" };

            return new DTO<InmuebleCondicion>
            {
                Correcto = true,
                Datos = condicion,
                Mensaje = "Condición del inmueble obtenida correctamente"
            };
        }

        public async Task<DTO<InmuebleCondicion>> Obtener_por_defecto()
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();

            var sql = @"SELECT TOP 1 Id, Nombre, Descripcion, Color, Activo, Orden, FechaCreacion, FechaModificacion, UsuarioCreadorId, UsuarioModificadorId
                        FROM InmuebleCondicion 
                        WHERE Activo = 1
                        ORDER BY Orden, Id;";

            var condicion = await conexion.QuerySingleOrDefaultAsync<InmuebleCondicion>(sql);

            if (condicion == null)
                return new DTO<InmuebleCondicion> { Correcto = false, Mensaje = "No se encontró condición por defecto" };

            return new DTO<InmuebleCondicion>
            {
                Correcto = true,
                Datos = condicion,
                Mensaje = "Condición por defecto obtenida correctamente"
            };
        }
    }
}