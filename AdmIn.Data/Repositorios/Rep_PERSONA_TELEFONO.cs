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
    public class Rep_PERSONA_TELEFONO : IRepoBase<PERSONA_TELEFONO>
    {
        public async Task<DTO<PERSONA_TELEFONO>> Crear(PERSONA_TELEFONO personaTelefono)
        {
            return MiSqlHelper.EjecutarComando("sp_PersonaTelefono_Insertar",
                comando =>
                {
                    comando.Parameters.AddWithValue("@PER_ID", personaTelefono.PER_ID);
                    comando.Parameters.AddWithValue("@TEL_ID", personaTelefono.TEL_ID);
                },
                comando =>
                {
                    comando.ExecuteNonQuery();
                    return personaTelefono;
                });
        }

        public async Task<DTO<PERSONA_TELEFONO>> Actualizar(PERSONA_TELEFONO personaTelefono)
        {
            return MiSqlHelper.EjecutarComando("sp_PersonaTelefono_Actualizar",
                comando =>
                {
                    comando.Parameters.AddWithValue("@PER_TEL_ID", personaTelefono.PER_TEL_ID);
                    comando.Parameters.AddWithValue("@PER_ID", personaTelefono.PER_ID);
                    comando.Parameters.AddWithValue("@TEL_ID", personaTelefono.TEL_ID);
                },
                comando =>
                {
                    comando.ExecuteNonQuery();
                    return personaTelefono;
                });
        }

        public async Task<DTO<bool>> Eliminar(PERSONA_TELEFONO personaTelefono)
        {
            return MiSqlHelper.EjecutarComando("sp_PersonaTelefono_Eliminar",
                comando => comando.Parameters.AddWithValue("@PER_TEL_ID", personaTelefono.PER_TEL_ID),
                comando =>
                {
                    comando.ExecuteNonQuery();
                    return true;
                });
        }

        public async Task<DTO<PERSONA_TELEFONO?>> Obtener_por_id(PERSONA_TELEFONO personaTelefono)
        {
            return MiSqlHelper.EjecutarComando("sp_PersonaTelefono_ObtenerPorId",
                comando => comando.Parameters.AddWithValue("@PER_TEL_ID", personaTelefono.PER_TEL_ID),
                comando =>
                {
                    using (var lector = comando.ExecuteReader())
                    {
                        if (lector.Read())
                        {
                            return new PERSONA_TELEFONO
                            {
                                PER_TEL_ID = lector.GetInt32(0),
                                PER_ID = lector.GetInt32(1),
                                TEL_ID = lector.GetInt32(2)
                            };
                        }
                        return null;
                    }
                });
        }

        public async Task<DTO<IEnumerable<PERSONA_TELEFONO>>> Obtener_por_persona(int personaId)
        {
            return MiSqlHelper.EjecutarComando("sp_Persona_Telefono_ObtenerPorPersona",
                comando => comando.Parameters.AddWithValue("@PER_ID", personaId),
               comando =>
               {
                   var lista = new List<PERSONA_TELEFONO>();
                   using (var lector = comando.ExecuteReader())
                   {
                       while (lector.Read())
                       {
                           lista.Add(new PERSONA_TELEFONO
                           {
                               PER_TEL_ID = lector.GetInt32(0),
                               PER_ID = lector.GetInt32(1),
                               TEL_ID = lector.GetInt32(2)
                           });
                       }
                   }
                   return lista.AsEnumerable();
               });
        }

        public async Task<DTO<IEnumerable<PERSONA_TELEFONO>>> Obtener_todos()
        {
            return MiSqlHelper.EjecutarComando("sp_PersonaTelefono_ObtenerTodos",
                null,
                comando =>
                {
                    var lista = new List<PERSONA_TELEFONO>();
                    using (var lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            lista.Add(new PERSONA_TELEFONO
                            {
                                PER_TEL_ID = lector.GetInt32(0),
                                PER_ID = lector.GetInt32(1),
                                TEL_ID = lector.GetInt32(2)
                            });
                        }
                    }
                    return lista.AsEnumerable();
                });
        }

        public async Task<DTO<Items_pagina<PERSONA_TELEFONO>>> Obtener_paginado(Filtros_paginado filtros)
        {
            try
            {
                string orderByClause = string.IsNullOrEmpty(filtros.OrderBy) ? "ORDER BY PER_TEL_ID" : $"ORDER BY {filtros.OrderBy}";
                string whereClause = string.IsNullOrEmpty(filtros.Filter) ? "" : $"WHERE {filtros.WhereClause()}";

                string query = $@"
                                SELECT * FROM (
                                    SELECT *, ROW_NUMBER() OVER ({orderByClause}) AS RowNum
                                    FROM PERSONA_TELEFONO
                                    {whereClause}
                                ) AS PaginatedQuery
                                WHERE RowNum > {filtros.Skip}
                                AND RowNum <= {filtros.Skip + filtros.Top}";

                string countQuery = $@"
                                    SELECT COUNT(*)
                                    FROM PERSONA_TELEFONO
                                    {whereClause}";

                DTO<List<PERSONA_TELEFONO>> resultado = MiSqlHelper.EjecutarComando(
                    query,
                    null,
                    comando =>
                    {
                        var relaciones = new List<PERSONA_TELEFONO>();
                        using (var lector = comando.ExecuteReader())
                        {
                            while (lector.Read())
                            {
                                relaciones.Add(new PERSONA_TELEFONO
                                {
                                    PER_TEL_ID = lector.GetInt32(0),
                                    PER_ID = lector.GetInt32(1),
                                    TEL_ID = lector.GetInt32(2)
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

                var itemsPagina = new Items_pagina<PERSONA_TELEFONO>
                {
                    Items = resultado.Datos,
                    Total_items = totalRegistros.Datos
                };

                return new DTO<Items_pagina<PERSONA_TELEFONO>>
                {
                    Datos = itemsPagina,
                    Correcto = true,
                    Mensaje = "Relaciones persona-teléfono obtenidas correctamente"
                };
            }
            catch (Exception ex)
            {
                return new DTO<Items_pagina<PERSONA_TELEFONO>>
                {
                    Datos = null,
                    Correcto = false,
                    Mensaje = $"Error al obtener las relaciones persona-teléfono: {ex.Message}"
                };
            }
        }
    }
}
