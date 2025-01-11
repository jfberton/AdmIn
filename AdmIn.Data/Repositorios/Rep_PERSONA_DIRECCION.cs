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
        namespace AdmIn.Data.Repositorios
        {
            public class Rep_PERSONA_DIRECCION : IRepoBase<PERSONA_DIRECCION>
            {
                public async Task<DTO<PERSONA_DIRECCION>> Crear(PERSONA_DIRECCION personaDireccion)
                {
                    return MiSqlHelper.EjecutarComando("sp_PersonaDireccion_Insertar",
                        comando =>
                        {
                            comando.Parameters.AddWithValue("@PER_ID", personaDireccion.PER_ID);
                            comando.Parameters.AddWithValue("@DIR_ID", personaDireccion.DIR_ID);
                            comando.Parameters.AddWithValue("@DIR_TIP_ID", personaDireccion.DIR_TIP_ID);
                        },
                        comando =>
                        {
                            comando.ExecuteNonQuery();
                            return personaDireccion;
                        });
                }

                public async Task<DTO<PERSONA_DIRECCION>> Actualizar(PERSONA_DIRECCION personaDireccion)
                {
                    return MiSqlHelper.EjecutarComando("sp_PersonaDireccion_Actualizar",
                        comando =>
                        {
                            comando.Parameters.AddWithValue("@PER_DIR_ID", personaDireccion.PER_DIR_ID);
                            comando.Parameters.AddWithValue("@PER_ID", personaDireccion.PER_ID);
                            comando.Parameters.AddWithValue("@DIR_ID", personaDireccion.DIR_ID);
                            comando.Parameters.AddWithValue("@DIR_TIP_ID", personaDireccion.DIR_TIP_ID);
                        },
                        comando =>
                        {
                            comando.ExecuteNonQuery();
                            return personaDireccion;
                        });
                }

                public async Task<DTO<bool>> Eliminar(PERSONA_DIRECCION pd)
                {
                    return MiSqlHelper.EjecutarComando("sp_PersonaDireccion_Eliminar",
                        comando => comando.Parameters.AddWithValue("@Id", pd.PER_DIR_ID),
                        comando =>
                        {
                            comando.ExecuteNonQuery();
                            return true;
                        });
                }

                public async Task<DTO<PERSONA_DIRECCION?>> Obtener_por_id(PERSONA_DIRECCION pd)
                {
                    return MiSqlHelper.EjecutarComando("sp_PersonaDireccion_ObtenerPorId",
                        comando => comando.Parameters.AddWithValue("@PER_DIR_ID", pd.PER_DIR_ID),
                        comando =>
                        {
                            using (var lector = comando.ExecuteReader())
                            {
                                if (lector.Read())
                                {
                                    return new PERSONA_DIRECCION
                                    {
                                        PER_DIR_ID = lector.GetInt32(0),
                                        PER_ID = lector.GetInt32(1),
                                        DIR_ID = lector.GetInt32(2),
                                        DIR_TIP_ID = lector.GetInt32(3)
                                    };
                                }
                                return null;
                            }
                        });
                }

                public async Task<DTO<IEnumerable<PERSONA_DIRECCION>>> Obtener_por_persona(int per_id)
                {
                    return MiSqlHelper.EjecutarComando("sp_PersonaDireccion_ObtenerPorPersona",
                        comando => comando.Parameters.AddWithValue("@PER_ID", per_id),
                        comando =>
                        {
                            var lista = new List<PERSONA_DIRECCION>();
                            using (var lector = comando.ExecuteReader())
                            {
                                while (lector.Read())
                                {
                                    lista.Add(new PERSONA_DIRECCION
                                    {
                                        PER_DIR_ID = lector.GetInt32(0),
                                        PER_ID = lector.GetInt32(1),
                                        DIR_ID = lector.GetInt32(2),
                                        DIR_TIP_ID = lector.GetInt32(3)
                                    });
                                }
                            }
                            return lista.AsEnumerable();
                        });
                }

                public async Task<DTO<IEnumerable<PERSONA_DIRECCION>>> Obtener_todos()
                {
                    return MiSqlHelper.EjecutarComando("sp_PersonaDireccion_ObtenerTodos",
                        null,
                        comando =>
                        {
                            var lista = new List<PERSONA_DIRECCION>();
                            using (var lector = comando.ExecuteReader())
                            {
                                while (lector.Read())
                                {
                                    lista.Add(new PERSONA_DIRECCION
                                    {
                                        PER_DIR_ID = lector.GetInt32(0),
                                        PER_ID = lector.GetInt32(1),
                                        DIR_ID = lector.GetInt32(2),
                                        DIR_TIP_ID = lector.GetInt32(3)
                                    });
                                }
                            }
                            return lista.AsEnumerable();
                        });
                }

                public async Task<DTO<Items_pagina<PERSONA_DIRECCION>>> Obtener_paginado(Filtros_paginado filtros)
                {
                    try
                    {
                        string orderByClause = string.IsNullOrEmpty(filtros.OrderBy) ? "ORDER BY PER_DIR_ID" : $"ORDER BY {filtros.OrderBy}";
                        string whereClause = string.IsNullOrEmpty(filtros.Filter) ? "" : $"WHERE {filtros.WhereClause()}";

                        string query = $@"
                                SELECT * FROM (
                                    SELECT *, ROW_NUMBER() OVER ({orderByClause}) AS RowNum
                                    FROM PERSONA_DIRECCION
                                    {whereClause}
                                ) AS PaginatedQuery
                                WHERE RowNum > {filtros.Skip}
                                AND RowNum <= {filtros.Skip + filtros.Top}";

                        string countQuery = $@"
                                    SELECT COUNT(*)
                                    FROM PERSONA_DIRECCION
                                    {whereClause}";

                        DTO<List<PERSONA_DIRECCION>> resultado = MiSqlHelper.EjecutarComando(
                            query,
                            null,
                            comando =>
                            {
                                var relaciones = new List<PERSONA_DIRECCION>();
                                using (var lector = comando.ExecuteReader())
                                {
                                    while (lector.Read())
                                    {
                                        relaciones.Add(new PERSONA_DIRECCION
                                        {
                                            PER_DIR_ID = lector.GetInt32(0),
                                            PER_ID = lector.GetInt32(1),
                                            DIR_ID = lector.GetInt32(2),
                                            DIR_TIP_ID = lector.GetInt32(3)
                                        });
                                    }
                                }
                                return relaciones;
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

                        var itemsPagina = new Items_pagina<PERSONA_DIRECCION>
                        {
                            Items = resultado.Datos,
                            Total_items = totalRegistros.Datos
                        };

                        return new DTO<Items_pagina<PERSONA_DIRECCION>>
                        {
                            Datos = itemsPagina,
                            Correcto = true,
                            Mensaje = "Relaciones persona-dirección obtenidas correctamente"
                        };
                    }
                    catch (Exception ex)
                    {
                        return new DTO<Items_pagina<PERSONA_DIRECCION>>
                        {
                            Datos = null,
                            Correcto = false,
                            Mensaje = $"Error al obtener las relaciones persona-dirección: {ex.Message}"
                        };
                    }
                }
            }
        }

    }

}
