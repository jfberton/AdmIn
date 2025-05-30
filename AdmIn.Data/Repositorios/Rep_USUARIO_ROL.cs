﻿using AdmIn.Data.Utilitarios;
using AdmIn.Data.Entidades;
using AdmIn.Common;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Collections.Generic;

namespace AdmIn.Data.Repositorios
{
    public class Rep_USUARIO_ROL : IRepoBase<USUARIO_ROL>
    {
        public async Task<DTO<USUARIO_ROL>> Crear(USUARIO_ROL usuarioRol)
        {
            if (usuarioRol.USU_ID == 0 || usuarioRol.ROL_ID == 0)
            {
                return new DTO<USUARIO_ROL>
                {
                    Correcto = false,
                    Mensaje = "El ID de usuario y el ID de rol no pueden ser cero."
                };
            }

            return MiSqlHelper.EjecutarComando("sp_UsuarioRol_Insertar",
                comando =>
                {
                    comando.Parameters.AddWithValue("@USU_ID", usuarioRol.USU_ID);
                    comando.Parameters.AddWithValue("@ROL_ID", usuarioRol.ROL_ID);
                },
                comando =>
                {
                    comando.ExecuteNonQuery();
                    return usuarioRol;
                });
        }

        public async Task<DTO<USUARIO_ROL>> Actualizar(USUARIO_ROL usuarioRol)
        {
            return new DTO<USUARIO_ROL>
            {
                Correcto = false,
                Mensaje = "No es posible actualizar una relación entre usuario y rol."
            };

        }

        public async Task<DTO<bool>> Eliminar(USUARIO_ROL entidad)
        {
            return MiSqlHelper.EjecutarComando("sp_UsuarioRol_Eliminar",
            comando =>
            {
                comando.Parameters.AddWithValue("@USU_ID", entidad.USU_ID);
                comando.Parameters.AddWithValue("@ROL_ID", entidad.ROL_ID);
            },
            comando =>
            {
                comando.ExecuteNonQuery();
                return true;
            });
        }

        public async Task<DTO<USUARIO_ROL?>> Obtener_por_id(USUARIO_ROL entidad)
        {
            return MiSqlHelper.EjecutarComando("sp_UsuarioRol_ObtenerPorId",
                comando => {
                    comando.Parameters.AddWithValue("@USU_ID", entidad.USU_ID);
                    comando.Parameters.AddWithValue("@ROL_ID", entidad.ROL_ID);
                },
                comando =>
                {
                    using (var lector = comando.ExecuteReader())
                    {
                        if (lector.Read())
                        {
                            return new USUARIO_ROL
                            {
                                USU_ID = lector.GetInt32(0),
                                ROL_ID = lector.GetInt32(1)
                            };
                        }
                        return null;
                    }
                });
        }

        public async Task<DTO<IEnumerable<USUARIO_ROL>>> Obtener_por_usuario(int id)
        {
            return MiSqlHelper.EjecutarComando("sp_UsuarioRol_ObtenerPorUsuario",
                comando => comando.Parameters.AddWithValue("@USU_ID", id),
                comando =>
                {
                    var lista = new List<USUARIO_ROL>();
                    using (var lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            lista.Add(new USUARIO_ROL
                            {
                                USU_ID = lector.GetInt32(0),
                                ROL_ID = lector.GetInt32(1)
                            });
                        }
                    }
                    return lista.AsEnumerable();
                });
        }

        public async Task<DTO<IEnumerable<USUARIO_ROL>>> Obtener_todos()
        {
            return MiSqlHelper.EjecutarComando("sp_UsuarioRol_ObtenerTodos",
                null,
                comando =>
                {
                    var lista = new List<USUARIO_ROL>();
                    using (var lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            lista.Add(new USUARIO_ROL
                            {
                                USU_ID = lector.GetInt32(0),
                                ROL_ID = lector.GetInt32(1)
                            });
                        }
                    }
                    return lista.AsEnumerable();
                });
        }

        public async Task<DTO<Items_pagina<USUARIO_ROL>>> Obtener_paginado(Filtros_paginado filtros)
        {
            try
            {
                // Construcción de la cláusula ORDER BY
                string orderByClause = string.IsNullOrEmpty(filtros.OrderBy) ? "ORDER BY USU_ID" : $"ORDER BY {filtros.OrderBy}";

                // Construcción del filtro WHERE dinámico
                string whereClause = string.IsNullOrEmpty(filtros.Filter) ? "" : $"WHERE {filtros.WhereClause()}";

                // Consulta SQL paginada
                string query = $@"
                                SELECT * FROM (
                                    SELECT *, ROW_NUMBER() OVER ({orderByClause}) AS RowNum
                                    FROM 
                                        (-- CONSULTA QUE AGRUPA LAS TABLAS RELACIONADAS PARA HACER EL FILTRO DEL WHERE
                                            SELECT DISTINCT USUARIO_ROL.*, ROL.ROL_NOMBRE, PERMISO.PERM_NOMBRE
                                            FROM
                                                USUARIO_ROL
                                                JOIN ROL ON USUARIO_ROL.ROL_ID = ROL.ROL_ID
                                                JOIN ROL_PERMISO ON ROL.ROL_ID = ROL_PERMISO.ROL_ID
                                                JOIN PERMISO ON ROL_PERMISO.PERM_ID = PERMISO.PERM_ID
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
                                            SELECT DISTINCT USUARIO_ROL.*
                                            FROM
                                                USUARIO_ROL
                                                JOIN ROL ON USUARIO_ROL.ROL_ID = ROL.ROL_ID
                                                JOIN ROL_PERMISO ON ROL.ROL_ID = ROL_PERMISO.ROL_ID
                                                JOIN PERMISO ON ROL_PERMISO.PERM_ID = PERMISO.PERM_ID
                                            {whereClause}
                                        ) AUX";

                // Ejecutamos la consulta principal para obtener los usuarios y roles paginados
                DTO<List<USUARIO_ROL>> resultado = MiSqlHelper.EjecutarComando(
                    query,
                    null,
                    comando =>
                    {
                        var usuarioRoles = new List<USUARIO_ROL>();
                        using (var lector = comando.ExecuteReader())
                        {
                            while (lector.Read())
                            {
                                usuarioRoles.Add(new USUARIO_ROL
                                {
                                    USU_ID = lector.GetInt32(0),
                                    ROL_ID = lector.GetInt32(1)
                                });
                            }
                        }
                        return usuarioRoles;
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

                // Creamos el objeto de la página de usuario roles
                var itemsPagina = new Items_pagina<USUARIO_ROL>
                {
                    Items = resultado.Datos,
                    Total_items = totalRegistros.Datos
                };

                return new DTO<Items_pagina<USUARIO_ROL>>
                {
                    Datos = itemsPagina,
                    Correcto = true,
                    Mensaje = "Usuarios y roles de la página obtenidos correctamente"
                };
            }
            catch (Exception ex)
            {
                return new DTO<Items_pagina<USUARIO_ROL>>
                {
                    Datos = null,
                    Correcto = false,
                    Mensaje = $"Error al obtener la lista de usuarios y roles de la página: {ex.Message}"
                };
            }
        }
    }
}

