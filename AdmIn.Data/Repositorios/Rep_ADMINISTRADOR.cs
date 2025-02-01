using AdmIn.Common;
using AdmIn.Data.Entidades;
using AdmIn.Data.Utilitarios;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmIn.Data.Repositorios
{
    public class Rep_ADMINISTRADOR : IRepoBase<ADMINISTRADOR>
    {
        public async Task<DTO<ADMINISTRADOR>> Crear(ADMINISTRADOR administrador)
        {
            return MiSqlHelper.EjecutarComando("sp_Administrador_Insertar",
                comando =>
                {
                    comando.Parameters.AddWithValue("@PER_ID", administrador.PER_ID);
                    comando.Parameters.AddWithValue("@ADM_SUPERIOR_ID", (object?)administrador.ADM_SUPERIOR_ID ?? DBNull.Value);
                },
                comando =>
                {
                    comando.ExecuteNonQuery();
                    return administrador;
                });
        }

        public async Task<DTO<ADMINISTRADOR>> Actualizar(ADMINISTRADOR administrador)
        {
            return MiSqlHelper.EjecutarComando("sp_Administrador_Actualizar",
                comando =>
                {
                    comando.Parameters.AddWithValue("@ADM_ID", administrador.ADM_ID);
                    comando.Parameters.AddWithValue("@PER_ID", administrador.PER_ID);
                    comando.Parameters.AddWithValue("@ADM_SUPERIOR_ID", (object?)administrador.ADM_SUPERIOR_ID ?? DBNull.Value);
                },
                comando =>
                {
                    comando.ExecuteNonQuery();
                    return administrador;
                });
        }

        public async Task<DTO<bool>> Eliminar(ADMINISTRADOR administrador)
        {
            return MiSqlHelper.EjecutarComando("sp_Administrador_Eliminar",
                comando => comando.Parameters.AddWithValue("@ADM_ID", administrador.ADM_ID),
                comando =>
                {
                    comando.ExecuteNonQuery();
                    return true;
                });
        }

        public async Task<DTO<ADMINISTRADOR?>> Obtener_por_id(ADMINISTRADOR administrador)
        {
            return MiSqlHelper.EjecutarComando("sp_Administrador_ObtenerPorId",
                comando => comando.Parameters.AddWithValue("@ADM_ID", administrador.ADM_ID),
                comando =>
                {
                    using (var lector = comando.ExecuteReader())
                    {
                        if (lector.Read())
                        {
                            return new ADMINISTRADOR
                            {
                                ADM_ID = lector.GetInt32(0),
                                PER_ID = lector.GetInt32(1),
                                ADM_SUPERIOR_ID = lector.IsDBNull(2) ? null : lector.GetInt32(2)
                            };
                        }
                        return null;
                    }
                });
        }

        public async Task<DTO<IEnumerable<ADMINISTRADOR>>> Obtener_todos()
        {
            return MiSqlHelper.EjecutarComando("sp_Administrador_ObtenerTodos",
                null,
                comando =>
                {
                    var lista = new List<ADMINISTRADOR>();
                    using (var lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            lista.Add(new ADMINISTRADOR
                            {
                                ADM_ID = lector.GetInt32(0),
                                PER_ID = lector.GetInt32(1),
                                ADM_SUPERIOR_ID = lector.IsDBNull(2) ? null : lector.GetInt32(2)
                            });
                        }
                    }
                    return lista.AsEnumerable();
                });
        }

        public async Task<DTO<Items_pagina<ADMINISTRADOR>>> Obtener_paginado(Filtros_paginado filtros)
        {
            try
            {
                string orderByClause = string.IsNullOrEmpty(filtros.OrderBy) ? "ORDER BY ADM_ID" : $"ORDER BY {filtros.OrderBy}";
                string whereClause = string.IsNullOrEmpty(filtros.Filter) ? "" : $"WHERE {filtros.WhereClause()}";

                string query = $@"
                                SELECT * FROM (
                                    SELECT *, ROW_NUMBER() OVER ({orderByClause}) AS RowNum
                                    FROM ADMINISTRADOR
                                    {whereClause}
                                ) AS PaginatedQuery
                                WHERE RowNum > {filtros.Skip}
                                AND RowNum <= {filtros.Skip + filtros.Top}";

                string countQuery = $@"
                                    SELECT COUNT(*)
                                    FROM ADMINISTRADOR
                                    {whereClause}";

                DTO<List<ADMINISTRADOR>> resultado = MiSqlHelper.EjecutarComando(
                    query,
                    null,
                    comando =>
                    {
                        var administradores = new List<ADMINISTRADOR>();
                        using (var lector = comando.ExecuteReader())
                        {
                            while (lector.Read())
                            {
                                administradores.Add(new ADMINISTRADOR
                                {
                                    ADM_ID = lector.GetInt32(0),
                                    PER_ID = lector.GetInt32(1),
                                    ADM_SUPERIOR_ID = lector.IsDBNull(2) ? null : lector.GetInt32(2)
                                });
                            }
                        }
                        return administradores;
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

                var itemsPagina = new Items_pagina<ADMINISTRADOR>
                {
                    Items = resultado.Datos,
                    Total_items = totalRegistros.Datos
                };

                return new DTO<Items_pagina<ADMINISTRADOR>>
                {
                    Datos = itemsPagina,
                    Correcto = true,
                    Mensaje = "Administradores de la página obtenidos correctamente"
                };
            }
            catch (Exception ex)
            {
                return new DTO<Items_pagina<ADMINISTRADOR>>
                {
                    Datos = null,
                    Correcto = false,
                    Mensaje = $"Error al obtener la lista de administradores de la página: {ex.Message}"
                };
            }
        }
    }
}
