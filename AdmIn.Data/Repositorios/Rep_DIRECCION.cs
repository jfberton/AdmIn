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
        public class Rep_DIRECCION : IRepoBase<DIRECCION>
        {
            public async Task<DTO<DIRECCION>> Crear(DIRECCION direccion)
            {
                return MiSqlHelper.EjecutarComando("sp_Direccion_Insertar",
                    comando =>
                    {
                        comando.Parameters.AddWithValue("@DIR_CALLE_NUMERO", direccion.DIR_CALLE_NUMERO);
                        comando.Parameters.AddWithValue("@DIR_COLONIA", (object?)direccion.DIR_COLONIA ?? DBNull.Value);
                        comando.Parameters.AddWithValue("@DIR_CIUDAD", (object?)direccion.DIR_CIUDAD ?? DBNull.Value);
                        comando.Parameters.AddWithValue("@DIR_ESTADO", (object?)direccion.DIR_ESTADO ?? DBNull.Value);
                        comando.Parameters.AddWithValue("@DIR_CP", (object?)direccion.DIR_CP ?? DBNull.Value);
                        comando.Parameters.AddWithValue("@DIR_PAIS", (object?)direccion.DIR_PAIS ?? DBNull.Value);
                    },
                    comando =>
                    {
                        comando.ExecuteNonQuery();
                        return direccion;
                    });
            }

            public async Task<DTO<DIRECCION>> Actualizar(DIRECCION direccion)
            {
                return MiSqlHelper.EjecutarComando("sp_Direccion_Actualizar",
                    comando =>
                    {
                        comando.Parameters.AddWithValue("@DIR_ID", direccion.DIR_ID);
                        comando.Parameters.AddWithValue("@DIR_CALLE_NUMERO", direccion.DIR_CALLE_NUMERO);
                        comando.Parameters.AddWithValue("@DIR_COLONIA", (object?)direccion.DIR_COLONIA ?? DBNull.Value);
                        comando.Parameters.AddWithValue("@DIR_CIUDAD", (object?)direccion.DIR_CIUDAD ?? DBNull.Value);
                        comando.Parameters.AddWithValue("@DIR_ESTADO", (object?)direccion.DIR_ESTADO ?? DBNull.Value);
                        comando.Parameters.AddWithValue("@DIR_CP", (object?)direccion.DIR_CP ?? DBNull.Value);
                        comando.Parameters.AddWithValue("@DIR_PAIS", (object?)direccion.DIR_PAIS ?? DBNull.Value);
                    },
                    comando =>
                    {
                        comando.ExecuteNonQuery();
                        return direccion;
                    });
            }

            public async Task<DTO<bool>> Eliminar(DIRECCION direccion)
            {
                return MiSqlHelper.EjecutarComando("sp_Direccion_Eliminar",
                    comando => comando.Parameters.AddWithValue("@DIR_ID", direccion.DIR_ID),
                    comando =>
                    {
                        comando.ExecuteNonQuery();
                        return true;
                    });
            }

            public async Task<DTO<DIRECCION?>> Obtener_por_id(DIRECCION direccion)
            {
                return MiSqlHelper.EjecutarComando("sp_Direccion_ObtenerPorId",
                    comando => comando.Parameters.AddWithValue("@DIR_ID", direccion.DIR_ID),
                    comando =>
                    {
                        using (var lector = comando.ExecuteReader())
                        {
                            if (lector.Read())
                            {
                                return new DIRECCION
                                {
                                    DIR_ID = lector.GetInt32(0),
                                    DIR_CALLE_NUMERO = lector.GetString(1),
                                    DIR_COLONIA = lector.IsDBNull(2) ? null : lector.GetString(2),
                                    DIR_CIUDAD = lector.IsDBNull(3) ? null : lector.GetString(3),
                                    DIR_ESTADO = lector.IsDBNull(4) ? null : lector.GetString(4),
                                    DIR_CP = lector.IsDBNull(5) ? null : lector.GetString(5),
                                    DIR_PAIS = lector.IsDBNull(6) ? null : lector.GetString(6)
                                };
                            }
                            return null;
                        }
                    });
            }

            public async Task<DTO<IEnumerable<DIRECCION>>> Obtener_todos()
            {
                return MiSqlHelper.EjecutarComando("sp_Direccion_ObtenerTodos",
                    null,
                    comando =>
                    {
                        var lista = new List<DIRECCION>();
                        using (var lector = comando.ExecuteReader())
                        {
                            while (lector.Read())
                            {
                                lista.Add(new DIRECCION
                                {
                                    DIR_ID = lector.GetInt32(0),
                                    DIR_CALLE_NUMERO = lector.GetString(1),
                                    DIR_COLONIA = lector.IsDBNull(2) ? null : lector.GetString(2),
                                    DIR_CIUDAD = lector.IsDBNull(3) ? null : lector.GetString(3),
                                    DIR_ESTADO = lector.IsDBNull(4) ? null : lector.GetString(4),
                                    DIR_CP = lector.IsDBNull(5) ? null : lector.GetString(5),
                                    DIR_PAIS = lector.IsDBNull(6) ? null : lector.GetString(6)
                                });
                            }
                        }
                        return lista.AsEnumerable();
                    });
            }

            public async Task<DTO<Items_pagina<DIRECCION>>> Obtener_paginado(Filtros_paginado filtros)
            {
                try
                {
                    string orderByClause = string.IsNullOrEmpty(filtros.OrderBy) ? "ORDER BY DIR_ID" : $"ORDER BY {filtros.OrderBy}";
                    string whereClause = string.IsNullOrEmpty(filtros.Filter) ? "" : $"WHERE {filtros.WhereClause()}";

                    string query = $@"
                                SELECT * FROM (
                                    SELECT *, ROW_NUMBER() OVER ({orderByClause}) AS RowNum
                                    FROM DIRECCION
                                    {whereClause}
                                ) AS PaginatedQuery
                                WHERE RowNum > {filtros.Skip}
                                AND RowNum <= {filtros.Skip + filtros.Top}";

                    string countQuery = $@"
                                    SELECT COUNT(*)
                                    FROM DIRECCION
                                    {whereClause}";

                    DTO<List<DIRECCION>> resultado = MiSqlHelper.EjecutarComando(
                        query,
                        null,
                        comando =>
                        {
                            var direcciones = new List<DIRECCION>();
                            using (var lector = comando.ExecuteReader())
                            {
                                while (lector.Read())
                                {
                                    direcciones.Add(new DIRECCION
                                    {
                                        DIR_ID = lector.GetInt32(0),
                                        DIR_CALLE_NUMERO = lector.GetString(1),
                                        DIR_COLONIA = lector.IsDBNull(2) ? null : lector.GetString(2),
                                        DIR_CIUDAD = lector.IsDBNull(3) ? null : lector.GetString(3),
                                        DIR_ESTADO = lector.IsDBNull(4) ? null : lector.GetString(4),
                                        DIR_CP = lector.IsDBNull(5) ? null : lector.GetString(5),
                                        DIR_PAIS = lector.IsDBNull(6) ? null : lector.GetString(6)
                                    });
                                }
                            }
                            return direcciones;
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

                    var itemsPagina = new Items_pagina<DIRECCION>
                    {
                        Items = resultado.Datos,
                        Total_items = totalRegistros.Datos
                    };

                    return new DTO<Items_pagina<DIRECCION>>
                    {
                        Datos = itemsPagina,
                        Correcto = true,
                        Mensaje = "Direcciones obtenidas correctamente"
                    };
                }
                catch (Exception ex)
                {
                    return new DTO<Items_pagina<DIRECCION>>
                    {
                        Datos = null,
                        Correcto = false,
                        Mensaje = $"Error al obtener las direcciones: {ex.Message}"
                    };
                }
            }
        }
    }

}
