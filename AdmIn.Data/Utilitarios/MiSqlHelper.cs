using AdmIn.Common;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmIn.Data.Utilitarios
{
    public static class MiSqlHelper
    {
        public static DTO<T> EjecutarComando<T>(
                                    string procedimiento, 
                                    Action<SqlCommand> configurarParametros, 
                                    Func<SqlCommand, T> procesarResultados,
                                    CommandType tipoComando = CommandType.StoredProcedure)
        {
            try
            {
                using (var conexion = new SqlConnection(InfoSQL.Conexion))
                {
                    conexion.Open();
                    using (var comando = new SqlCommand(procedimiento, conexion))
                    {
                        comando.CommandType = tipoComando;
                        configurarParametros?.Invoke(comando);

                        return new DTO<T>
                        {
                            Correcto = true,
                            Datos = procesarResultados(comando),
                            Mensaje = "Operación realizada con éxito."
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new DTO<T>
                {
                    Correcto = false,
                    Datos = default,
                    Mensaje = $"Error al ejecutar el procedimiento: {ex.Message}"
                };
            }
        }
    }
}

