using AdmIn.Data.Utilitarios;
using AdmIn.Data.Entidades;
using AdmIn.Common;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AdmIn.Data.Repositorios
{
    public class Rep_USUARIO
    {
        public DTO<USUARIO> Insertar(USUARIO usuario)
        {
            if (string.IsNullOrWhiteSpace(usuario.USU_NOMBRE) ||
                string.IsNullOrWhiteSpace(usuario.USU_PASSWORD) ||
                string.IsNullOrWhiteSpace(usuario.USU_EMAIL))
            {
                return new DTO<USUARIO>
                {
                    Correcto = false,
                    Mensaje = "El nombre, la contraseña y el correo del usuario no pueden estar vacíos."
                };
            }

            return MiSqlHelper.EjecutarComando("sp_Usuario_Insertar",
                comando =>
                {
                    comando.Parameters.AddWithValue("@USU_NOMBRE", usuario.USU_NOMBRE);
                    comando.Parameters.AddWithValue("@USU_PASSWORD", usuario.USU_PASSWORD);
                    comando.Parameters.AddWithValue("@USU_EMAIL", usuario.USU_EMAIL);
                    comando.Parameters.AddWithValue("@USU_FECHA_CREACION", usuario.USU_FECHA_CREACION);
                    comando.Parameters.AddWithValue("@USU_ACTIVO", usuario.USU_ACTIVO);
                    var nuevoId = new SqlParameter("@NuevoId", SqlDbType.Int) { Direction = ParameterDirection.Output };
                    comando.Parameters.Add(nuevoId);
                },
                comando =>
                {
                    comando.ExecuteNonQuery();
                    usuario.USU_ID = (int)comando.Parameters["@NuevoId"].Value;
                    return usuario;
                });
        }

        public DTO<USUARIO> Actualizar(USUARIO usuario)
        {
            if (string.IsNullOrWhiteSpace(usuario.USU_NOMBRE) ||
                string.IsNullOrWhiteSpace(usuario.USU_PASSWORD) ||
                string.IsNullOrWhiteSpace(usuario.USU_EMAIL))
            {
                return new DTO<USUARIO>
                {
                    Correcto = false,
                    Mensaje = "El nombre, la contraseña y el correo del usuario no pueden estar vacíos."
                };
            }

            return MiSqlHelper.EjecutarComando("sp_Usuario_Actualizar",
                comando =>
                {
                    comando.Parameters.AddWithValue("@USU_ID", usuario.USU_ID);
                    comando.Parameters.AddWithValue("@USU_NOMBRE", usuario.USU_NOMBRE);
                    comando.Parameters.AddWithValue("@USU_PASSWORD", usuario.USU_PASSWORD);
                    comando.Parameters.AddWithValue("@USU_EMAIL", usuario.USU_EMAIL);
                    comando.Parameters.AddWithValue("@USU_ACTIVO", usuario.USU_ACTIVO);
                },
                comando =>
                {
                    comando.ExecuteNonQuery();
                    return usuario;
                });
        }

        public DTO<bool> Eliminar(int id)
        {
            return MiSqlHelper.EjecutarComando("sp_Usuario_Eliminar",
                comando => comando.Parameters.AddWithValue("@USU_ID", id),
                comando =>
                {
                    comando.ExecuteNonQuery();
                    return true;
                });
        }

        public DTO<USUARIO> Obtener_por_Id(int id)
        {
            return MiSqlHelper.EjecutarComando("sp_Usuario_ObtenerPorId",
                comando => comando.Parameters.AddWithValue("@USU_ID", id),
                comando =>
                {
                    using (var lector = comando.ExecuteReader())
                    {
                        if (lector.Read())
                        {
                            return new USUARIO
                            {
                                USU_ID = lector.GetInt32(0),
                                USU_NOMBRE = lector.GetString(1),
                                USU_PASSWORD = lector.GetString(2),
                                USU_EMAIL = lector.GetString(3),
                                USU_FECHA_CREACION = lector.GetDateTime(4),
                                USU_ACTIVO = lector.GetBoolean(5)
                            };
                        }
                        return null;
                    }
                });
        }

        public DTO<USUARIO> Obtener_por_Email(string email)
        {
            return MiSqlHelper.EjecutarComando("sp_Usuario_ObtenerPorEmail",
                comando => comando.Parameters.AddWithValue("@USU_EMAIL", email),
                comando =>
                {
                    using (var lector = comando.ExecuteReader())
                    {
                        if (lector.Read())
                        {
                            return new USUARIO
                            {
                                USU_ID = lector.GetInt32(0),
                                USU_NOMBRE = lector.GetString(1),
                                USU_PASSWORD = lector.GetString(2),
                                USU_EMAIL = lector.GetString(3),
                                USU_FECHA_CREACION = lector.GetDateTime(4),
                                USU_ACTIVO = lector.GetBoolean(5)
                            };
                        }
                        return null;
                    }
                });
        }

        public DTO<List<USUARIO>> Obtener_todos()
        {
            return MiSqlHelper.EjecutarComando("sp_Usuario_ObtenerTodos",
                null,
                comando =>
                {
                    var lista = new List<USUARIO>();
                    using (var lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            lista.Add(new USUARIO
                            {
                                USU_ID = lector.GetInt32(0),
                                USU_NOMBRE = lector.GetString(1),
                                USU_PASSWORD = lector.GetString(2),
                                USU_EMAIL = lector.GetString(3),
                                USU_FECHA_CREACION = lector.GetDateTime(4),
                                USU_ACTIVO = lector.GetBoolean(5)
                            });
                        }
                    }
                    return lista;
                });
        }

        public DTO<Items_pagina<USUARIO>> Obtener_lista_pagina_usuarios(Filtros_paginado filtros)
        {
            try
            {
                // Construcción de la cláusula ORDER BY
                string orderByClause = string.IsNullOrEmpty(filtros.OrderBy) ? "ORDER BY USU_FECHA_CREACION" : $"ORDER BY {filtros.OrderBy}";

                // Construcción del filtro WHERE dinámico
                string whereClause = string.IsNullOrEmpty(filtros.Filter) ? "" : $"WHERE {filtros.WhereClause()}";

                // Consulta SQL paginada
                string query = $@"
                                SELECT * FROM (
                                    SELECT *, ROW_NUMBER() OVER ({orderByClause}) AS RowNum
                                    FROM USUARIO
                                    {whereClause}
                                ) AS PaginatedQuery
                                WHERE RowNum > {filtros.Skip}
                                AND RowNum <= {filtros.Skip + filtros.Top}";

                // Consulta SQL para contar registros totales aplicando los mismos filtros
                string countQuery = $@"
                                    SELECT COUNT(*)
                                    FROM USUARIO
                                    {whereClause}";

                // Ejecutamos la consulta principal para obtener los usuarios paginados
                DTO<List<USUARIO>> resultado = MiSqlHelper.EjecutarComando(
                    query,
                    null,
                    comando =>
                    {
                        var usuarios = new List<USUARIO>();
                        using (var lector = comando.ExecuteReader())
                        {
                            while (lector.Read())
                            {
                                usuarios.Add(new USUARIO
                                {
                                    USU_ID = lector.GetInt32(0),
                                    USU_NOMBRE = lector.GetString(1),
                                    USU_PASSWORD = lector.GetString(2),
                                    USU_EMAIL = lector.GetString(3),
                                    USU_FECHA_CREACION = lector.GetDateTime(4),
                                    USU_ACTIVO = lector.GetBoolean(5)
                                });
                            }
                        }
                        return usuarios;
                    },
                    CommandType.Text
                );

                // Ejecutamos la consulta para contar los registros filtrados
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

                // Creamos el objeto de la página de usuarios
                var itemsPagina = new Items_pagina<USUARIO>
                {
                    Items = resultado.Datos,
                    Total_items = totalRegistros.Datos
                };

                return new DTO<Items_pagina<USUARIO>>
                {
                    Datos = itemsPagina,
                    Correcto = true,
                    Mensaje = "Usuarios de la página obtenidos correctamente"
                };
            }
            catch (Exception ex)
            {
                return new DTO<Items_pagina<USUARIO>>
                {
                    Datos = null,
                    Correcto = false,
                    Mensaje = $"Error al obtener la lista de usuarios de la página: {ex.Message}"
                };
            }
        }

    }
}
