using AdmIn.Common;
using AdmIn.Common.Entidades;
using AdmIn.Common.Repositorios;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace AdmIn.Data.Repositorios
{
    public class TipoServicioRepository : ITipoServicioRepository
    {
        public TipoServicioRepository()
        {
        }

        public async Task<DTO<TipoServicio>> Crear(TipoServicio tipoServicio)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            using var transaccion = conexion.BeginTransaction();

            try
            {
                var sql = @"INSERT INTO TipoServicio (Nombre, Descripcion)
                           OUTPUT INSERTED.TipoServicioID as Id,
                                  INSERTED.Nombre,
                                  INSERTED.Descripcion
                           VALUES (@Nombre, @Descripcion);";

                var tipoServicioCreado = await conexion.QuerySingleOrDefaultAsync<TipoServicio>(sql, new
                {
                    tipoServicio.Nombre,
                    tipoServicio.Descripcion
                }, transaccion);

                if (tipoServicioCreado == null)
                    throw new Exception("No se pudo crear el tipo de servicio.");

                transaccion.Commit();

                return new DTO<TipoServicio>
                {
                    Correcto = true,
                    Datos = tipoServicioCreado,
                    Mensaje = "Tipo de servicio creado correctamente."
                };
            }
            catch (Exception ex)
            {
                transaccion.Rollback();
                return new DTO<TipoServicio>
                {
                    Correcto = false,
                    Mensaje = $"Error al crear tipo de servicio: {ex.Message}"
                };
            }
        }

        public async Task<DTO<TipoServicio>> Actualizar(TipoServicio tipoServicio)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            using var transaccion = conexion.BeginTransaction();

            try
            {
                var sql = @"UPDATE TipoServicio
                           SET Nombre = @Nombre,
                               Descripcion = @Descripcion
                           OUTPUT INSERTED.TipoServicioID as Id,
                                  INSERTED.Nombre,
                                  INSERTED.Descripcion
                           WHERE TipoServicioID = @Id;";

                var tipoServicioActualizado = await conexion.QuerySingleOrDefaultAsync<TipoServicio>(sql, new
                {
                    tipoServicio.Id,
                    tipoServicio.Nombre,
                    tipoServicio.Descripcion
                }, transaccion);

                if (tipoServicioActualizado == null)
                    throw new Exception("No se pudo actualizar el tipo de servicio.");

                transaccion.Commit();

                return new DTO<TipoServicio>
                {
                    Correcto = true,
                    Datos = tipoServicioActualizado,
                    Mensaje = "Tipo de servicio actualizado correctamente."
                };
            }
            catch (Exception ex)
            {
                transaccion.Rollback();
                return new DTO<TipoServicio>
                {
                    Correcto = false,
                    Mensaje = $"Error al actualizar tipo de servicio: {ex.Message}"
                };
            }
        }

        public async Task<DTO<bool>> Eliminar(TipoServicio tipoServicio)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            using var transaccion = conexion.BeginTransaction();

            try
            {
                // Verificar si está siendo usado por algún servicio
                var sqlVerificar = @"SELECT COUNT(*) FROM Servicio WHERE TipoServicioID = @Id;";
                var enUso = await conexion.QuerySingleAsync<int>(sqlVerificar, new { Id = tipoServicio.Id }, transaccion);

                if (enUso > 0)
                {
                    return new DTO<bool>
                    {
                        Correcto = false,
                        Datos = false,
                        Mensaje = "No se puede eliminar el tipo de servicio porque está siendo utilizado por servicios."
                    };
                }

                var sql = @"DELETE FROM TipoServicio WHERE TipoServicioID = @Id;";
                var filasAfectadas = await conexion.ExecuteAsync(sql, new { Id = tipoServicio.Id }, transaccion);

                transaccion.Commit();

                return new DTO<bool>
                {
                    Correcto = filasAfectadas > 0,
                    Datos = filasAfectadas > 0,
                    Mensaje = filasAfectadas > 0 ? "Tipo de servicio eliminado correctamente." : "No se encontró el tipo de servicio."
                };
            }
            catch (Exception ex)
            {
                transaccion.Rollback();
                return new DTO<bool>
                {
                    Correcto = false,
                    Mensaje = $"Error al eliminar tipo de servicio: {ex.Message}"
                };
            }
        }

        public async Task<DTO<TipoServicio>> Obtener_por_id(TipoServicio tipoServicio)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();

            var sql = @"SELECT TipoServicioID as Id, Nombre, Descripcion 
                        FROM TipoServicio 
                        WHERE TipoServicioID = @Id;";

            var tipoServicioEncontrado = await conexion.QuerySingleOrDefaultAsync<TipoServicio>(sql, new { Id = tipoServicio.Id });

            if (tipoServicioEncontrado == null)
                return new DTO<TipoServicio> { Correcto = false, Mensaje = "Tipo de servicio no encontrado" };

            return new DTO<TipoServicio>
            {
                Correcto = true,
                Datos = tipoServicioEncontrado,
                Mensaje = "Tipo de servicio obtenido correctamente"
            };
        }

        public async Task<DTO<IEnumerable<TipoServicio>>> Obtener_todos()
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();

            var sql = @"SELECT TipoServicioID as Id, Nombre, Descripcion 
                        FROM TipoServicio 
                        ORDER BY Nombre;";

            var tiposServicio = await conexion.QueryAsync<TipoServicio>(sql);

            return new DTO<IEnumerable<TipoServicio>>
            {
                Correcto = true,
                Datos = tiposServicio,
                Mensaje = "Tipos de servicio obtenidos correctamente"
            };
        }

        public async Task<DTO<Items_pagina<TipoServicio>>> Obtener_paginado(Filtros_paginado filtros)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();

            var sql = @"SELECT 
                            COUNT(*) OVER() AS TotalItems,
                            TipoServicioID as Id,
                            Nombre,
                            Descripcion
                        FROM TipoServicio
                        WHERE 
                            (@FiltroBusqueda IS NULL OR Nombre LIKE '%' + @FiltroBusqueda + '%' 
                             OR Descripcion LIKE '%' + @FiltroBusqueda + '%')
                        ORDER BY
                            CASE WHEN @OrdenarPor = 'Nombre' THEN Nombre END,
                            CASE WHEN @OrdenarPor = 'Descripcion' THEN Descripcion END
                        OFFSET @Skip ROWS FETCH NEXT @Top ROWS ONLY;";

            var lista = await conexion.QueryAsync<dynamic>(sql, new
            {
                FiltroBusqueda = (object?)filtros.Filter ?? DBNull.Value,
                OrdenarPor = (object?)filtros.OrderBy ?? "Nombre",
                Skip = filtros.Skip < 0 ? 0 : filtros.Skip,
                Top = filtros.Top <= 0 ? 10 : filtros.Top
            });

            var tiposServicio = new List<TipoServicio>();
            int totalItems = 0;

            foreach (var row in lista)
            {
                totalItems = row.TotalItems;

                var tipoServicio = new TipoServicio
                {
                    Id = row.Id,
                    Nombre = row.Nombre,
                    Descripcion = row.Descripcion
                };

                tiposServicio.Add(tipoServicio);
            }

            return new DTO<Items_pagina<TipoServicio>>
            {
                Correcto = true,
                Datos = new Items_pagina<TipoServicio>
                {
                    Total_items = totalItems,
                    Items = tiposServicio
                },
                Mensaje = "Tipos de servicio paginados correctamente."
            };
        }
    }
}