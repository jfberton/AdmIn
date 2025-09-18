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
    public class RolRepository : IRolRepository
    {
        public RolRepository()
        {
        }

        public async Task<DTO<Rol>> Crear(Rol rol)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            using var transaccion = conexion.BeginTransaction();

            try
            {
                var sqlRol = @"INSERT INTO Rol (Nombre) 
                               OUTPUT INSERTED.RolID as Id, INSERTED.Nombre
                               VALUES (@Nombre);";

                var rolCreado = await conexion.QuerySingleOrDefaultAsync<Rol>(sqlRol, new
                {
                    rol.Nombre
                }, transaccion);

                if (rolCreado == null)
                    throw new Exception("No se pudo crear el rol.");

                transaccion.Commit();

                return new DTO<Rol>
                {
                    Correcto = true,
                    Datos = rolCreado,
                    Mensaje = "Rol creado correctamente."
                };
            }
            catch (Exception ex)
            {
                transaccion.Rollback();
                return new DTO<Rol>
                {
                    Correcto = false,
                    Mensaje = $"Error al crear rol: {ex.Message}"
                };
            }
        }

        public async Task<DTO<Rol>> Actualizar(Rol rol)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            using var transaccion = conexion.BeginTransaction();

            try
            {
                var sqlActualizarRol = @"UPDATE Rol
                                        SET Nombre = @Nombre
                                        OUTPUT INSERTED.RolID as Id, INSERTED.Nombre
                                        WHERE RolID = @RolID;";

                var rolActualizado = await conexion.QuerySingleOrDefaultAsync<Rol>(sqlActualizarRol, new
                {
                    RolID = rol.Id,
                    rol.Nombre
                }, transaccion);

                if (rolActualizado == null)
                    throw new Exception("No se pudo actualizar el rol.");

                transaccion.Commit();

                return new DTO<Rol>
                {
                    Correcto = true,
                    Datos = rolActualizado,
                    Mensaje = "Rol actualizado correctamente."
                };
            }
            catch (Exception ex)
            {
                transaccion.Rollback();
                return new DTO<Rol>
                {
                    Correcto = false,
                    Mensaje = $"Error al actualizar rol: {ex.Message}"
                };
            }
        }

        public async Task<DTO<bool>> Eliminar(Rol rol)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            using var transaccion = conexion.BeginTransaction();

            try
            {
                // Primero eliminar relaciones en UsuarioRol
                var sqlEliminarRelaciones = @"DELETE FROM UsuarioRol WHERE RolID = @RolID;";
                await conexion.ExecuteAsync(sqlEliminarRelaciones, new { RolID = rol.Id }, transaccion);

                // Luego eliminar el rol
                var sqlEliminarRol = @"DELETE FROM Rol WHERE RolID = @RolID;";
                var filasAfectadas = await conexion.ExecuteAsync(sqlEliminarRol, new { RolID = rol.Id }, transaccion);

                transaccion.Commit();

                return new DTO<bool>
                {
                    Correcto = filasAfectadas > 0,
                    Datos = filasAfectadas > 0,
                    Mensaje = filasAfectadas > 0 ? "Rol eliminado correctamente." : "No se encontró el rol."
                };
            }
            catch (Exception ex)
            {
                transaccion.Rollback();
                return new DTO<bool>
                {
                    Correcto = false,
                    Mensaje = $"Error al eliminar rol: {ex.Message}"
                };
            }
        }

        public async Task<DTO<Rol>> Obtener_por_id(Rol rol)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();

            var sqlRol = @"SELECT RolID as Id, Nombre FROM Rol WHERE RolID = @RolID;";

            var rolEncontrado = await conexion.QuerySingleOrDefaultAsync<Rol>(sqlRol, new { RolID = rol.Id });
            if (rolEncontrado == null)
                return new DTO<Rol> { Correcto = false, Mensaje = "Rol no encontrado" };

            return new DTO<Rol>
            {
                Correcto = true,
                Datos = rolEncontrado,
                Mensaje = "Rol obtenido correctamente"
            };
        }

        public async Task<DTO<IEnumerable<Rol>>> Obtener_todos()
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();

            var sqlRoles = @"SELECT RolID as Id, Nombre FROM Rol;";

            var roles = (await conexion.QueryAsync<Rol>(sqlRoles)).ToList();

            return new DTO<IEnumerable<Rol>>
            {
                Correcto = true,
                Datos = roles,
                Mensaje = "Roles obtenidos correctamente"
            };
        }

        public async Task<DTO<Items_pagina<Rol>>> Obtener_paginado(Filtros_paginado filtros)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();

            var sql = @"SELECT 
                            COUNT(*) OVER() AS TotalItems,
                            RolID as Id,
                            Nombre
                        FROM Rol
                        WHERE 
                            (@FiltroBusqueda IS NULL OR Nombre LIKE '%' + @FiltroBusqueda + '%')
                        ORDER BY
                            CASE WHEN @OrdenarPor = 'Nombre' THEN Nombre END
                        OFFSET @Skip ROWS FETCH NEXT @Top ROWS ONLY;";

            var lista = await conexion.QueryAsync<dynamic>(sql, new
            {
                FiltroBusqueda = (object?)filtros.Filter ?? DBNull.Value,
                OrdenarPor = (object?)filtros.OrderBy ?? "Nombre",
                Skip = filtros.Skip < 0 ? 0 : filtros.Skip,
                Top = filtros.Top <= 0 ? 10 : filtros.Top
            });

            var roles = new List<Rol>();
            int totalItems = 0;

            foreach (var row in lista)
            {
                totalItems = row.TotalItems;

                roles.Add(new Rol
                {
                    Id = row.Id,
                    Nombre = row.Nombre
                });
            }

            return new DTO<Items_pagina<Rol>>
            {
                Correcto = true,
                Datos = new Items_pagina<Rol>
                {
                    Total_items = totalItems,
                    Items = roles
                },
                Mensaje = "Roles paginados correctamente."
            };
        }

        public async Task<DTO<IEnumerable<Rol>>> Obtener_por_usuario(int usuarioId)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();

            var sqlRoles = @"SELECT r.RolID as Id, r.Nombre 
                             FROM Rol r
                             INNER JOIN UsuarioRol ur ON ur.RolID = r.RolID
                             WHERE ur.UsuarioID = @UsuarioID;";

            var roles = await conexion.QueryAsync<Rol>(sqlRoles, new { UsuarioID = usuarioId });

            return new DTO<IEnumerable<Rol>>
            {
                Correcto = true,
                Datos = roles.ToList(),
                Mensaje = "Roles del usuario obtenidos correctamente"
            };
        }
    }
}