using AdmIn.Data.Utilitarios;
using AdmIn.Data.Entidades;
using AdmIn.Common;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AdmIn.Data.Repositorios
{
    public class Rep_ROL : IRepoBase<ROL>
    {
        public async Task<DTO<ROL>> Crear(ROL rol)
        {
            if (string.IsNullOrEmpty(rol.ROL_NOMBRE))
            {
                return new DTO<ROL>
                {
                    Correcto = false,
                    Mensaje = "El nombre del rol no puede estar vacío."
                };
            }

            return MiSqlHelper.EjecutarComando("sp_Rol_Insertar",
                comando =>
                {
                    comando.Parameters.AddWithValue("@ROL_NOMBRE", rol.ROL_NOMBRE);
                },
                comando =>
                {
                    comando.ExecuteNonQuery();
                    return rol;
                });
        }

        public async Task<DTO<ROL>> Actualizar(ROL rol)
        {
            if (rol.ROL_ID == 0 || string.IsNullOrEmpty(rol.ROL_NOMBRE))
            {
                return new DTO<ROL>
                {
                    Correcto = false,
                    Mensaje = "El ID del rol no puede ser cero y el nombre no puede estar vacío."
                };
            }

            return MiSqlHelper.EjecutarComando("sp_Rol_Actualizar",
                comando =>
                {
                    comando.Parameters.AddWithValue("@ROL_ID", rol.ROL_ID);
                    comando.Parameters.AddWithValue("@ROL_NOMBRE", rol.ROL_NOMBRE);
                },
                comando =>
                {
                    comando.ExecuteNonQuery();
                    return rol;
                });
        }

        public async Task<DTO<bool>> Eliminar(ROL entidad)
        {
            return MiSqlHelper.EjecutarComando("sp_Rol_Eliminar",
                comando => comando.Parameters.AddWithValue("@ROL_ID", entidad.ROL_ID),
                comando =>
                {
                    comando.ExecuteNonQuery();
                    return true;
                });
        }

        public async Task<DTO<ROL?>> Obtener_por_id(ROL entidad)
        {
            return MiSqlHelper.EjecutarComando("sp_Rol_ObtenerPorId",
                comando => comando.Parameters.AddWithValue("@ROL_ID", entidad.ROL_ID),
                comando =>
                {
                    using (var lector = comando.ExecuteReader())
                    {
                        if (lector.Read())
                        {
                            return new ROL
                            {
                                ROL_ID = lector.GetInt32(0),
                                ROL_NOMBRE = lector.GetString(1)
                            };
                        }
                        return null;
                    }
                });
        }

        public async Task<DTO<IEnumerable<ROL>>> Obtener_todos()
        {
            return MiSqlHelper.EjecutarComando("sp_Rol_ObtenerTodos",
                null,
                comando =>
                {
                    var lista = new List<ROL>();
                    using (var lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            lista.Add(new ROL
                            {
                                ROL_ID = lector.GetInt32(0),
                                ROL_NOMBRE = lector.GetString(1)
                            });
                        }
                    }
                    return lista.AsEnumerable();
                });
        }

        public async Task<DTO<Items_pagina<ROL>>> Obtener_paginado(Filtros_paginado filtros)
        {
            try
            {
                // Construcción de la cláusula ORDER BY
                string orderByClause = string.IsNullOrEmpty(filtros.OrderBy) ? "ORDER BY ROL_ID" : $"ORDER BY {filtros.OrderBy}";

                // Construcción del filtro WHERE dinámico
                string whereClause = string.IsNullOrEmpty(filtros.Filter) ? "" : $"WHERE {filtros.WhereClause()}";

                // Consulta SQL paginada
                string query = $@"
                                SELECT * FROM (
                                    SELECT *, ROW_NUMBER() OVER ({orderByClause}) AS RowNum
                                    FROM 
                                        (-- CONSULTA QUE AGRUPA LAS TABLAS RELACIONADAS PARA HACER EL FILTRO DEL WHERE
                                            SELECT DISTINCT ROL.*
                                            FROM
                                                ROL
                                                LEFT JOIN ROL_PERMISO ON ROL.ROL_ID = ROL_PERMISO.ROL_ID
                                                LEFT JOIN PERMISO ON ROL_PERMISO.PERM_ID = PERMISO.PERM_ID
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
                                            SELECT DISTINCT ROL.*
                                            FROM
                                                ROL
                                                LEFT JOIN ROL_PERMISO ON ROL.ROL_ID = ROL_PERMISO.ROL_ID
                                                LEFT JOIN PERMISO ON ROL_PERMISO.PERM_ID = PERMISO.PERM_ID
                                            {whereClause}
                                        ) AUX";

                // Ejecutamos la consulta principal para obtener los roles paginados
                DTO<List<ROL>> resultado = MiSqlHelper.EjecutarComando(
                    query,
                    null,
                    comando =>
                    {
                        var roles = new List<ROL>();
                        using (var lector = comando.ExecuteReader())
                        {
                            while (lector.Read())
                            {
                                roles.Add(new ROL
                                {
                                    ROL_ID = lector.GetInt32(0),
                                    ROL_NOMBRE = lector.GetString(1)
                                });
                            }
                        }
                        return roles;
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

                // Creamos el objeto de la página de roles
                var itemsPagina = new Items_pagina<ROL>
                {
                    Items = resultado.Datos,
                    Total_items = totalRegistros.Datos
                };

                return new DTO<Items_pagina<ROL>>
                {
                    Datos = itemsPagina,
                    Correcto = true,
                    Mensaje = "Roles de la página obtenidos correctamente"
                };
            }
            catch (Exception ex)
            {
                return new DTO<Items_pagina<ROL>>
                {
                    Datos = null,
                    Correcto = false,
                    Mensaje = $"Error al obtener la lista de roles de la página: {ex.Message}"
                };
            }
        }


    }
}

