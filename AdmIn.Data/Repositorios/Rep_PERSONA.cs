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
    public class Rep_PERSONA : IRepoBase<PERSONA>
    {
        public async Task<DTO<PERSONA>> Crear(PERSONA persona)
        {
            return MiSqlHelper.EjecutarComando("sp_Persona_Insertar",
                comando =>
                {
                    comando.Parameters.AddWithValue("@PER_RFC", persona.PER_RFC);
                    comando.Parameters.AddWithValue("@PER_NOMBRE", persona.PER_NOMBRE);
                    comando.Parameters.AddWithValue("@PER_APATERNO", (object?)persona.PER_APATERNO ?? DBNull.Value);
                    comando.Parameters.AddWithValue("@PER_AMATERNO", (object?)persona.PER_AMATERNO ?? DBNull.Value);
                    comando.Parameters.AddWithValue("@PER_EMAIL", (object?)persona.PER_EMAIL ?? DBNull.Value);
                    comando.Parameters.AddWithValue("@PER_NACIONALIDAD", (object?)persona.PER_NACIONALIDAD ?? DBNull.Value);
                    comando.Parameters.AddWithValue("@PER_ESPERSONA", persona.PER_ESPERSONA);
                    comando.Parameters.AddWithValue("@PER_TITULAR", persona.PER_TITULAR);
                },
                comando =>
                {
                    comando.ExecuteNonQuery();
                    return persona;
                });
        }

        public async Task<DTO<PERSONA>> Actualizar(PERSONA persona)
        {
            return MiSqlHelper.EjecutarComando("sp_Persona_Actualizar",
                comando =>
                {
                    comando.Parameters.AddWithValue("@PER_ID", persona.PER_ID);
                    comando.Parameters.AddWithValue("@PER_RFC", persona.PER_RFC);
                    comando.Parameters.AddWithValue("@PER_NOMBRE", persona.PER_NOMBRE);
                    comando.Parameters.AddWithValue("@PER_APATERNO", (object?)persona.PER_APATERNO ?? DBNull.Value);
                    comando.Parameters.AddWithValue("@PER_AMATERNO", (object?)persona.PER_AMATERNO ?? DBNull.Value);
                    comando.Parameters.AddWithValue("@PER_EMAIL", (object?)persona.PER_EMAIL ?? DBNull.Value);
                    comando.Parameters.AddWithValue("@PER_NACIONALIDAD", (object?)persona.PER_NACIONALIDAD ?? DBNull.Value);
                    comando.Parameters.AddWithValue("@PER_ESPERSONA", persona.PER_ESPERSONA);
                    comando.Parameters.AddWithValue("@PER_TITULAR", persona.PER_TITULAR);
                },
                comando =>
                {
                    comando.ExecuteNonQuery();
                    return persona;
                });
        }

        public async Task<DTO<bool>> Eliminar(PERSONA persona)
        {
            return MiSqlHelper.EjecutarComando("sp_Persona_Eliminar",
                comando => comando.Parameters.AddWithValue("@PER_ID", persona.PER_ID),
                comando =>
                {
                    comando.ExecuteNonQuery();
                    return true;
                });
        }

        public async Task<DTO<PERSONA?>> Obtener_por_id(PERSONA persona)
        {
            return MiSqlHelper.EjecutarComando("sp_Persona_ObtenerPorId",
                comando => comando.Parameters.AddWithValue("@PER_ID", persona.PER_ID),
                comando =>
                {
                    using (var lector = comando.ExecuteReader())
                    {
                        if (lector.Read())
                        {
                            return new PERSONA
                            {
                                PER_ID = lector.GetInt32(0),
                                PER_RFC = lector.GetString(1),
                                PER_NOMBRE = lector.GetString(2),
                                PER_APATERNO = lector.IsDBNull(3) ? null : lector.GetString(3),
                                PER_AMATERNO = lector.IsDBNull(4) ? null : lector.GetString(4),
                                PER_EMAIL = lector.IsDBNull(5) ? null : lector.GetString(5),
                                PER_NACIONALIDAD = lector.IsDBNull(6) ? null : lector.GetString(6),
                                PER_ESPERSONA = lector.GetBoolean(7),
                                PER_TITULAR = lector.GetBoolean(8)
                            };
                        }
                        return null;
                    }
                });
        }

        public async Task<DTO<IEnumerable<PERSONA>>> Obtener_todos()
        {
            return MiSqlHelper.EjecutarComando("sp_Persona_ObtenerTodos",
                null,
                comando =>
                {
                    var lista = new List<PERSONA>();
                    using (var lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            lista.Add(new PERSONA
                            {
                                PER_ID = lector.GetInt32(0),
                                PER_RFC = lector.GetString(1),
                                PER_NOMBRE = lector.GetString(2),
                                PER_APATERNO = lector.IsDBNull(3) ? null : lector.GetString(3),
                                PER_AMATERNO = lector.IsDBNull(4) ? null : lector.GetString(4),
                                PER_EMAIL = lector.IsDBNull(5) ? null : lector.GetString(5),
                                PER_NACIONALIDAD = lector.IsDBNull(6) ? null : lector.GetString(6),
                                PER_ESPERSONA = lector.GetBoolean(7),
                                PER_TITULAR = lector.GetBoolean(8)
                            });
                        }
                    }
                    return lista.AsEnumerable();
                });
        }

        public async Task<DTO<Items_pagina<PERSONA>>> Obtener_paginado(Filtros_paginado filtros)
        {
            try
            {
                string orderByClause = string.IsNullOrEmpty(filtros.OrderBy) ? "ORDER BY PER_ID" : $"ORDER BY {filtros.OrderBy}";
                string whereClause = string.IsNullOrEmpty(filtros.Filter) ? "" : $"WHERE {filtros.WhereClause()}";

                string query = $@"
                                SELECT * FROM (
                                    SELECT *, ROW_NUMBER() OVER ({orderByClause}) AS RowNum
                                    FROM PERSONA
                                    {whereClause}
                                ) AS PaginatedQuery
                                WHERE RowNum > {filtros.Skip}
                                AND RowNum <= {filtros.Skip + filtros.Top}";

                string countQuery = $@"
                                    SELECT COUNT(*)
                                    FROM PERSONA
                                    {whereClause}";

                DTO<List<PERSONA>> resultado = MiSqlHelper.EjecutarComando(
                    query,
                    null,
                    comando =>
                    {
                        var personas = new List<PERSONA>();
                        using (var lector = comando.ExecuteReader())
                        {
                            while (lector.Read())
                            {
                                personas.Add(new PERSONA
                                {
                                    PER_ID = lector.GetInt32(0),
                                    PER_RFC = lector.GetString(1),
                                    PER_NOMBRE = lector.GetString(2),
                                    PER_APATERNO = lector.IsDBNull(3) ? null : lector.GetString(3),
                                    PER_AMATERNO = lector.IsDBNull(4) ? null : lector.GetString(4),
                                    PER_EMAIL = lector.IsDBNull(5) ? null : lector.GetString(5),
                                    PER_NACIONALIDAD = lector.IsDBNull(6) ? null : lector.GetString(6),
                                    PER_ESPERSONA = lector.GetBoolean(7),
                                    PER_TITULAR = lector.GetBoolean(8)
                                });
                            }
                        }
                        return personas;
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

                var itemsPagina = new Items_pagina<PERSONA>
                {
                    Items = resultado.Datos,
                    Total_items = totalRegistros.Datos
                };

                return new DTO<Items_pagina<PERSONA>>
                {
                    Datos = itemsPagina,
                    Correcto = true,
                    Mensaje = "Personas de la página obtenidas correctamente"
                };
            }
            catch (Exception ex)
            {
                return new DTO<Items_pagina<PERSONA>>
                {
                    Datos = null,
                    Correcto = false,
                    Mensaje = $"Error al obtener la lista de personas de la página: {ex.Message}"
                };
            }
        }
    }
}
