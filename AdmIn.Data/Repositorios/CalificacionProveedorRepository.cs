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
    public class CalificacionProveedorRepository : ICalificacionProveedorRepository
    {
        public async Task<DTO<CalificacionProveedor>> Crear(CalificacionProveedor calificacionProveedor)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            using var transaccion = conexion.BeginTransaction();
            try
            {
                var sql = @"INSERT INTO CalificacionProveedor (TrabajoProveedorId, ProveedorId, UsuarioId, Valor, Comentario, Fecha)
                            OUTPUT INSERTED.CalificacionProveedorId, INSERTED.TrabajoProveedorId, INSERTED.ProveedorId, INSERTED.UsuarioId, INSERTED.Valor, INSERTED.Comentario, INSERTED.Fecha
                            VALUES (@TrabajoProveedorId, @ProveedorId, @UsuarioId, @Valor, @Comentario, @Fecha);";
                var creado = await conexion.QuerySingleOrDefaultAsync<CalificacionProveedor>(sql, calificacionProveedor, transaccion);
                if (creado == null)
                    throw new Exception("No se pudo crear la calificación.");
                transaccion.Commit();
                return new DTO<CalificacionProveedor> { Correcto = true, Datos = creado, Mensaje = "Calificación creada correctamente." };
            }
            catch (Exception ex)
            {
                transaccion.Rollback();
                return new DTO<CalificacionProveedor> { Correcto = false, Mensaje = $"Error al crear calificación: {ex.Message}" };
            }
        }

        public async Task<DTO<CalificacionProveedor>> Actualizar(CalificacionProveedor calificacionProveedor)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            using var transaccion = conexion.BeginTransaction();
            try
            {
                var sql = @"UPDATE CalificacionProveedor SET TrabajoProveedorId=@TrabajoProveedorId, ProveedorId=@ProveedorId, UsuarioId=@UsuarioId, Valor=@Valor, Comentario=@Comentario, Fecha=@Fecha
                            OUTPUT INSERTED.CalificacionProveedorId, INSERTED.TrabajoProveedorId, INSERTED.ProveedorId, INSERTED.UsuarioId, INSERTED.Valor, INSERTED.Comentario, INSERTED.Fecha
                            WHERE CalificacionProveedorId=@CalificacionProveedorId;";
                var actualizado = await conexion.QuerySingleOrDefaultAsync<CalificacionProveedor>(sql, calificacionProveedor, transaccion);
                if (actualizado == null)
                    throw new Exception("No se pudo actualizar la calificación.");
                transaccion.Commit();
                return new DTO<CalificacionProveedor> { Correcto = true, Datos = actualizado, Mensaje = "Calificación actualizada correctamente." };
            }
            catch (Exception ex)
            {
                transaccion.Rollback();
                return new DTO<CalificacionProveedor> { Correcto = false, Mensaje = $"Error al actualizar calificación: {ex.Message}" };
            }
        }

        public async Task<DTO<bool>> Eliminar(CalificacionProveedor calificacionProveedor)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            using var transaccion = conexion.BeginTransaction();
            try
            {
                var sql = "DELETE FROM CalificacionProveedor WHERE CalificacionProveedorId=@CalificacionProveedorId;";
                var filas = await conexion.ExecuteAsync(sql, new { calificacionProveedor.CalificacionProveedorId }, transaccion);
                transaccion.Commit();
                return new DTO<bool> { Correcto = filas > 0, Datos = filas > 0, Mensaje = filas > 0 ? "Calificación eliminada correctamente." : "No se encontró la calificación." };
            }
            catch (Exception ex)
            {
                transaccion.Rollback();
                return new DTO<bool> { Correcto = false, Mensaje = $"Error al eliminar calificación: {ex.Message}" };
            }
        }

        public async Task<DTO<CalificacionProveedor>> Obtener_por_id(int calificacionProveedorId)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            var sql = "SELECT * FROM CalificacionProveedor WHERE CalificacionProveedorId=@calificacionProveedorId;";
            var encontrado = await conexion.QuerySingleOrDefaultAsync<CalificacionProveedor>(sql, new { calificacionProveedorId });
            if (encontrado == null)
                return new DTO<CalificacionProveedor> { Correcto = false, Mensaje = "Calificación no encontrada" };
            return new DTO<CalificacionProveedor> { Correcto = true, Datos = encontrado, Mensaje = "Calificación obtenida correctamente" };
        }

        public async Task<DTO<IEnumerable<CalificacionProveedor>>> Obtener_todos()
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            var sql = "SELECT * FROM CalificacionProveedor ORDER BY Fecha DESC;";
            var lista = await conexion.QueryAsync<CalificacionProveedor>(sql);
            return new DTO<IEnumerable<CalificacionProveedor>> { Correcto = true, Datos = lista, Mensaje = "Calificaciones obtenidas correctamente" };
        }

        public async Task<DTO<Items_pagina<CalificacionProveedor>>> Obtener_paginado(Filtros_paginado filtros)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            var sql = @"SELECT COUNT(*) OVER() AS TotalItems, *
                        FROM CalificacionProveedor
                        WHERE (@FiltroBusqueda IS NULL OR Comentario LIKE '%' + @FiltroBusqueda + '%')
                        ORDER BY Fecha DESC
                        OFFSET @Skip ROWS FETCH NEXT @Top ROWS ONLY;";
            var lista = await conexion.QueryAsync<dynamic>(sql, new
            {
                FiltroBusqueda = (object?)filtros.Filter ?? DBNull.Value,
                Skip = filtros.Skip < 0 ? 0 : filtros.Skip,
                Top = filtros.Top <= 0 ? 10 : filtros.Top
            });
            var calificaciones = new List<CalificacionProveedor>();
            int totalItems = 0;
            foreach (var row in lista)
            {
                totalItems = row.TotalItems;
                calificaciones.Add(new CalificacionProveedor
                {
                    CalificacionProveedorId = row.CalificacionProveedorId,
                    TrabajoProveedorId = row.TrabajoProveedorId,
                    ProveedorId = row.ProveedorId,
                    UsuarioId = row.UsuarioId,
                    Valor = row.Valor,
                    Comentario = row.Comentario,
                    Fecha = row.Fecha
                });
            }
            return new DTO<Items_pagina<CalificacionProveedor>>
            {
                Correcto = true,
                Datos = new Items_pagina<CalificacionProveedor> { Total_items = totalItems, Items = calificaciones },
                Mensaje = "Calificaciones paginadas correctamente."
            };
        }
    }
}
