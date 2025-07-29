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
        public UsuarioRepository()
        {
        }

        public async Task<DTO<Usuario>> Crear(Usuario usuario)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            using var transaccion = conexion.BeginTransaction();

            try
            {
                var sqlUsuario = @"INSERT INTO Usuario 
                    (Nombre, Email, Password, Pais, Telefono, PersonaID, EmpresaID, MonedaID, Activo, FechaCreacion, FechaModificacion, UsuarioCreadorID)
                    OUTPUT INSERTED.UsuarioID as Id,
                           INSERTED.Nombre,
                           INSERTED.Email,
                           INSERTED.Password,
                           INSERTED.Pais,
                           INSERTED.Telefono,
                           INSERTED.PersonaID,
                           INSERTED.EmpresaID,
                           INSERTED.MonedaID,
                           INSERTED.Activo,
                           INSERTED.FechaCreacion,
                           INSERTED.FechaModificacion,
                           INSERTED.UsuarioCreadorID,
                           INSERTED.UsuarioModificadorID
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

                // Obtener los roles del usuario creado para devolverlo completo
                var sqlRoles = @"SELECT r.RolID as Id, r.Nombre FROM Rol r
                                 INNER JOIN UsuarioRol ur ON ur.RolID = r.RolID
                                 WHERE ur.UsuarioID = @UsuarioID;";
                
                var roles = await conexion.QueryAsync<Rol>(sqlRoles, new { UsuarioID = usuarioCreado.Id }, transaccion);
                usuarioCreado.Roles = roles.ToList();

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
            using var conexion = new SqlConnection(InfoSQL.Conexion);
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
                    OUTPUT INSERTED.UsuarioID as Id,
                           INSERTED.Nombre,
                           INSERTED.Email,
                           INSERTED.Password,
                           INSERTED.Pais,
                           INSERTED.Telefono,
                           INSERTED.PersonaID,
                           INSERTED.EmpresaID,
                           INSERTED.MonedaID,
                           INSERTED.Activo,
                           INSERTED.FechaCreacion,
                           INSERTED.FechaModificacion,
                           INSERTED.UsuarioCreadorID,
                           INSERTED.UsuarioModificadorID
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

                // Obtener los roles del usuario actualizado para devolverlo completo
                var sqlRoles = @"SELECT r.RolID as Id, r.Nombre FROM Rol r
                                 INNER JOIN UsuarioRol ur ON ur.RolID = r.RolID
                                 WHERE ur.UsuarioID = @UsuarioID;";
                
                var roles = await conexion.QueryAsync<Rol>(sqlRoles, new { UsuarioID = usuario.Id }, transaccion);
                usuarioActualizado.Roles = roles.ToList();

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
            using var conexion = new SqlConnection(InfoSQL.Conexion);
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
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();

            var sqlUsuario = @"SELECT 
                                u.UsuarioID as Id,
                                u.Nombre,
                                u.Email,
                                u.Password,
                                u.Pais,
                                u.Telefono,
                                u.PersonaID,
                                u.EmpresaID,
                                u.MonedaID,
                                u.Activo,
                                u.FechaCreacion,
                                u.FechaModificacion,
                                u.UsuarioCreadorID,
                                u.UsuarioModificadorID,
                                m.MonedaID as Moneda_Id,
                                m.Codigo as Moneda_Codigo,
                                m.Nombre as Moneda_Nombre
                              FROM Usuario u
                              LEFT JOIN Moneda m ON u.MonedaID = m.MonedaID
                              WHERE u.UsuarioID = @UsuarioID;";
            var sqlRoles = @"SELECT r.RolID as Id, r.Nombre FROM Rol r
                             INNER JOIN UsuarioRol ur ON ur.RolID = r.RolID
                             WHERE ur.UsuarioID = @UsuarioID;";

            var usuarioData = await conexion.QuerySingleOrDefaultAsync(sqlUsuario, new { UsuarioID = usuario.Id });
            if (usuarioData == null)
                return new DTO<Usuario> { Correcto = false, Mensaje = "Usuario no encontrado" };

            var usuarioEncontrado = new Usuario
            {
                Id = usuarioData.Id,
                Nombre = usuarioData.Nombre,
                Email = usuarioData.Email,
                Password = usuarioData.Password,
                Pais = usuarioData.Pais,
                Telefono = usuarioData.Telefono,
                PersonaId = usuarioData.PersonaID,
                EmpresaId = usuarioData.EmpresaID,
                MonedaId = usuarioData.MonedaID,
                Activo = usuarioData.Activo,
                FechaCreacion = usuarioData.FechaCreacion,
                FechaModificacion = usuarioData.FechaModificacion,
                UsuarioCreador = usuarioData.UsuarioCreadorID,
                UsuarioModificador = usuarioData.UsuarioModificadorID
            };

            // Mapear la moneda si existe
            if (usuarioData.Moneda_Id != null)
            {
                usuarioEncontrado.Moneda = new Moneda
                {
                    Id = usuarioData.Moneda_Id,
                    Codigo = usuarioData.Moneda_Codigo,
                    Nombre = usuarioData.Moneda_Nombre
                };
            }

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
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();

            var sqlUsuario = @"SELECT 
                                UsuarioID as Id,
                                Nombre,
                                Email,
                                Password,
                                Pais,
                                Telefono,
                                PersonaID,
                                EmpresaID,
                                MonedaID,
                                Activo,
                                FechaCreacion,
                                FechaModificacion,
                                UsuarioCreadorID,
                                UsuarioModificadorID
                              FROM Usuario WHERE Email = @Email;";
            var sqlRoles = @"SELECT r.RolID as Id, r.Nombre FROM Rol r
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
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();

            var sqlUsuarios = @"SELECT 
                                 u.UsuarioID as Id,
                                 u.Nombre,
                                 u.Email,
                                 u.Password,
                                 u.Pais,
                                 u.Telefono,
                                 u.PersonaID,
                                 u.EmpresaID,
                                 u.MonedaID,
                                 u.Activo,
                                 u.FechaCreacion,
                                 u.FechaModificacion,
                                 u.UsuarioCreadorID,
                                 u.UsuarioModificadorID,
                                 m.MonedaID as Moneda_Id,
                                 m.Codigo as Moneda_Codigo,
                                 m.Nombre as Moneda_Nombre
                               FROM Usuario u
                               LEFT JOIN Moneda m ON u.MonedaID = m.MonedaID;";
            var sqlTodosRoles = @"SELECT ur.UsuarioID, r.RolID as Id, r.Nombre 
                                  FROM UsuarioRol ur
                                  INNER JOIN Rol r ON ur.RolID = r.RolID;";

            var usuariosData = (await conexion.QueryAsync(sqlUsuarios)).ToList();
            var usuarioRoles = (await conexion.QueryAsync<dynamic>(sqlTodosRoles)).ToList();

            var usuarios = new List<Usuario>();

            foreach (var userData in usuariosData)
            {
                var usuario = new Usuario
                {
                    Id = userData.Id,
                    Nombre = userData.Nombre,
                    Email = userData.Email,
                    Password = userData.Password,
                    Pais = userData.Pais,
                    Telefono = userData.Telefono,
                    PersonaId = userData.PersonaID,
                    EmpresaId = userData.EmpresaID,
                    MonedaId = userData.MonedaID,
                    Activo = userData.Activo,
                    FechaCreacion = userData.FechaCreacion,
                    FechaModificacion = userData.FechaModificacion,
                    UsuarioCreador = userData.UsuarioCreadorID,
                    UsuarioModificador = userData.UsuarioModificadorID
                };

                // Mapear la moneda si existe
                if (userData.Moneda_Id != null)
                {
                    usuario.Moneda = new Moneda
                    {
                        Id = userData.Moneda_Id,
                        Codigo = userData.Moneda_Codigo,
                        Nombre = userData.Moneda_Nombre
                    };
                }

                // Asociar roles al usuario
                var rolesDelUsuario = usuarioRoles
                    .Where(ur => ur.UsuarioID == usuario.Id)
                    .Select(ur => new Rol { Id = ur.Id, Nombre = ur.Nombre })
                    .ToList();

                usuario.Roles = rolesDelUsuario;
                usuarios.Add(usuario);
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
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();

            var sql = @"SELECT 
                            COUNT(*) OVER() AS TotalItems,
                            u.UsuarioID as Id,
                            u.Nombre,
                            u.Email,
                            u.Password,
                            u.Pais,
                            u.Telefono,
                            u.PersonaID,
                            u.EmpresaID,
                            u.MonedaID,
                            u.Activo,
                            u.FechaCreacion,
                            u.FechaModificacion,
                            u.UsuarioCreadorID,
                            u.UsuarioModificadorID,
                            m.MonedaID as Moneda_Id,
                            m.Codigo as Moneda_Codigo,
                            m.Nombre as Moneda_Nombre
                        FROM Usuario u
                        LEFT JOIN Moneda m ON u.MonedaID = m.MonedaID
                        WHERE 
                            (@FiltroBusqueda IS NULL OR u.Nombre LIKE '%' + @FiltroBusqueda + '%' OR u.Email LIKE '%' + @FiltroBusqueda + '%')
                        ORDER BY
                            CASE WHEN @OrdenarPor = 'Nombre' THEN u.Nombre END,
                            CASE WHEN @OrdenarPor = 'Email' THEN u.Email END
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

                var usuario = new Usuario
                {
                    Id = row.Id,
                    Nombre = row.Nombre,
                    Email = row.Email,
                    Password = row.Password,
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
                };

                // Mapear la moneda si existe
                if (row.Moneda_Id != null)
                {
                    usuario.Moneda = new Moneda
                    {
                        Id = row.Moneda_Id,
                        Codigo = row.Moneda_Codigo,
                        Nombre = row.Moneda_Nombre
                    };
                }

                usuarios.Add(usuario);
            }

            // Obtener roles para todos los usuarios paginados
            if (usuarios.Any())
            {
                var usuarioIds = usuarios.Select(u => u.Id).ToList();
                var sqlRoles = @"SELECT ur.UsuarioID, r.RolID as Id, r.Nombre 
                                 FROM UsuarioRol ur
                                 INNER JOIN Rol r ON ur.RolID = r.RolID
                                 WHERE ur.UsuarioID IN @UsuarioIds;";

                var rolesData = await conexion.QueryAsync<dynamic>(sqlRoles, new { UsuarioIds = usuarioIds });

                // Asociar roles a cada usuario
                foreach (var usuario in usuarios)
                {
                    var rolesDelUsuario = rolesData
                        .Where(r => r.UsuarioID == usuario.Id)
                        .Select(r => new Rol { Id = r.Id, Nombre = r.Nombre })
                        .ToList();

                    usuario.Roles = rolesDelUsuario;
                }
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
