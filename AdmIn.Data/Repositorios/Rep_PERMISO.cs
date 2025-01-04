using AdmIn.Data.Utilitarios;
using AdmIn.Data.Entidades;
using AdmIn.Common;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AdmIn.Data.Repositorios
{
    public class Rep_PERMISO
    {
        public DTO<PERMISO> Insertar(PERMISO permiso)
        {
            if (!permiso.IsValid())
                return new DTO<PERMISO>
                {
                    Correcto = false,
                    Mensaje = "El permiso no es válido."
                };

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

        public DTO<PERMISO> Actualizar(PERMISO permiso)
        {
            if (!permiso.IsValid())
                return new DTO<PERMISO>
                {
                    Correcto = false,
                    Mensaje = "El permiso no es válido."
                };

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

        public DTO<bool> Eliminar(int id)
        {
            return MiSqlHelper.EjecutarComando("sp_Permiso_Eliminar",
                comando => comando.Parameters.AddWithValue("@PERM_ID", id),
                comando =>
                {
                    comando.ExecuteNonQuery();
                    return true;
                });
        }

        public DTO<PERMISO?> Obtener_por_Id(int id)
        {
            return MiSqlHelper.EjecutarComando("sp_Permiso_ObtenerPorId",
                comando => comando.Parameters.AddWithValue("@PERM_ID", id),
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

        public DTO<List<PERMISO>> Obtener_todos()
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
                    return lista;
                });
        }
    }
}


