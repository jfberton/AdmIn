using AdmIn.Data.Utilitarios;
using AdmIn.Data.Entidades;
using AdmIn.Common;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Security;

namespace AdmIn.Data.Repositorios
{
    public class Rep_PERMISO : IRepoBase<PERMISO>
    {
        public async Task<DTO<PERMISO>> Crear(PERMISO permiso)
        {
            return MiSqlHelper.EjecutarComando("sp_Permiso_Insertar",
                comando =>
                {
                    comando.Parameters.AddWithValue("@PERM_NOMBRE", permiso.PERM_NOMBRE);
                    var nuevoId = new SqlParameter("@NuevoId", SqlDbType.Int) { Direction = ParameterDirection.Output };
                    comando.Parameters.Add(nuevoId);
                },
                comando =>
                {
                    comando.ExecuteNonQuery();
                    permiso.PERM_ID = (int)comando.Parameters["@NuevoId"].Value;
                    return permiso;
                });
        }

        public async Task<DTO<PERMISO>> Actualizar(PERMISO permiso)
        {
            return MiSqlHelper.EjecutarComando("sp_Permiso_Actualizar",
                comando =>
                {
                    comando.Parameters.AddWithValue("@PERM_ID", permiso.PERM_ID);
                    comando.Parameters.AddWithValue("@PERM_NOMBRE", permiso.PERM_NOMBRE);
                },
                comando =>
                {
                    comando.ExecuteNonQuery();
                    return permiso;
                });
        }

        public async Task<DTO<bool>> Eliminar(PERMISO permiso)
        {
            return MiSqlHelper.EjecutarComando("sp_Permiso_Eliminar",
                comando => comando.Parameters.AddWithValue("@PERM_ID", permiso.PERM_ID),
                comando =>
                {
                    comando.ExecuteNonQuery();
                    return true;
                });
        }

        public async Task<DTO<PERMISO?>> Obtener_por_id(PERMISO permiso)
        {
            return MiSqlHelper.EjecutarComando("sp_Permiso_ObtenerPorId",
                comando => comando.Parameters.AddWithValue("@PERM_ID", permiso.PERM_ID),
                comando =>
                {
                    using (var lector = comando.ExecuteReader())
                    {
                        if (lector.Read())
                        {
                            return new PERMISO
                            {
                                PERM_ID = lector.GetInt32(0),
                                PERM_NOMBRE = lector.GetString(1)
                            };
                        }
                        return null;
                    }
                });
        }

        public async Task<DTO<IEnumerable<PERMISO>>> Obtener_todos()
        {
            return MiSqlHelper.EjecutarComando("sp_Permiso_ObtenerTodos",
                null,
                comando =>
                {
                    var lista = new List<PERMISO>();
                    using (var lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            lista.Add(new PERMISO
                            {
                                PERM_ID = lector.GetInt32(0),
                                PERM_NOMBRE = lector.GetString(1)
                            });
                        }
                    }
                    return lista.AsEnumerable();
                });
        }

        public async Task<DTO<Items_pagina<PERMISO>>> Obtener_paginado(Filtros_paginado filtros)
        {
            try
            {
                string orderByClause = string.IsNullOrEmpty(filtros.OrderBy) ? "ORDER BY PERM_ID" : $"ORDER BY {filtros.OrderBy}";
                string whereClause = string.IsNullOrEmpty(filtros.Filter) ? "" : $"WHERE {filtros.WhereClause()}";

                string query = $@"
                                SELECT * FROM (
                                    SELECT *, ROW_NUMBER() OVER ({orderByClause}) AS RowNum
                                    FROM PERMISO
                                    {whereClause}
                                ) AS PaginatedQuery
                                WHERE RowNum > {filtros.Skip}
                                AND RowNum <= {filtros.Skip + filtros.Top}";

                string countQuery = $@"
                                    SELECT COUNT(*)
                                    FROM PERMISO
                                    {whereClause}";

                DTO<List<PERMISO>> resultado = MiSqlHelper.EjecutarComando(
                    query,
                    null,
                    comando =>
                    {
                        var permisos = new List<PERMISO>();
                        using (var lector = comando.ExecuteReader())
                        {
                            while (lector.Read())
                            {
                                permisos.Add(new PERMISO
                                {
                                    PERM_ID = lector.GetInt32(0),
                                    PERM_NOMBRE = lector.GetString(1)
                                });
                            }
                        }
                        return permisos;
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

                var itemsPagina = new Items_pagina<PERMISO>
                {
                    Items = resultado.Datos,
                    Total_items = totalRegistros.Datos
                };

                return new DTO<Items_pagina<PERMISO>>
                {
                    Datos = itemsPagina,
                    Correcto = true,
                    Mensaje = "Permisos obtenidos correctamente"
                };
            }
            catch (Exception ex)
            {
                return new DTO<Items_pagina<PERMISO>>
                {
                    Datos = null,
                    Correcto = false,
                    Mensaje = $"Error al obtener los permisos: {ex.Message}"
                };
            }
        }
    }
}


