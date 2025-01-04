using AdmIn.Data.Utilitarios;
using AdmIn.Data.Entidades;
using AdmIn.Common;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AdmIn.Data.Repositorios
{
    public class Rep_USUARIO_PERMISO
    {
        public DTO<bool> Insertar(USUARIO_PERMISO usuarioPermiso)
        {
            return MiSqlHelper.EjecutarComando("sp_UsuarioPermiso_Insertar",
                comando =>
                {
                    comando.Parameters.AddWithValue("@USU_ID", usuarioPermiso.USU_ID);
                    comando.Parameters.AddWithValue("@PERM_ID", usuarioPermiso.PERM_ID);
                },
                comando =>
                {
                    comando.ExecuteNonQuery();
                    return true;
                });
        }

        public DTO<List<USUARIO_PERMISO>> ObtenerPorUsuario(int usuId)
        {
            return MiSqlHelper.EjecutarComando("sp_UsuarioPermiso_ObtenerPorUsuario",
                comando => comando.Parameters.AddWithValue("@USU_ID", usuId),
                comando =>
                {
                    var lista = new List<USUARIO_PERMISO>();
                    using (var lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            lista.Add(new USUARIO_PERMISO
                            {
                                USU_ID = lector.GetInt32(0),
                                PERM_ID = lector.GetInt32(1)
                            });
                        }
                    }
                    return lista;
                });
        }

        public DTO<bool> Eliminar(int usuId, int permId)
        {
            return MiSqlHelper.EjecutarComando("sp_UsuarioPermiso_Eliminar",
                comando =>
                {
                    comando.Parameters.AddWithValue("@USU_ID", usuId);
                    comando.Parameters.AddWithValue("@PERM_ID", permId);
                },
                comando =>
                {
                    comando.ExecuteNonQuery();
                    return true;
                });
        }
    }
}
