using AdmIn.Common;
using AdmIn.Common.Entidades;
using AdmIn.Common.Repositorios;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace AdmIn.Data.Repositorios
{
 public class ReservaRepository : IReservaRepository
 {
 public ReservaRepository()
 {
 }

 public async Task<DTO<Reserva>> Crear(Reserva reserva)
 {
 using var conexion = new SqlConnection(InfoSQL.Conexion);
 await conexion.OpenAsync();
 using var transaccion = conexion.BeginTransaction();

 try
 {
 var sqlInsert = @"INSERT INTO Reserva (InmuebleId, UsuarioReservadorId, FechaCreacion, FechaVencimiento, Costo, MonedaId, Estado, AplicadoAlContrato, MontoAplicadoAlContrato, FechaModificacion, UsuarioCreadorId, UsuarioModificadorId)
 VALUES (@InmuebleId, @UsuarioReservadorId, @FechaCreacion, @FechaVencimiento, @Costo, @MonedaId, @Estado, @AplicadoAlContrato, @MontoAplicadoAlContrato, GETDATE(), @UsuarioCreadorId, @UsuarioModificadorId);
 SELECT SCOPE_IDENTITY() AS NuevoId;";

 var nuevoId = await conexion.QuerySingleAsync<int>(sqlInsert, new
 {
 reserva.InmuebleId,
 reserva.UsuarioReservadorId,
 FechaCreacion = reserva.FechaCreacion,
 FechaVencimiento = reserva.FechaVencimiento,
 reserva.Costo,
 MonedaId = (object?)reserva.MonedaId ?? DBNull.Value,
 Estado = (int)reserva.Estado,
 AplicadoAlContrato = reserva.AplicadoAlContrato,
 MontoAplicadoAlContrato = (object?)reserva.MontoAplicadoAlContrato ?? DBNull.Value,
 UsuarioCreadorId = (object?)reserva.UsuarioCreadorId ?? DBNull.Value,
 UsuarioModificadorId = (object?)reserva.UsuarioModificadorId ?? DBNull.Value
 }, transaccion);

 var reservaCreada = await conexion.QuerySingleOrDefaultAsync<Reserva>("SELECT * FROM Reserva WHERE Id = @Id", new { Id = nuevoId }, transaccion);

 transaccion.Commit();

 return new DTO<Reserva> { Correcto = true, Datos = reservaCreada, Mensaje = "Reserva creada." };
 }
 catch (System.Exception ex)
 {
 transaccion.Rollback();
 return new DTO<Reserva> { Correcto = false, Mensaje = ex.Message };
 }
 }

 public async Task<DTO<Reserva>> Obtener_por_id(int id)
 {
 using var conexion = new SqlConnection(InfoSQL.Conexion);
 await conexion.OpenAsync();

 var reserva = await conexion.QuerySingleOrDefaultAsync<Reserva>("SELECT * FROM Reserva WHERE Id = @Id", new { Id = id });
 if (reserva == null)
 return new DTO<Reserva> { Correcto = false, Mensaje = "Reserva no encontrada." };

 return new DTO<Reserva> { Correcto = true, Datos = reserva };
 }

 public async Task<DTO<Reserva>> Actualizar(Reserva reserva)
 {
 using var conexion = new SqlConnection(InfoSQL.Conexion);
 await conexion.OpenAsync();
 using var transaccion = conexion.BeginTransaction();

 try
 {
 var sql = @"UPDATE Reserva SET FechaVencimiento = @FechaVencimiento, Costo = @Costo, MonedaId = @MonedaId, Estado = @Estado, AplicadoAlContrato = @AplicadoAlContrato, MontoAplicadoAlContrato = @MontoAplicadoAlContrato, FechaModificacion = GETDATE(), UsuarioModificadorId = @UsuarioModificadorId WHERE Id = @Id";

 var filas = await conexion.ExecuteAsync(sql, new
 {
 reserva.FechaVencimiento,
 reserva.Costo,
 MonedaId = (object?)reserva.MonedaId ?? DBNull.Value,
 Estado = (int)reserva.Estado,
 AplicadoAlContrato = reserva.AplicadoAlContrato,
 MontoAplicadoAlContrato = (object?)reserva.MontoAplicadoAlContrato ?? DBNull.Value,
 UsuarioModificadorId = (object?)reserva.UsuarioModificadorId ?? DBNull.Value,
 reserva.Id
 }, transaccion);

 if (filas ==0) throw new System.Exception("No se actualizó la reserva.");

 var reservaAct = await conexion.QuerySingleOrDefaultAsync<Reserva>("SELECT * FROM Reserva WHERE Id = @Id", new { Id = reserva.Id }, transaccion);
 transaccion.Commit();
 return new DTO<Reserva> { Correcto = true, Datos = reservaAct, Mensaje = "Reserva actualizada." };
 }
 catch (System.Exception ex)
 {
 transaccion.Rollback();
 return new DTO<Reserva> { Correcto = false, Mensaje = ex.Message };
 }
 }

 public async Task<DTO<IEnumerable<Reserva>>> Obtener_activas_por_inmueble(int inmuebleId)
 {
 using var conexion = new SqlConnection(InfoSQL.Conexion);
 await conexion.OpenAsync();

 var lista = await conexion.QueryAsync<Reserva>("SELECT * FROM Reserva WHERE InmuebleId = @InmuebleId AND Estado = @Estado", new { InmuebleId = inmuebleId, Estado = (int)ReservaEstado.Activa });
 return new DTO<IEnumerable<Reserva>> { Correcto = true, Datos = lista };
 }

 public async Task<DTO<IEnumerable<Reserva>>> Obtener_vencidas(System.DateTime ahora)
 {
 using var conexion = new SqlConnection(InfoSQL.Conexion);
 await conexion.OpenAsync();

 var lista = await conexion.QueryAsync<Reserva>("SELECT * FROM Reserva WHERE Estado = @Estado AND FechaVencimiento < @Ahora", new { Estado = (int)ReservaEstado.Activa, Ahora = ahora });
 return new DTO<IEnumerable<Reserva>> { Correcto = true, Datos = lista };
 }
 }
}
