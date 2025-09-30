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
    public class HistorialTrabajoRepository : IHistorialTrabajoRepository
    {
        public async Task<DTO<HistorialTrabajo>> Crear(HistorialTrabajo historialTrabajo)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            using var transaccion = conexion.BeginTransaction();
            try
            {
                var sql = @"INSERT INTO HistorialTrabajo (TrabajoProveedorId, Fecha, Estado, UsuarioId, Comentario)
                            OUTPUT INSERTED.HistorialTrabajoId, INSERTED.TrabajoProveedorId, INSERTED.Fecha, INSERTED.Estado, INSERTED.UsuarioId, INSERTED.Comentario
                            VALUES (@TrabajoProveedorId, @Fecha, @Estado, @UsuarioId, @Comentario);";
                var creado = await conexion.QuerySingleOrDefaultAsync<HistorialTrabajo>(sql, historialTrabajo, transaccion);
                if (creado == null)
                    throw new Exception("No se pudo crear el historial de trabajo.");
                transaccion.Commit();
                return new DTO<HistorialTrabajo> { Correcto = true, Datos = creado, Mensaje = "Historial de trabajo creado correctamente." };
            }
            catch (Exception ex)
            {
                transaccion.Rollback();
                return new DTO<HistorialTrabajo> { Correcto = false, Mensaje = $"Error al crear historial: {ex.Message}" };
            }
        }

        public async Task<DTO<HistorialTrabajo>> Actualizar(HistorialTrabajo historialTrabajo)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            using var transaccion = conexion.BeginTransaction();
            try
            {
                var sql = @"UPDATE HistorialTrabajo SET TrabajoProveedorId=@TrabajoProveedorId, Fecha=@Fecha, Estado=@Estado, UsuarioId=@UsuarioId, Comentario=@Comentario
                            OUTPUT INSERTED.HistorialTrabajoId, INSERTED.TrabajoProveedorId, INSERTED.Fecha, INSERTED.Estado, INSERTED.UsuarioId, INSERTED.Comentario
                            WHERE HistorialTrabajoId=@HistorialTrabajoId;";
                var actualizado = await conexion.QuerySingleOrDefaultAsync<HistorialTrabajo>(sql, historialTrabajo, transaccion);
                if (actualizado == null)
                    throw new Exception("No se pudo actualizar el historial de trabajo.");
                transaccion.Commit();
                return new DTO<HistorialTrabajo> { Correcto = true, Datos = actualizado, Mensaje = "Historial de trabajo actualizado correctamente." };
            }
            catch (Exception ex)
            {
                transaccion.Rollback();
                return new DTO<HistorialTrabajo> { Correcto = false, Mensaje = $"Error al actualizar historial: {ex.Message}" };
            }
        }

        public async Task<DTO<bool>> Eliminar(HistorialTrabajo historialTrabajo)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            using var transaccion = conexion.BeginTransaction();
            try
            {
                var sql = "DELETE FROM HistorialTrabajo WHERE HistorialTrabajoId=@HistorialTrabajoId;";
                var filas = await conexion.ExecuteAsync(sql, new { historialTrabajo.HistorialTrabajoId }, transaccion);
                transaccion.Commit();
                return new DTO<bool> { Correcto = filas > 0, Datos = filas > 0, Mensaje = filas > 0 ? "Historial eliminado correctamente." : "No se encontró el historial." };
            }
            catch (Exception ex)
            {
                transaccion.Rollback();
                return new DTO<bool> { Correcto = false, Mensaje = $"Error al eliminar historial: {ex.Message}" };
            }
        }

        public async Task<DTO<HistorialTrabajo>> Obtener_por_id(int historialTrabajoId)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            var sql = "SELECT * FROM HistorialTrabajo WHERE HistorialTrabajoId=@historialTrabajoId;";
            var encontrado = await conexion.QuerySingleOrDefaultAsync<HistorialTrabajo>(sql, new { historialTrabajoId });
            if (encontrado == null)
                return new DTO<HistorialTrabajo> { Correcto = false, Mensaje = "Historial no encontrado" };
            return new DTO<HistorialTrabajo> { Correcto = true, Datos = encontrado, Mensaje = "Historial obtenido correctamente" };
        }

        public async Task<DTO<IEnumerable<HistorialTrabajo>>> Obtener_todos()
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            var sql = "SELECT * FROM HistorialTrabajo ORDER BY Fecha DESC;";
            var lista = await conexion.QueryAsync<HistorialTrabajo>(sql);
            return new DTO<IEnumerable<HistorialTrabajo>> { Correcto = true, Datos = lista, Mensaje = "Historiales obtenidos correctamente" };
        }

        public async Task<DTO<Items_pagina<HistorialTrabajo>>> Obtener_paginado(Filtros_paginado filtros)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            var sql = @"SELECT COUNT(*) OVER() AS TotalItems, *
                        FROM HistorialTrabajo
                        WHERE (@FiltroBusqueda IS NULL OR Estado LIKE '%' + @FiltroBusqueda + '%' OR Comentario LIKE '%' + @FiltroBusqueda + '%')
                        ORDER BY Fecha DESC
                        OFFSET @Skip ROWS FETCH NEXT @Top ROWS ONLY;";
            var lista = await conexion.QueryAsync<dynamic>(sql, new
            {
                FiltroBusqueda = (object?)filtros.Filter ?? DBNull.Value,
                Skip = filtros.Skip < 0 ? 0 : filtros.Skip,
                Top = filtros.Top <= 0 ? 10 : filtros.Top
            });
            var historiales = new List<HistorialTrabajo>();
            int totalItems = 0;
            foreach (var row in lista)
            {
                totalItems = row.TotalItems;
                historiales.Add(new HistorialTrabajo
                {
                    HistorialTrabajoId = row.HistorialTrabajoId,
                    TrabajoProveedorId = row.TrabajoProveedorId,
                    Fecha = row.Fecha,
                    Estado = row.Estado,
                    UsuarioId = row.UsuarioId,
                    Comentario = row.Comentario
                });
            }
            return new DTO<Items_pagina<HistorialTrabajo>>
            {
                Correcto = true,
                Datos = new Items_pagina<HistorialTrabajo> { Total_items = totalItems, Items = historiales },
                Mensaje = "Historiales paginados correctamente."
            };
        }

        public async Task<DTO<IEnumerable<HistorialTrabajo>>> Obtener_por_trabajo(int trabajoId)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();

            try
            {
                var sql = "SELECT * FROM HistorialTrabajo WHERE TrabajoProveedorId = @trabajoId ORDER BY Fecha DESC;";
                var lista = await conexion.QueryAsync<HistorialTrabajo>(sql, new { trabajoId });
                return new DTO<IEnumerable<HistorialTrabajo>> { Correcto = true, Datos = lista.ToList(), Mensaje = "Historiales por trabajo obtenidos correctamente" };
            }
            catch (Exception ex)
            {
                return new DTO<IEnumerable<HistorialTrabajo>> { Correcto = false, Datos = new List<HistorialTrabajo>(), Mensaje = ex.Message };
            }
        }
    }
}
