using AdmIn.Data.Utilitarios;
using AdmIn.Data.Entidades;
using AdmIn.Common;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AdmIn.Data.Repositorios
{
    public class Rep_ROL_PERMISO : IRepoBase<ROL_PERMISO>
    {
        public async Task<DTO<ROL_PERMISO>> Crear(ROL_PERMISO rolPermiso)
        {
            if (rolPermiso.ROL_ID == 0 || rolPermiso.PERM_ID == 0)
            {
                return new DTO<ROL_PERMISO>
                {
                    Correcto = false,
                    Mensaje = "El ID del rol y el permiso no pueden ser cero."
                };
            }

            return MiSqlHelper.EjecutarComando("sp_RolPermiso_Insertar",
                comando =>
                {
                    comando.Parameters.AddWithValue("@ROL_ID", rolPermiso.ROL_ID);
                    comando.Parameters.AddWithValue("@PERM_ID", rolPermiso.PERM_ID);
                },
                comando =>
                {
                    comando.ExecuteNonQuery();
                    return rolPermiso;
                });
        }

        public async Task<DTO<ROL_PERMISO>> Actualizar(ROL_PERMISO rolPermiso)
        {
            return new DTO<ROL_PERMISO>
            {
                Correcto = false,
                Mensaje = "No es posible actualizar una relación entre rol y permiso."
            };
        }

        public async Task<DTO<bool>> Eliminar(ROL_PERMISO rol_permiso)
        {
            return MiSqlHelper.EjecutarComando("sp_RolPermiso_Eliminar",
                comando =>
                {
                    comando.Parameters.AddWithValue("@ROL_ID", rol_permiso.ROL_ID);
                    comando.Parameters.AddWithValue("@PERM_ID", rol_permiso.PERM_ID);
                },
                comando =>
                {
                    comando.ExecuteNonQuery();
                    return true;
                });
        }

        public async Task<DTO<ROL_PERMISO?>> Obtener_por_id(ROL_PERMISO entidad)
        {
            return MiSqlHelper.EjecutarComando("sp_RolPermiso_ObtenerPorId",
                comando =>
                {
                    comando.Parameters.AddWithValue("@ROL_ID", entidad.ROL_ID);
                    comando.Parameters.AddWithValue("@PERM_ID", entidad.PERM_ID);
                },
                comando =>
                {
                    using (var lector = comando.ExecuteReader())
                    {
                        if (lector.Read())
                        {
                            return new ROL_PERMISO
                            {
                                ROL_ID = lector.GetInt32(0),
                                PERM_ID = lector.GetInt32(1)
                            };
                        }
                        return null;
                    }
                });
        }

        public async Task<DTO<IEnumerable<ROL_PERMISO>>> Obtener_por_rol(int rol_id)
        {
            return MiSqlHelper.EjecutarComando("sp_RolPermiso_ObtenerPorRol",
                 comando => comando.Parameters.AddWithValue("@ROL_ID", rol_id),
                comando =>
                {
                    var lista = new List<ROL_PERMISO>();
                    using (var lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            lista.Add(new ROL_PERMISO
                            {
                                ROL_ID = lector.GetInt32(0),
                                PERM_ID = lector.GetInt32(1)
                            });
                        }
                    }
                    return lista.AsEnumerable();
                });
        }


        public async Task<DTO<IEnumerable<ROL_PERMISO>>> Obtener_todos()
        {
            return MiSqlHelper.EjecutarComando("sp_RolPermiso_ObtenerTodos",
                null,
                comando =>
                {
                    var lista = new List<ROL_PERMISO>();
                    using (var lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            lista.Add(new ROL_PERMISO
                            {
                                ROL_ID = lector.GetInt32(0),
                                PERM_ID = lector.GetInt32(1)
                            });
                        }
                    }
                    return lista.AsEnumerable();
                });
        }

        public async Task<DTO<Items_pagina<ROL_PERMISO>>> Obtener_paginado(Filtros_paginado filtros)
        {
            try
            {
                // Construcción de la cláusula ORDER BY
                string orderByClause = string.IsNullOrEmpty(filtros.OrderBy) ? "ORDER BY ROL_PERMISO_ID" : $"ORDER BY {filtros.OrderBy}";

                // Construcción del filtro WHERE dinámico
                string whereClause = string.IsNullOrEmpty(filtros.Filter) ? "" : $"WHERE {filtros.WhereClause()}";

                // Consulta SQL paginada
                string query = $@"
                                SELECT * FROM (
                                    SELECT *, ROW_NUMBER() OVER ({orderByClause}) AS RowNum
                                    FROM 
                                        (-- CONSULTA QUE AGRUPA LAS TABLAS RELACIONADAS PARA HACER EL FILTRO DEL WHERE
                                            SELECT DISTINCT RP.*
                                            FROM
                                                ROL_PERMISO
                                                INNER JOIN PERMISO ON ROL_PERMISO.PERM_ID = PERMISO.PERM_ID
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
                                            SELECT DISTINCT RP.*
                                            FROM
                                                ROL_PERMISO
                                                INNER JOIN PERMISO ON ROL_PERMISO.PERM_ID = PERMISO.PERM_ID
                                            {whereClause}
                                        ) AUX";

                // Ejecutamos la consulta principal para obtener los rol-permiso paginados
                DTO<List<ROL_PERMISO>> resultado = MiSqlHelper.EjecutarComando(
                    query,
                    null,
                    comando =>
                    {
                        var rolesPermisos = new List<ROL_PERMISO>();
                        using (var lector = comando.ExecuteReader())
                        {
                            while (lector.Read())
                            {
                                rolesPermisos.Add(new ROL_PERMISO
                                {
                                    ROL_ID = lector.GetInt32(0),
                                    PERM_ID = lector.GetInt32(1)
                                });
                            }
                        }
                        return rolesPermisos;
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

                // Creamos el objeto de la página de rol-permiso
                var itemsPagina = new Items_pagina<ROL_PERMISO>
                {
                    Items = resultado.Datos,
                    Total_items = totalRegistros.Datos
                };

                return new DTO<Items_pagina<ROL_PERMISO>>
                {
                    Datos = itemsPagina,
                    Correcto = true,
                    Mensaje = "Roles-Permisos de la página obtenidos correctamente"
                };
            }
            catch (Exception ex)
            {
                return new DTO<Items_pagina<ROL_PERMISO>>
                {
                    Datos = null,
                    Correcto = false,
                    Mensaje = $"Error al obtener la lista de roles-permisos de la página: {ex.Message}"
                };
            }
        }
    }
}


