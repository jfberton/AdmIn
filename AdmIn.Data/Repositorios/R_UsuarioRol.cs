using AdmIn.Common;
using AdmIn.Data.Entidades;
using AdmIn.Data.Utilitarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdmIn.Data.Repositorios
{
    public class R_UsuarioRol : IRepoBase<T_UsuarioRol>
    {
        public async Task<DTO<T_UsuarioRol>> Crear(T_UsuarioRol usuarioRol)
        {
            return MiSqlHelper.EjecutarComando("sp_UsuarioRol_Insertar",
                comando =>
                {
                    comando.Parameters.AddWithValue("@UsuarioID", usuarioRol.UsuarioID);
                    comando.Parameters.AddWithValue("@RolID", usuarioRol.RolID);
                },
                comando =>
                {
                    comando.ExecuteNonQuery();
                    return usuarioRol;
                });
        }

        public async Task<DTO<T_UsuarioRol>> Actualizar(T_UsuarioRol usuarioRol)
        {
            return MiSqlHelper.EjecutarComando("sp_UsuarioRol_Actualizar",
                comando =>
                {
                    comando.Parameters.AddWithValue("@UsuarioRolID", usuarioRol.UsuarioRolID);
                    comando.Parameters.AddWithValue("@UsuarioID", usuarioRol.UsuarioID);
                    comando.Parameters.AddWithValue("@RolID", usuarioRol.RolID);
                },
                comando =>
                {
                    comando.ExecuteNonQuery();
                    return usuarioRol;
                });
        }

        public async Task<DTO<bool>> Eliminar(T_UsuarioRol usuarioRol)
        {
            return MiSqlHelper.EjecutarComando("sp_UsuarioRol_Eliminar",
                comando => comando.Parameters.AddWithValue("@UsuarioRolID", usuarioRol.UsuarioRolID),
                comando =>
                {
                    comando.ExecuteNonQuery();
                    return true;
                });
        }

        public async Task<DTO<T_UsuarioRol?>> Obtener_por_id(T_UsuarioRol usuarioRol)
        {
            return MiSqlHelper.EjecutarComando("sp_UsuarioRol_ObtenerPorId",
                comando => comando.Parameters.AddWithValue("@UsuarioRolID", usuarioRol.UsuarioRolID),
                comando =>
                {
                    using (var lector = comando.ExecuteReader())
                    {
                        if (lector.Read())
                        {
                            return new T_UsuarioRol
                            {
                                UsuarioRolID = lector.GetInt32(0),
                                UsuarioID = lector.GetInt32(1),
                                RolID = lector.GetInt32(2)
                            };
                        }
                        return null;
                    }
                });
        }

        public async Task<DTO<IEnumerable<T_UsuarioRol>>> Obtener_todos()
        {
            return MiSqlHelper.EjecutarComando("sp_UsuarioRol_ObtenerTodos",
                null,
                comando =>
                {
                    var lista = new List<T_UsuarioRol>();
                    using (var lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            lista.Add(new T_UsuarioRol
                            {
                                UsuarioRolID = lector.GetInt32(0),
                                UsuarioID = lector.GetInt32(1),
                                RolID = lector.GetInt32(2)
                            });
                        }
                    }
                    return lista.AsEnumerable();
                });
        }

        public async Task<DTO<Items_pagina<T_UsuarioRol>>> Obtener_paginado(Filtros_paginado filtros)
        {
            try
            {
                string orderByClause = string.IsNullOrEmpty(filtros.OrderBy) ? "ORDER BY UsuarioRolID" : $"ORDER BY {filtros.OrderBy}";
                string whereClause = string.IsNullOrEmpty(filtros.Filter) ? "" : $"WHERE {filtros.WhereClause()}";

                string query = $@"
                                SELECT * FROM (
                                    SELECT *, ROW_NUMBER() OVER ({orderByClause}) AS RowNum
                                    FROM T_UsuarioRol
                                    {whereClause}
                                ) AS PaginatedQuery
                                WHERE RowNum > {filtros.Skip}
                                AND RowNum <= {filtros.Skip + filtros.Top}";

                string countQuery = $@"
                                    SELECT COUNT(*)
                                    FROM T_UsuarioRol
                                    {whereClause}";

                DTO<List<T_UsuarioRol>> resultado = MiSqlHelper.EjecutarComando(
                    query,
                    null,
                    comando =>
                    {
                        var usuarioRoles = new List<T_UsuarioRol>();
                        using (var lector = comando.ExecuteReader())
                        {
                            while (lector.Read())
                            {
                                usuarioRoles.Add(new T_UsuarioRol
                                {
                                    UsuarioRolID = lector.GetInt32(0),
                                    UsuarioID = lector.GetInt32(1),
                                    RolID = lector.GetInt32(2)
                                });
                            }
                        }
                        return usuarioRoles;
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

                var itemsPagina = new Items_pagina<T_UsuarioRol>
                {
                    Items = resultado.Datos,
                    Total_items = totalRegistros.Datos
                };

                return new DTO<Items_pagina<T_UsuarioRol>>
                {
                    Datos = itemsPagina,
                    Correcto = true,
                    Mensaje = "UsuarioRoles de la página obtenidos correctamente"
                };
            }
            catch (Exception ex)
            {
                return new DTO<Items_pagina<T_UsuarioRol>>
                {
                    Datos = null,
                    Correcto = false,
                    Mensaje = $"Error al obtener la lista de UsuarioRoles de la página: {ex.Message}"
                };
            }
        }
    }
}