using AdmIn.Data.Utilitarios;
using AdmIn.Data.Entidades;
using AdmIn.Common;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AdmIn.Data.Repositorios
{
    public class Rep_USUARIO : IRepoBase<USUARIO>
    {
        public async Task<DTO<USUARIO>> Crear(USUARIO usuario)
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

        public async Task<DTO<USUARIO>> Actualizar(USUARIO usuario)
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

        public async Task<DTO<bool>> Eliminar(USUARIO usuario)
        {
            return MiSqlHelper.EjecutarComando("sp_Usuario_Eliminar",
                comando => comando.Parameters.AddWithValue("@USU_ID", usuario.USU_ID),
                comando =>
                {
                    comando.ExecuteNonQuery();
                    return true;
                });
        }

        public async Task<DTO<USUARIO>> Obtener_por_id(USUARIO entidad)
        {
            return MiSqlHelper.EjecutarComando("sp_Usuario_ObtenerPorId",
                comando => comando.Parameters.AddWithValue("@USU_ID", entidad.USU_ID),
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

        public async Task<DTO<USUARIO>> Obtener_por_email(string email)
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

        public async Task<DTO<IEnumerable<USUARIO>>> Obtener_todos()
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
                    return lista.AsEnumerable();
                });
        }

        public async Task<DTO<Items_pagina<USUARIO>>> Obtener_paginado(Filtros_paginado filtros)
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
                                    FROM 
                                        (-- CONSULTA QUE AGRUPA LAS TABLAS RELACIONADAS PARA HACER EL FILTRO DEL WHERE
                                            SELECT DISTINCT USUARIO.*
				                            FROM
					                            USUARIO
					                            JOIN USUARIO_ROL ON USUARIO.USU_ID = USUARIO_ROL.USU_ID
					                            JOIN ROL ON USUARIO_ROL.ROL_ID = ROL.ROL_ID
					                            JOIN ROL_PERMISO ON ROL.ROL_ID = ROL_PERMISO.ROL_ID
					                            JOIN PERMISO ON ROL_PERMISO.PERM_ID = PERMISO.PERM_ID   
                                            {whereClause}
                                        ) AUX
                                ) AS PaginatedQuery
                                WHERE RowNum > {filtros.Skip}
                                AND RowNum <= {filtros.Skip + filtros.Top}";

                // Consulta SQL para contar registros totales aplicando los mismos filtros
                string countQuery = $@"
                                    SELECT COUNT(*)
                                    FROM 
                                       (-- CONSULTA QUE AGRUPA LAS TABLAS RELACIONADAS PARA HACER EL FILTRO DEL WHERE
                                            SELECT DISTINCT USUARIO.*
				                            FROM
					                            USUARIO
					                            JOIN USUARIO_ROL ON USUARIO.USU_ID = USUARIO_ROL.USU_ID
					                            JOIN ROL ON USUARIO_ROL.ROL_ID = ROL.ROL_ID
					                            JOIN ROL_PERMISO ON ROL.ROL_ID = ROL_PERMISO.ROL_ID
					                            JOIN PERMISO ON ROL_PERMISO.PERM_ID = PERMISO.PERM_ID   
                                            {whereClause}
                                        ) AUX";

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
