using AdmIn.Common;
using AdmIn.Data.Entidades;
using AdmIn.Data.Utilitarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdmIn.Data.Repositorios
{
    public class R_Usuario : IRepoBase<T_Usuario>
    {
        public async Task<DTO<T_Usuario>> Crear(T_Usuario usuario)
        {
            return MiSqlHelper.EjecutarComando("sp_Usuario_Insertar",
                comando =>
                {
                    comando.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                    comando.Parameters.AddWithValue("@Email", usuario.Email);
                    comando.Parameters.AddWithValue("@Password", usuario.Password);
                    comando.Parameters.AddWithValue("@Pais", usuario.Pais);
                    comando.Parameters.AddWithValue("@Telefono", usuario.Telefono);
                    comando.Parameters.AddWithValue("@PersonaID", (object?)usuario.PersonaID ?? DBNull.Value);
                    comando.Parameters.AddWithValue("@EmpresaID", (object?)usuario.EmpresaID ?? DBNull.Value);
                    comando.Parameters.AddWithValue("@MonedaID", usuario.MonedaID);
                    comando.Parameters.AddWithValue("@Activo", usuario.Activo);
                },
                comando =>
                {
                    comando.ExecuteNonQuery();
                    return usuario;
                });
        }

        public async Task<DTO<T_Usuario>> Actualizar(T_Usuario usuario)
        {
            return MiSqlHelper.EjecutarComando("sp_Usuario_Actualizar",
                comando =>
                {
                    comando.Parameters.AddWithValue("@UsuarioID", usuario.UsuarioID);
                    comando.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                    comando.Parameters.AddWithValue("@Email", usuario.Email);
                    comando.Parameters.AddWithValue("@Password", usuario.Password);
                    comando.Parameters.AddWithValue("@Pais", usuario.Pais);
                    comando.Parameters.AddWithValue("@Telefono", usuario.Telefono);
                    comando.Parameters.AddWithValue("@PersonaID", (object?)usuario.PersonaID ?? DBNull.Value);
                    comando.Parameters.AddWithValue("@EmpresaID", (object?)usuario.EmpresaID ?? DBNull.Value);
                    comando.Parameters.AddWithValue("@MonedaID", usuario.MonedaID);
                    comando.Parameters.AddWithValue("@Activo", usuario.Activo);
                },
                comando =>
                {
                    comando.ExecuteNonQuery();
                    return usuario;
                });
        }

        public async Task<DTO<bool>> Eliminar(T_Usuario usuario)
        {
            return MiSqlHelper.EjecutarComando("sp_Usuario_Eliminar",
                comando => comando.Parameters.AddWithValue("@UsuarioID", usuario.UsuarioID),
                comando =>
                {
                    comando.ExecuteNonQuery();
                    return true;
                });
        }

        public async Task<DTO<T_Usuario?>> Obtener_por_id(T_Usuario usuario)
        {
            return MiSqlHelper.EjecutarComando("sp_Usuario_ObtenerPorId",
                comando => comando.Parameters.AddWithValue("@UsuarioID", usuario.UsuarioID),
                comando =>
                {
                    using (var lector = comando.ExecuteReader())
                    {
                        if (lector.Read())
                        {
                            return new T_Usuario
                            {
                                UsuarioID = lector.GetInt32(0),
                                Nombre = lector.GetString(1),
                                Email = lector.GetString(2),
                                Password = lector.GetString(3),
                                Pais = lector.GetString(4),
                                Telefono = lector.GetString(5),
                                PersonaID = lector.IsDBNull(6) ? null : lector.GetInt32(6),
                                EmpresaID = lector.IsDBNull(7) ? null : lector.GetInt32(7),
                                MonedaID = lector.GetInt32(8),
                                Activo = lector.GetBoolean(9),
                                FechaCreacion = lector.GetDateTime(10),
                                FechaModificacion = lector.GetDateTime(11)
                            };
                        }
                        return null;
                    }
                });
        }

        public async Task<DTO<IEnumerable<T_Usuario>>> Obtener_todos()
        {
            return MiSqlHelper.EjecutarComando("sp_Usuario_ObtenerTodos",
                null,
                comando =>
                {
                    var lista = new List<T_Usuario>();
                    using (var lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            lista.Add(new T_Usuario
                            {
                                UsuarioID = lector.GetInt32(0),
                                Nombre = lector.GetString(1),
                                Email = lector.GetString(2),
                                Password = lector.GetString(3),
                                Pais = lector.GetString(4),
                                Telefono = lector.GetString(5),
                                PersonaID = lector.IsDBNull(6) ? null : lector.GetInt32(6),
                                EmpresaID = lector.IsDBNull(7) ? null : lector.GetInt32(7),
                                MonedaID = lector.GetInt32(8),
                                Activo = lector.GetBoolean(9),
                                FechaCreacion = lector.GetDateTime(10),
                                FechaModificacion = lector.GetDateTime(11)
                            });
                        }
                    }
                    return lista.AsEnumerable();
                });
        }

        public async Task<DTO<Items_pagina<T_Usuario>>> Obtener_paginado(Filtros_paginado filtros)
        {
            try
            {
                string orderByClause = string.IsNullOrEmpty(filtros.OrderBy) ? "ORDER BY UsuarioID" : $"ORDER BY {filtros.OrderBy}";
                string whereClause = string.IsNullOrEmpty(filtros.Filter) ? "" : $"WHERE {filtros.WhereClause()}";

                string query = $@"
                                SELECT * FROM (
                                    SELECT *, ROW_NUMBER() OVER ({orderByClause}) AS RowNum
                                    FROM T_Usuario
                                    {whereClause}
                                ) AS PaginatedQuery
                                WHERE RowNum > {filtros.Skip}
                                AND RowNum <= {filtros.Skip + filtros.Top}";

                string countQuery = $@"
                                    SELECT COUNT(*)
                                    FROM T_Usuario
                                    {whereClause}";

                DTO<List<T_Usuario>> resultado = MiSqlHelper.EjecutarComando(
                    query,
                    null,
                    comando =>
                    {
                        var usuarios = new List<T_Usuario>();
                        using (var lector = comando.ExecuteReader())
                        {
                            while (lector.Read())
                            {
                                usuarios.Add(new T_Usuario
                                {
                                    UsuarioID = lector.GetInt32(0),
                                    Nombre = lector.GetString(1),
                                    Email = lector.GetString(2),
                                    Password = lector.GetString(3),
                                    Pais = lector.GetString(4),
                                    Telefono = lector.GetString(5),
                                    PersonaID = lector.IsDBNull(6) ? null : lector.GetInt32(6),
                                    EmpresaID = lector.IsDBNull(7) ? null : lector.GetInt32(7),
                                    MonedaID = lector.GetInt32(8),
                                    Activo = lector.GetBoolean(9),
                                    FechaCreacion = lector.GetDateTime(10),
                                    FechaModificacion = lector.GetDateTime(11)
                                });
                            }
                        }
                        return usuarios;
                    },
                    CommandType.Text
                );

                DTO<int> totalRegistros = MiSqlHelper.EjecutarComando(
                    countQuery,
                    null,
                    comando => Convert.ToInt32(comando.ExecuteScalar()),
                    CommandType.Text
                );

                if (!resultado.Correcto || !totalRegistros.Correcto)
                {
                    throw new Exception($"{resultado.Mensaje} {totalRegistros.Mensaje}");
                }

                var itemsPagina = new Items_pagina<T_Usuario>
                {
                    Items = resultado.Datos,
                    Total_items = totalRegistros.Datos
                };

                return new DTO<Items_pagina<T_Usuario>>
                {
                    Datos = itemsPagina,
                    Correcto = true,
                    Mensaje = "Usuarios de la página obtenidos correctamente"
                };
            }
            catch (Exception ex)
            {
                return new DTO<Items_pagina<T_Usuario>>
                {
                    Datos = null,
                    Correcto = false,
                    Mensaje = $"Error al obtener la lista de usuarios de la página: {ex.Message}"
                };
            }
        }
    }
}