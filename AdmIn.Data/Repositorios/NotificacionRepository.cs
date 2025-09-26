using AdmIn.Common;
using AdmIn.Common.Entidades;
using AdmIn.Common.Repositorios;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace AdmIn.Data.Repositorios
{
    public class NotificacionRepository : INotificacionRepository
    {
        public async Task<DTO<Notificacion>> Crear(Notificacion notificacion)
        {
            using var conn = new SqlConnection(InfoSQL.Conexion);
            await conn.OpenAsync();
            using var tran = conn.BeginTransaction();
            try
            {
                var sql = @"INSERT INTO Notificacion (UsuarioId, Tipo, Mensaje, Leida, Fecha, Payload)
                            OUTPUT INSERTED.NotificacionId, INSERTED.UsuarioId, INSERTED.Tipo, INSERTED.Mensaje, INSERTED.Leida, INSERTED.Fecha, INSERTED.Payload
                            VALUES (@UsuarioId, @Tipo, @Mensaje, @Leida, @Fecha, @Payload);";
                var creado = await conn.QuerySingleOrDefaultAsync<Notificacion>(sql, notificacion, tran);
                tran.Commit();
                return new DTO<Notificacion> { Correcto = true, Datos = creado };
            }
            catch (System.Exception ex)
            {
                tran.Rollback();
                return new DTO<Notificacion> { Correcto = false, Mensaje = ex.Message };
            }
        }

        public async Task<DTO<bool>> MarcarComoLeida(int notificacionId)
        {
            using var conn = new SqlConnection(InfoSQL.Conexion);
            await conn.OpenAsync();
            var sql = "UPDATE Notificacion SET Leida = 1 WHERE NotificacionId = @Id";
            var filas = await conn.ExecuteAsync(sql, new { Id = notificacionId });
            return new DTO<bool> { Correcto = filas > 0, Datos = filas > 0 };
        }

        public async Task<DTO<IEnumerable<Notificacion>>> Obtener_por_usuario(int usuarioId)
        {
            using var conn = new SqlConnection(InfoSQL.Conexion);
            await conn.OpenAsync();
            var sql = "SELECT NotificacionId, UsuarioId, Tipo, Mensaje, Leida, Fecha, Payload FROM Notificacion WHERE UsuarioId = @UsuarioId ORDER BY Fecha DESC";
            var lista = await conn.QueryAsync<Notificacion>(sql, new { UsuarioId = usuarioId });
            return new DTO<IEnumerable<Notificacion>> { Correcto = true, Datos = lista };
        }

        public async Task<DTO<int>> Contar_no_leidas(int usuarioId)
        {
            using var conn = new SqlConnection(InfoSQL.Conexion);
            await conn.OpenAsync();
            var sql = "SELECT COUNT(*) FROM Notificacion WHERE UsuarioId = @UsuarioId AND Leida = 0";
            var count = await conn.QuerySingleAsync<int>(sql, new { UsuarioId = usuarioId });
            return new DTO<int> { Correcto = true, Datos = count };
        }
    }
}
