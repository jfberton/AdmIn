using AdmIn.Common;
using AdmIn.Common.Entidades;
using AdmIn.Common.Repositorios;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace AdmIn.Data.Repositorios
{
    public class HistorialDetalleTrabajoRepository : IHistorialDetalleTrabajoRepository
    {
        public async Task<DTO<HistorialDetalleTrabajo>> Crear(HistorialDetalleTrabajo historialDetalleTrabajo)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            using var transaccion = conexion.BeginTransaction();
            try
            {
                var sql = @"INSERT INTO HistorialDetalleTrabajo (DetalleTrabajoId, Fecha, Estado, UsuarioId, Comentario)
                            OUTPUT INSERTED.HistorialDetalleTrabajoId, INSERTED.DetalleTrabajoId, INSERTED.Fecha, INSERTED.Estado, INSERTED.UsuarioId, INSERTED.Comentario
                            VALUES (@DetalleTrabajoId, @Fecha, @Estado, @UsuarioId, @Comentario);";
                var creado = await conexion.QuerySingleOrDefaultAsync<HistorialDetalleTrabajo>(sql, historialDetalleTrabajo, transaccion);
                if (creado == null)
                    throw new Exception("No se pudo crear el historial de detalle de trabajo.");
                transaccion.Commit();
                return new DTO<HistorialDetalleTrabajo> { Correcto = true, Datos = creado, Mensaje = "Historial de detalle de trabajo creado correctamente." };
            }
            catch (Exception ex)
            {
                transaccion.Rollback();
                return new DTO<HistorialDetalleTrabajo> { Correcto = false, Mensaje = $"Error al crear historial: {ex.Message}" };
            }
        }

        public async Task<DTO<HistorialDetalleTrabajo>> Actualizar(HistorialDetalleTrabajo historialDetalleTrabajo)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            using var transaccion = conexion.BeginTransaction();
            try
            {
                var sql = @"UPDATE HistorialDetalleTrabajo SET DetalleTrabajoId=@DetalleTrabajoId, Fecha=@Fecha, Estado=@Estado, UsuarioId=@UsuarioId, Comentario=@Comentario
                            OUTPUT INSERTED.HistorialDetalleTrabajoId, INSERTED.DetalleTrabajoId, INSERTED.Fecha, INSERTED.Estado, INSERTED.UsuarioId, INSERTED.Comentario
                            WHERE HistorialDetalleTrabajoId=@HistorialDetalleTrabajoId;";
                var actualizado = await conexion.QuerySingleOrDefaultAsync<HistorialDetalleTrabajo>(sql, historialDetalleTrabajo, transaccion);
                if (actualizado == null)
                    throw new Exception("No se pudo actualizar el historial de detalle de trabajo.");
                transaccion.Commit();
                return new DTO<HistorialDetalleTrabajo> { Correcto = true, Datos = actualizado, Mensaje = "Historial de detalle de trabajo actualizado correctamente." };
            }
            catch (Exception ex)
            {
                transaccion.Rollback();
                return new DTO<HistorialDetalleTrabajo> { Correcto = false, Mensaje = $"Error al actualizar historial: {ex.Message}" };
            }
        }

        public async Task<DTO<bool>> Eliminar(HistorialDetalleTrabajo historialDetalleTrabajo)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            using var transaccion = conexion.BeginTransaction();
            try
            {
                var sql = "DELETE FROM HistorialDetalleTrabajo WHERE HistorialDetalleTrabajoId=@HistorialDetalleTrabajoId;";
                var filas = await conexion.ExecuteAsync(sql, new { historialDetalleTrabajo.HistorialDetalleTrabajoId }, transaccion);
                transaccion.Commit();
                return new DTO<bool> { Correcto = filas > 0, Datos = filas > 0, Mensaje = filas > 0 ? "Historial eliminado correctamente." : "No se encontró el historial." };
            }
            catch (Exception ex)
            {
                transaccion.Rollback();
                return new DTO<bool> { Correcto = false, Mensaje = $"Error al eliminar historial: {ex.Message}" };
            }
        }

        public async Task<DTO<HistorialDetalleTrabajo>> Obtener_por_id(int historialDetalleTrabajoId)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            var sql = "SELECT * FROM HistorialDetalleTrabajo WHERE HistorialDetalleTrabajoId=@historialDetalleTrabajoId;";
            var encontrado = await conexion.QuerySingleOrDefaultAsync<HistorialDetalleTrabajo>(sql, new { historialDetalleTrabajoId });
            if (encontrado == null)
                return new DTO<HistorialDetalleTrabajo> { Correcto = false, Mensaje = "Historial no encontrado" };
            return new DTO<HistorialDetalleTrabajo> { Correcto = true, Datos = encontrado, Mensaje = "Historial obtenido correctamente" };
        }

        public async Task<DTO<IEnumerable<HistorialDetalleTrabajo>>> Obtener_todos()
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            var sql = "SELECT * FROM HistorialDetalleTrabajo ORDER BY Fecha DESC;";
            var lista = await conexion.QueryAsync<HistorialDetalleTrabajo>(sql);
            return new DTO<IEnumerable<HistorialDetalleTrabajo>> { Correcto = true, Datos = lista, Mensaje = "Historiales obtenidos correctamente" };
        }

        public async Task<DTO<Items_pagina<HistorialDetalleTrabajo>>> Obtener_paginado(Filtros_paginado filtros)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            var sql = @"SELECT COUNT(*) OVER() AS TotalItems, *
                        FROM HistorialDetalleTrabajo
                        WHERE (@FiltroBusqueda IS NULL OR Estado LIKE '%' + @FiltroBusqueda + '%' OR Comentario LIKE '%' + @FiltroBusqueda + '%')
                        ORDER BY Fecha DESC
                        OFFSET @Skip ROWS FETCH NEXT @Top ROWS ONLY;";
            var lista = await conexion.QueryAsync<dynamic>(sql, new
            {
                FiltroBusqueda = (object?)filtros.Filter ?? DBNull.Value,
                Skip = filtros.Skip < 0 ? 0 : filtros.Skip,
                Top = filtros.Top <= 0 ? 10 : filtros.Top
            });
            var historiales = new List<HistorialDetalleTrabajo>();
            int totalItems = 0;
            foreach (var row in lista)
            {
                totalItems = row.TotalItems;
                historiales.Add(new HistorialDetalleTrabajo
                {
                    HistorialDetalleTrabajoId = row.HistorialDetalleTrabajoId,
                    DetalleTrabajoId = row.DetalleTrabajoId,
                    Fecha = row.Fecha,
                    Estado = row.Estado,
                    UsuarioId = row.UsuarioId,
                    Comentario = row.Comentario
                });
            }
            return new DTO<Items_pagina<HistorialDetalleTrabajo>>
            {
                Correcto = true,
                Datos = new Items_pagina<HistorialDetalleTrabajo> { Total_items = totalItems, Items = historiales },
                Mensaje = "Historiales paginados correctamente."
            };
        }
    }
}
