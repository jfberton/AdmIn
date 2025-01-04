using System;
using System.Collections.Generic;
using System.Data;
using AdmIn.Common;
using AdmIn.Data.Entidades;
using AdmIn.Data.Utilitarios;
using Microsoft.Data.SqlClient;

namespace AdmIn.Data.Repositorios
{
    public class Rep_TIPO_TELEFONO
    {
        public int Insertar(string descripcion)
        {
            try
            {
                using (var conexion = new SqlConnection(InfoSQL.Conexion))
                {
                    conexion.Open();
                    using (var comando = new SqlCommand("sp_tipo_telefono_insertar", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.AddWithValue("@TPT_DESCRIPCION", descripcion);

                        var nuevoId = comando.ExecuteScalar();
                        return nuevoId != null ? Convert.ToInt32(nuevoId) : -1;
                    }
                }
            }
            catch (Exception)
            {
                return -1; // Error
            }
        }

        public TIPO_TELEFONO ObtenerPorId(int id)
        {
            try
            {
                using (var conexion = new SqlConnection(InfoSQL.Conexion))
                {
                    conexion.Open();
                    using (var comando = new SqlCommand("sp_tipo_telefono_obtener_por_id", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.AddWithValue("@TIPO_TEL_ID", id);

                        using (var lector = comando.ExecuteReader())
                        {
                            if (lector.Read())
                            {
                                return new TIPO_TELEFONO
                                {
                                    TPT_ID = lector.GetInt32(0),
                                    TPT_DESCRIPCION = lector.GetString(1)
                                };
                            }
                        }
                    }
                }
                return null; // No encontrado
            }
            catch (Exception)
            {
                return null; // Error
            }
        }

        public List<TIPO_TELEFONO> ObtenerPorDescripcion(string descripcion)
        {
            var lista = new List<TIPO_TELEFONO>();
            try
            {
                using (var conexion = new SqlConnection(InfoSQL.Conexion))
                {
                    conexion.Open();
                    using (var comando = new SqlCommand("sp_tipo_telefono_obtener_por_descripcion", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.AddWithValue("@TPT_DESCRIPCION", descripcion);

                        using (var lector = comando.ExecuteReader())
                        {
                            while (lector.Read())
                            {
                                lista.Add(new TIPO_TELEFONO
                                {
                                    TPT_ID = lector.GetInt32(0),
                                    TPT_DESCRIPCION = lector.GetString(1)
                                });
                            }
                        }
                    }
                }
                return lista;
            }
            catch (Exception)
            {
                return null; // Error
            }
        }

        public bool Actualizar(int id, string descripcion)
        {
            try
            {
                using (var conexion = new SqlConnection(InfoSQL.Conexion))
                {
                    conexion.Open();
                    using (var comando = new SqlCommand("sp_tipo_telefono_actualizar", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.AddWithValue("@TIPO_TEL_ID", id);
                        comando.Parameters.AddWithValue("@TPT_DESCRIPCION", descripcion);

                        comando.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                return false; // Error
            }
        }

        public bool Eliminar(int id)
        {
            try
            {
                using (var conexion = new SqlConnection(InfoSQL.Conexion))
                {
                    conexion.Open();
                    using (var comando = new SqlCommand("sp_tipo_telefono_eliminar", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.AddWithValue("@TIPO_TEL_ID", id);

                        comando.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                return false; // Error
            }
        }
    }
}
