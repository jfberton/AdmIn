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
    namespace AdmIn.Data.Repositorios
    {
        public class Rep_TELEFONO : IRepoBase<TELEFONO>
        {
            public async Task<DTO<TELEFONO>> Crear(TELEFONO telefono)
            {
                return MiSqlHelper.EjecutarComando("sp_Telefono_Insertar",
                    comando =>
                    {
                        comando.Parameters.AddWithValue("@TEL_NUMERO", telefono.TEL_NUMERO);
                        comando.Parameters.AddWithValue("@TPT_ID", telefono.TPT_ID);
                    },
                    comando =>
                    {
                        comando.ExecuteNonQuery();
                        return telefono;
                    });
            }

            public async Task<DTO<TELEFONO>> Actualizar(TELEFONO telefono)
            {
                return MiSqlHelper.EjecutarComando("sp_Telefono_Actualizar",
                    comando =>
                    {
                        comando.Parameters.AddWithValue("@TEL_ID", telefono.TEL_ID);
                        comando.Parameters.AddWithValue("@TEL_NUMERO", telefono.TEL_NUMERO);
                        comando.Parameters.AddWithValue("@TPT_ID", telefono.TPT_ID);
                    },
                    comando =>
                    {
                        comando.ExecuteNonQuery();
                        return telefono;
                    });
            }

            public async Task<DTO<bool>> Eliminar(TELEFONO telefono)
            {
                return MiSqlHelper.EjecutarComando("sp_Telefono_Eliminar",
                    comando => comando.Parameters.AddWithValue("@TEL_ID", telefono.TEL_ID),
                    comando =>
                    {
                        comando.ExecuteNonQuery();
                        return true;
                    });
            }

            public async Task<DTO<TELEFONO?>> Obtener_por_id(TELEFONO telefono)
            {
                return MiSqlHelper.EjecutarComando("sp_Telefono_ObtenerPorId",
                    comando => comando.Parameters.AddWithValue("@TEL_ID", telefono.TEL_ID),
                    comando =>
                    {
                        using (var lector = comando.ExecuteReader())
                        {
                            if (lector.Read())
                            {
                                return new TELEFONO
                                {
                                    TEL_ID = lector.GetInt32(0),
                                    TEL_NUMERO = lector.GetString(1),
                                    TPT_ID = lector.GetInt32(2)
                                };
                            }
                            return null;
                        }
                    });
            }

            public async Task<DTO<IEnumerable<TELEFONO>>> Obtener_todos()
            {
                return MiSqlHelper.EjecutarComando("sp_Telefono_ObtenerTodos",
                    null,
                    comando =>
                    {
                        var lista = new List<TELEFONO>();
                        using (var lector = comando.ExecuteReader())
                        {
                            while (lector.Read())
                            {
                                lista.Add(new TELEFONO
                                {
                                    TEL_ID = lector.GetInt32(0),
                                    TEL_NUMERO = lector.GetString(1),
                                    TPT_ID = lector.GetInt32(2)
                                });
                            }
                        }
                        return lista.AsEnumerable();
                    });
            }

            public async Task<DTO<Items_pagina<TELEFONO>>> Obtener_paginado(Filtros_paginado filtros)
            {
                try
                {
                    string orderByClause = string.IsNullOrEmpty(filtros.OrderBy) ? "ORDER BY TEL_ID" : $"ORDER BY {filtros.OrderBy}";
                    string whereClause = string.IsNullOrEmpty(filtros.Filter) ? "" : $"WHERE {filtros.WhereClause()}";

                    string query = $@"
                                SELECT * FROM (
                                    SELECT *, ROW_NUMBER() OVER ({orderByClause}) AS RowNum
                                    FROM TELEFONO
                                    {whereClause}
                                ) AS PaginatedQuery
                                WHERE RowNum > {filtros.Skip}
                                AND RowNum <= {filtros.Skip + filtros.Top}";

                    string countQuery = $@"
                                    SELECT COUNT(*)
                                    FROM TELEFONO
                                    {whereClause}";

                    DTO<List<TELEFONO>> resultado = MiSqlHelper.EjecutarComando(
                        query,
                        null,
                        comando =>
                        {
                            var telefonos = new List<TELEFONO>();
                            using (var lector = comando.ExecuteReader())
                            {
                                while (lector.Read())
                                {
                                    telefonos.Add(new TELEFONO
                                    {
                                        TEL_ID = lector.GetInt32(0),
                                        TEL_NUMERO = lector.GetString(1),
                                        TPT_ID = lector.GetInt32(2)
                                    });
                                }
                            }
                            return telefonos;
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

                    var itemsPagina = new Items_pagina<TELEFONO>
                    {
                        Items = resultado.Datos,
                        Total_items = totalRegistros.Datos
                    };

                    return new DTO<Items_pagina<TELEFONO>>
                    {
                        Datos = itemsPagina,
                        Correcto = true,
                        Mensaje = "Telefonos obtenidos correctamente"
                    };
                }
                catch (Exception ex)
                {
                    return new DTO<Items_pagina<TELEFONO>>
                    {
                        Datos = null,
                        Correcto = false,
                        Mensaje = $"Error al obtener los telefonos: {ex.Message}"
                    };
                }
            }
        }
    }
}
