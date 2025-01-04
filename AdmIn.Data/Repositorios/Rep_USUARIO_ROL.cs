using AdmIn.Data.Utilitarios;
using AdmIn.Data.Entidades;
using AdmIn.Common;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Collections.Generic;

namespace AdmIn.Data.Repositorios
{
    public class Rep_USUARIO_ROL
    {
        public DTO<bool> Insertar(USUARIO_ROL usuarioRol)
        {
            return MiSqlHelper.EjecutarComando("sp_UsuarioRol_Insertar",
                comando =>
                {
                    comando.Parameters.AddWithValue("@USU_ID", usuarioRol.USU_ID);
                    comando.Parameters.AddWithValue("@ROL_ID", usuarioRol.ROL_ID);
                },
                comando =>
                {
                    comando.ExecuteNonQuery();
                    return true;
                });
        }

        public DTO<List<USUARIO_ROL>> ObtenerPorUsuario(int usuId)
        {
            return MiSqlHelper.EjecutarComando("sp_UsuarioRol_ObtenerPorUsuario",
                comando => comando.Parameters.AddWithValue("@USU_ID", usuId),
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
                    return lista;
                });
        }

        public DTO<bool> Eliminar(int usuId, int rolId)
        {
            return MiSqlHelper.EjecutarComando("sp_UsuarioRol_Eliminar",
                comando =>
                {
                    comando.Parameters.AddWithValue("@USU_ID", usuId);
                    comando.Parameters.AddWithValue("@ROL_ID", rolId);
                },
                comando =>
                {
                    comando.ExecuteNonQuery();
                    return true;
                });
        }
    }
}
