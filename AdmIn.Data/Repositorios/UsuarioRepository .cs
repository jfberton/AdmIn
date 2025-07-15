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
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly string _connectionString;

        public UsuarioRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<DTO<Usuario>> Crear(Usuario usuario)
        {
            using var conexion = new SqlConnection(_connectionString);
            await conexion.OpenAsync();
            using var transaccion = conexion.BeginTransaction();

            try
            {
                var sqlUsuario = @"INSERT INTO Usuario 
                    (Nombre, Email, Password, Pais, Telefono, PersonaID, EmpresaID, MonedaID, Activo, FechaCreacion, FechaModificacion, UsuarioCreadorID)
                    OUTPUT INSERTED.*
                    VALUES (@Nombre, @Email, @Password, @Pais, @Telefono, @PersonaID, @EmpresaID, @MonedaID, @Activo, GETDATE(), GETDATE(), @UsuarioCreadorID);";

                var usuarioCreado = await conexion.QuerySingleOrDefaultAsync<Usuario>(sqlUsuario, new
                {
                    usuario.Nombre,
                    usuario.Email,
                    usuario.Password,
                    Pais = (object?)usuario.Pais ?? DBNull.Value,
                    Telefono = (object?)usuario.Telefono ?? DBNull.Value,
                    PersonaID = (object?)usuario.PersonaId ?? DBNull.Value,
                    EmpresaID = (object?)usuario.EmpresaId ?? DBNull.Value,
                    usuario.MonedaId,
                    usuario.Activo,
                    UsuarioCreadorID = (object?)usuario.UsuarioCreador ?? DBNull.Value
                }, transaccion);

                if (usuarioCreado == null)
                    throw new Exception("No se pudo crear el usuario.");

                // Insertar los roles en UsuarioRol
                if (usuario.Roles?.Any() == true)
                {
                    var sqlUsuarioRol = @"INSERT INTO UsuarioRol (UsuarioID, RolID) VALUES (@UsuarioID, @RolID);";

                    foreach (var rol in usuario.Roles)
                    {
                        await conexion.ExecuteAsync(sqlUsuarioRol, new { UsuarioID = usuarioCreado.Id, RolID = rol.Id }, transaccion);
                    }
                }

                transaccion.Commit();

                return new DTO<Usuario>
                {
                    Correcto = true,
                    Datos = usuarioCreado,
                    Mensaje = "Usuario creado correctamente."
                };
            }
            catch (Exception ex)
            {
                transaccion.Rollback();
                return new DTO<Usuario>
                {
                    Correcto = false,
                    Mensaje = $"Error al crear usuario: {ex.Message}"
                };
            }
        }

        public async Task<DTO<Usuario>> Actualizar(Usuario usuario)
        {
            using var conexion = new SqlConnection(_connectionString);
            await conexion.OpenAsync();
            using var transaccion = conexion.BeginTransaction();

            try
            {
                var sqlActualizarUsuario = @"UPDATE Usuario
                    SET Nombre = @Nombre,
                        Email = @Email,
                        Pais = @Pais,
                        Telefono = @Telefono,
                        PersonaID = @PersonaID,
                        EmpresaID = @EmpresaID,
                        MonedaID = @MonedaID,
                        Activo = @Activo,
                        FechaModificacion = GETDATE(),
                        UsuarioModificadorID = @UsuarioModificadorID
                    OUTPUT INSERTED.*
                    WHERE UsuarioID = @UsuarioID;";

                var usuarioActualizado = await conexion.QuerySingleOrDefaultAsync<Usuario>(sqlActualizarUsuario, new
                {
                    UsuarioID = usuario.Id,
                    usuario.Nombre,
                    usuario.Email,
                    Pais = (object?)usuario.Pais ?? DBNull.Value,
                    Telefono = (object?)usuario.Telefono ?? DBNull.Value,
                    PersonaID = (object?)usuario.PersonaId ?? DBNull.Value,
                    EmpresaID = (object?)usuario.EmpresaId ?? DBNull.Value,
                    usuario.MonedaId,
                    usuario.Activo,
                    UsuarioModificadorID = (object?)usuario.UsuarioModificador ?? DBNull.Value
                }, transaccion);

                if (usuarioActualizado == null)
                    throw new Exception("No se pudo actualizar el usuario.");

                // Primero eliminar roles actuales asignados
                var sqlEliminarRoles = @"DELETE FROM UsuarioRol WHERE UsuarioID = @UsuarioID;";
                await conexion.ExecuteAsync(sqlEliminarRoles, new { UsuarioID = usuario.Id }, transaccion);

                // Insertar roles actualizados
                if (usuario.Roles?.Any() == true)
                {
                    var sqlInsertarRoles = @"INSERT INTO UsuarioRol (UsuarioID, RolID) VALUES (@UsuarioID, @RolID);";
                    foreach (var rol in usuario.Roles)
                    {
                        await conexion.ExecuteAsync(sqlInsertarRoles, new { UsuarioID = usuario.Id, RolID = rol.Id }, transaccion);
                    }
                }

                transaccion.Commit();

                return new DTO<Usuario>
                {
                    Correcto = true,
                    Datos = usuarioActualizado,
                    Mensaje = "Usuario actualizado correctamente."
                };
            }
            catch (Exception ex)
            {
                transaccion.Rollback();
                return new DTO<Usuario>
                {
                    Correcto = false,
                    Mensaje = $"Error al actualizar usuario: {ex.Message}"
                };
            }
        }

        public async Task<DTO<bool>> Eliminar(Usuario usuario)
        {
            using var conexion = new SqlConnection(_connectionString);
            await conexion.OpenAsync();
            using var transaccion = conexion.BeginTransaction();

            try
            {
                // Primero eliminar roles asignados
                var sqlEliminarRoles = @"DELETE FROM UsuarioRol WHERE UsuarioID = @UsuarioID;";
                await conexion.ExecuteAsync(sqlEliminarRoles, new { UsuarioID = usuario.Id }, transaccion);

                // Luego eliminar el usuario
                var sqlEliminarUsuario = @"DELETE FROM Usuario WHERE UsuarioID = @UsuarioID;";
                var filasAfectadas = await conexion.ExecuteAsync(sqlEliminarUsuario, new { UsuarioID = usuario.Id }, transaccion);

                transaccion.Commit();

                return new DTO<bool>
                {
                    Correcto = filasAfectadas > 0,
                    Datos = filasAfectadas > 0,
                    Mensaje = filasAfectadas > 0 ? "Usuario eliminado correctamente." : "No se encontró el usuario."
                };
            }
            catch (Exception ex)
            {
                transaccion.Rollback();
                return new DTO<bool>
                {
                    Correcto = false,
                    Mensaje = $"Error al eliminar usuario: {ex.Message}"
                };
            }
        }

        public async Task<DTO<Usuario>> Obtener_por_id(Usuario usuario)
        {
            using var conexion = new SqlConnection(_connectionString);
            await conexion.OpenAsync();

            var sqlUsuario = @"SELECT * FROM Usuario WHERE UsuarioID = @UsuarioID;";
            var sqlRoles = @"SELECT r.RolID, r.Nombre FROM Rol r
                             INNER JOIN UsuarioRol ur ON ur.RolID = r.RolID
                             WHERE ur.UsuarioID = @UsuarioID;";

            var usuarioEncontrado = await conexion.QuerySingleOrDefaultAsync<Usuario>(sqlUsuario, new { UsuarioID = usuario.Id });
            if (usuarioEncontrado == null)
                return new DTO<Usuario> { Correcto = false, Mensaje = "Usuario no encontrado" };

            var roles = await conexion.QueryAsync<Rol>(sqlRoles, new { UsuarioID = usuario.Id });
            usuarioEncontrado.Roles = roles.ToList();

            return new DTO<Usuario>
            {
                Correcto = true,
                Datos = usuarioEncontrado,
                Mensaje = "Usuario obtenido correctamente"
            };
        }

        public async Task<DTO<Usuario>> Obtener_por_email(string email)
        {
            using var conexion = new SqlConnection(_connectionString);
            await conexion.OpenAsync();

            var sqlUsuario = @"SELECT * FROM Usuario WHERE Email = @Email;";
            var sqlRoles = @"SELECT r.RolID, r.Nombre FROM Rol r
                             INNER JOIN UsuarioRol ur ON ur.RolID = r.RolID
                             WHERE ur.UsuarioID = @UsuarioID;";

            var usuarioEncontrado = await conexion.QuerySingleOrDefaultAsync<Usuario>(sqlUsuario, new { Email = email });
            if (usuarioEncontrado == null)
                return new DTO<Usuario> { Correcto = false, Mensaje = "Usuario no encontrado" };

            var roles = await conexion.QueryAsync<Rol>(sqlRoles, new { UsuarioID = usuarioEncontrado.Id });
            usuarioEncontrado.Roles = roles.ToList();

            return new DTO<Usuario>
            {
                Correcto = true,
                Datos = usuarioEncontrado,
                Mensaje = "Usuario obtenido correctamente"
            };
        }

        public async Task<DTO<IEnumerable<Usuario>>> Obtener_todos()
        {
            using var conexion = new SqlConnection(_connectionString);
            await conexion.OpenAsync();

            var sqlUsuarios = @"SELECT * FROM Usuario;";
            var sqlRoles = @"SELECT * FROM UsuarioRol;";

            var usuarios = (await conexion.QueryAsync<Usuario>(sqlUsuarios)).ToList();
            var usuarioRoles = (await conexion.QueryAsync<UsuarioRol>(sqlRoles)).ToList();

            // Asociar roles a cada usuario
            foreach (var usuario in usuarios)
            {
                var rolesIds = usuarioRoles.Where(ur => ur.UsuarioID == usuario.Id).Select(ur => ur.RolID).ToList();

                // Opcional: Podrías obtener los nombres si querés, pero acá sólo seteo Id para cada rol
                usuario.Roles = rolesIds.Select(id => new Rol { Id = id }).ToList();
            }

            return new DTO<IEnumerable<Usuario>>
            {
                Correcto = true,
                Datos = usuarios,
                Mensaje = "Usuarios obtenidos correctamente"
            };
        }

        public async Task<DTO<Items_pagina<Usuario>>> Obtener_paginado(Filtros_paginado filtros)
        {
            using var conexion = new SqlConnection(_connectionString);
            await conexion.OpenAsync();

            var sql = @"SELECT 
                            COUNT(*) OVER() AS TotalItems,
                            UsuarioID, Nombre, Email, Pais, Telefono, PersonaID, EmpresaID, MonedaID, Activo, FechaCreacion, FechaModificacion, UsuarioCreadorID, UsuarioModificadorID
                        FROM Usuario
                        WHERE 
                            (@FiltroBusqueda IS NULL OR Nombre LIKE '%' + @FiltroBusqueda + '%' OR Email LIKE '%' + @FiltroBusqueda + '%')
                        ORDER BY
                            CASE WHEN @OrdenarPor = 'Nombre' THEN Nombre END,
                            CASE WHEN @OrdenarPor = 'Email' THEN Email END
                        OFFSET @Skip ROWS FETCH NEXT @Top ROWS ONLY;";

            var lista = await conexion.QueryAsync<dynamic>(sql, new
            {
                FiltroBusqueda = (object?)filtros.Filter ?? DBNull.Value,
                OrdenarPor = (object?)filtros.OrderBy ?? "Nombre",
                Skip = filtros.Skip < 0 ? 0 : filtros.Skip,
                Top = filtros.Top <= 0 ? 10 : filtros.Top
            });

            var usuarios = new List<Usuario>();
            int totalItems = 0;

            foreach (var row in lista)
            {
                totalItems = row.TotalItems;

                usuarios.Add(new Usuario
                {
                    Id = row.UsuarioID,
                    Nombre = row.Nombre,
                    Email = row.Email,
                    Pais = row.Pais,
                    Telefono = row.Telefono,
                    PersonaId = row.PersonaID,
                    EmpresaId = row.EmpresaID,
                    MonedaId = row.MonedaID,
                    Activo = row.Activo,
                    FechaCreacion = row.FechaCreacion,
                    FechaModificacion = row.FechaModificacion,
                    UsuarioCreador = row.UsuarioCreadorID,
                    UsuarioModificador = row.UsuarioModificadorID
                });
            }

            return new DTO<Items_pagina<Usuario>>
            {
                Correcto = true,
                Datos = new Items_pagina<Usuario>
                {
                    Total_items = totalItems,
                    Items = usuarios
                },
                Mensaje = "Usuarios paginados correctamente."
            };
        }
    }
}
