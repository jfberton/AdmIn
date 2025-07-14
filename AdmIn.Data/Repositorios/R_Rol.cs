using AdmIn.Common;
using AdmIn.Data.Entidades;
using AdmIn.Data.Utilitarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdmIn.Data.Repositorios
{
    public class R_Rol : IRepoBase<T_Rol>
    {
        public async Task<DTO<T_Rol>> Crear(T_Rol rol)
        {
            return MiSqlHelper.EjecutarComando("sp_Rol_Insertar",
                comando =>
                {
                    comando.Parameters.AddWithValue("@Nombre", rol.Nombre);
                },
                comando =>
                {
                    comando.ExecuteNonQuery();
                    return rol;
                });
        }

        public async Task<DTO<T_Rol>> Actualizar(T_Rol rol)
        {
            return MiSqlHelper.EjecutarComando("sp_Rol_Actualizar",
                comando =>
                {
                    comando.Parameters.AddWithValue("@RolID", rol.RolID);
                    comando.Parameters.AddWithValue("@Nombre", rol.Nombre);
                },
                comando =>
                {
                    comando.ExecuteNonQuery();
                    return rol;
                });
        }

        public async Task<DTO<bool>> Eliminar(T_Rol rol)
        {
            return MiSqlHelper.EjecutarComando("sp_Rol_Eliminar",
                comando => comando.Parameters.AddWithValue("@RolID", rol.RolID),
                comando =>
                {
                    comando.ExecuteNonQuery();
                    return true;
                });
        }

        public async Task<DTO<T_Rol?>> Obtener_por_id(T_Rol rol)
        {
            return MiSqlHelper.EjecutarComando("sp_Rol_ObtenerPorId",
                comando => comando.Parameters.AddWithValue("@RolID", rol.RolID),
                comando =>
                {
                    using (var lector = comando.ExecuteReader())
                    {
                        if (lector.Read())
                        {
                            return new T_Rol
                            {
                                RolID = lector.GetInt32(0),
                                Nombre = lector.GetString(1)
                            };
                        }
                        return null;
                    }
                });
        }

        public async Task<DTO<IEnumerable<T_Rol>>> Obtener_todos()
        {
            return MiSqlHelper.EjecutarComando("sp_Rol_ObtenerTodos",
                null,
                comando =>
                {
                    var lista = new List<T_Rol>();
                    using (var lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            lista.Add(new T_Rol
                            {
                                RolID = lector.GetInt32(0),
                                Nombre = lector.GetString(1)
                            });
                        }
                    }
                    return lista.AsEnumerable();
                });
        }

        public async Task<DTO<Items_pagina<T_Rol>>> Obtener_paginado(Filtros_paginado filtros)
        {
            try
            {
                string orderByClause = string.IsNullOrEmpty(filtros.OrderBy) ? "ORDER BY RolID" : $"ORDER BY {filtros.OrderBy}";
                string whereClause = string.IsNullOrEmpty(filtros.Filter) ? "" : $"WHERE {filtros.WhereClause()}";

                string query = $@"
                                SELECT * FROM (
                                    SELECT *, ROW_NUMBER() OVER ({orderByClause}) AS RowNum
                                    FROM T_Rol
                                    {whereClause}
                                ) AS PaginatedQuery
                                WHERE RowNum > {filtros.Skip}
                                AND RowNum <= {filtros.Skip + filtros.Top}";

                string countQuery = $@"
                                    SELECT COUNT(*)
                                    FROM T_Rol
                                    {whereClause}";

                DTO<List<T_Rol>> resultado = MiSqlHelper.EjecutarComando(
                    query,
                    null,
                    comando =>
                    {
                        var roles = new List<T_Rol>();
                        using (var lector = comando.ExecuteReader())
                        {
                            while (lector.Read())
                            {
                                roles.Add(new T_Rol
                                {
                                    RolID = lector.GetInt32(0),
                                    Nombre = lector.GetString(1)
                                });
                            }
                        }
                        return roles;
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

                var itemsPagina = new Items_pagina<T_Rol>
                {
                    Items = resultado.Datos,
                    Total_items = totalRegistros.Datos
                };

                return new DTO<Items_pagina<T_Rol>>
                {
                    Datos = itemsPagina,
                    Correcto = true,
                    Mensaje = "Roles de la página obtenidos correctamente"
                };
            }
            catch (Exception ex)
            {
                return new DTO<Items_pagina<T_Rol>>
                {
                    Datos = null,
                    Correcto = false,
                    Mensaje = $"Error al obtener la lista de roles de la página: {ex.Message}"
                };
            }
        }
    }
}