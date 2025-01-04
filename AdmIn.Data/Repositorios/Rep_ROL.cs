using AdmIn.Data.Utilitarios;
using AdmIn.Data.Entidades;
using AdmIn.Common;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AdmIn.Data.Repositorios
{
    public class Rep_ROL
    {
        public DTO<ROL> Insertar(ROL rol)
        {
            if (string.IsNullOrWhiteSpace(rol.ROL_NOMBRE))
                return new DTO<ROL>
                {
                    Correcto = false,
                    Mensaje = "El nombre del rol no puede estar vacío."
                };

            return MiSqlHelper.EjecutarComando("sp_Rol_Insertar",
                comando =>
                {
                    comando.Parameters.AddWithValue("@ROL_NOMBRE", rol.ROL_NOMBRE);
                    var nuevoId = new SqlParameter("@NuevoId", SqlDbType.Int) { Direction = ParameterDirection.Output };
                    comando.Parameters.Add(nuevoId);
                },
                comando =>
                {
                    comando.ExecuteNonQuery();
                    rol.ROL_ID = (int)comando.Parameters["@NuevoId"].Value;
                    return rol;
                });
        }

        public DTO<ROL> Actualizar(ROL rol)
        {
            if (string.IsNullOrWhiteSpace(rol.ROL_NOMBRE))
                return new DTO<ROL>
                {
                    Correcto = false,
                    Mensaje = "El nombre del rol no puede estar vacío."
                };

            return MiSqlHelper.EjecutarComando("sp_Rol_Actualizar",
                comando =>
                {
                    comando.Parameters.AddWithValue("@ROL_ID", rol.ROL_ID);
                    comando.Parameters.AddWithValue("@ROL_NOMBRE", rol.ROL_NOMBRE);
                },
                comando =>
                {
                    comando.ExecuteNonQuery();
                    return rol;
                });
        }

        public DTO<bool> Eliminar(int id)
        {
            return MiSqlHelper.EjecutarComando("sp_Rol_Eliminar",
                comando => comando.Parameters.AddWithValue("@ROL_ID", id),
                comando =>
                {
                    comando.ExecuteNonQuery();
                    return true;
                });
        }

        public DTO<ROL?> Obtener_por_Id(int id)
        {
            return MiSqlHelper.EjecutarComando("sp_Rol_ObtenerPorId",
                comando => comando.Parameters.AddWithValue("@ROL_ID", id),
                comando =>
                {
                    using (var lector = comando.ExecuteReader())
                    {
                        if (lector.Read())
                        {
                            return new ROL
                            {
                                ROL_ID = lector.GetInt32(0),
                                ROL_NOMBRE = lector.GetString(1)
                            };
                        }
                        return null;
                    }
                });
        }

        public DTO<List<ROL>> Obtener_todos()
        {
            return MiSqlHelper.EjecutarComando("sp_Rol_ObtenerTodos",
                null,
                comando =>
                {
                    var lista = new List<ROL>();
                    using (var lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            lista.Add(new ROL
                            {
                                ROL_ID = lector.GetInt32(0),
                                ROL_NOMBRE = lector.GetString(1)
                            });
                        }
                    }
                    return lista;
                });
        }
    }
}
