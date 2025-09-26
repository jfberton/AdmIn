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
    public class DetalleTrabajoRepository : IDetalleTrabajoRepository
    {
        public async Task<DTO<DetalleTrabajo>> Crear(DetalleTrabajo detalleTrabajo)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            using var transaccion = conexion.BeginTransaction();
            try
            {
                var sql = @"INSERT INTO DetalleTrabajo (TrabajoProveedorId, Fecha, Descripcion, Costo, Estado, UsuarioId)
                            OUTPUT INSERTED.DetalleTrabajoId, INSERTED.TrabajoProveedorId, INSERTED.Fecha, INSERTED.Descripcion, INSERTED.Costo, INSERTED.Estado, INSERTED.UsuarioId
                            VALUES (@TrabajoProveedorId, @Fecha, @Descripcion, @Costo, @Estado, @UsuarioId);";
                var creado = await conexion.QuerySingleOrDefaultAsync<DetalleTrabajo>(sql, detalleTrabajo, transaccion);
                if (creado == null)
                    throw new Exception("No se pudo crear el detalle de trabajo.");
                transaccion.Commit();
                return new DTO<DetalleTrabajo> { Correcto = true, Datos = creado, Mensaje = "Detalle de trabajo creado correctamente." };
            }
            catch (Exception ex)
            {
                transaccion.Rollback();
                return new DTO<DetalleTrabajo> { Correcto = false, Mensaje = $"Error al crear detalle: {ex.Message}" };
            }
        }

        public async Task<DTO<DetalleTrabajo>> Actualizar(DetalleTrabajo detalleTrabajo)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            using var transaccion = conexion.BeginTransaction();
            try
            {
                var sql = @"UPDATE DetalleTrabajo SET TrabajoProveedorId=@TrabajoProveedorId, Fecha=@Fecha, Descripcion=@Descripcion, Costo=@Costo, Estado=@Estado, UsuarioId=@UsuarioId
                            OUTPUT INSERTED.DetalleTrabajoId, INSERTED.TrabajoProveedorId, INSERTED.Fecha, INSERTED.Descripcion, INSERTED.Costo, INSERTED.Estado, INSERTED.UsuarioId
                            WHERE DetalleTrabajoId=@DetalleTrabajoId;";
                var actualizado = await conexion.QuerySingleOrDefaultAsync<DetalleTrabajo>(sql, detalleTrabajo, transaccion);
                if (actualizado == null)
                    throw new Exception("No se pudo actualizar el detalle de trabajo.");
                transaccion.Commit();
                return new DTO<DetalleTrabajo> { Correcto = true, Datos = actualizado, Mensaje = "Detalle de trabajo actualizado correctamente." };
            }
            catch (Exception ex)
            {
                transaccion.Rollback();
                return new DTO<DetalleTrabajo> { Correcto = false, Mensaje = $"Error al actualizar detalle: {ex.Message}" };
            }
        }

        public async Task<DTO<bool>> Eliminar(DetalleTrabajo detalleTrabajo)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            using var transaccion = conexion.BeginTransaction();
            try
            {
                var sql = "DELETE FROM DetalleTrabajo WHERE DetalleTrabajoId=@DetalleTrabajoId;";
                var filas = await conexion.ExecuteAsync(sql, new { detalleTrabajo.DetalleTrabajoId }, transaccion);
                transaccion.Commit();
                return new DTO<bool> { Correcto = filas > 0, Datos = filas > 0, Mensaje = filas > 0 ? "Detalle eliminado correctamente." : "No se encontró el detalle." };
            }
            catch (Exception ex)
            {
                transaccion.Rollback();
                return new DTO<bool> { Correcto = false, Mensaje = $"Error al eliminar detalle: {ex.Message}" };
            }
        }

        public async Task<DTO<DetalleTrabajo>> Obtener_por_id(int detalleTrabajoId)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            var sql = "SELECT * FROM DetalleTrabajo WHERE DetalleTrabajoId=@detalleTrabajoId;";
            var encontrado = await conexion.QuerySingleOrDefaultAsync<DetalleTrabajo>(sql, new { detalleTrabajoId });
            if (encontrado == null)
                return new DTO<DetalleTrabajo> { Correcto = false, Mensaje = "Detalle no encontrado" };
            return new DTO<DetalleTrabajo> { Correcto = true, Datos = encontrado, Mensaje = "Detalle obtenido correctamente" };
        }

        public async Task<DTO<IEnumerable<DetalleTrabajo>>> Obtener_todos()
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            var sql = "SELECT * FROM DetalleTrabajo ORDER BY Fecha DESC;";
            var lista = await conexion.QueryAsync<DetalleTrabajo>(sql);
            return new DTO<IEnumerable<DetalleTrabajo>> { Correcto = true, Datos = lista, Mensaje = "Detalles obtenidos correctamente" };
        }

        public async Task<DTO<Items_pagina<DetalleTrabajo>>> Obtener_paginado(Filtros_paginado filtros)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            var sql = @"SELECT COUNT(*) OVER() AS TotalItems, *
                        FROM DetalleTrabajo
                        WHERE (@FiltroBusqueda IS NULL OR Descripcion LIKE '%' + @FiltroBusqueda + '%' OR Estado LIKE '%' + @FiltroBusqueda + '%')
                        ORDER BY Fecha DESC
                        OFFSET @Skip ROWS FETCH NEXT @Top ROWS ONLY;";
            var lista = await conexion.QueryAsync<dynamic>(sql, new
            {
                FiltroBusqueda = (object?)filtros.Filter ?? DBNull.Value,
                Skip = filtros.Skip < 0 ? 0 : filtros.Skip,
                Top = filtros.Top <= 0 ? 10 : filtros.Top
            });
            var detalles = new List<DetalleTrabajo>();
            int totalItems = 0;
            foreach (var row in lista)
            {
                totalItems = row.TotalItems;
                detalles.Add(new DetalleTrabajo
                {
                    DetalleTrabajoId = row.DetalleTrabajoId,
                    TrabajoProveedorId = row.TrabajoProveedorId,
                    Fecha = row.Fecha,
                    Descripcion = row.Descripcion,
                    Costo = row.Costo,
                    Estado = row.Estado,
                    UsuarioId = row.UsuarioId
                });
            }
            return new DTO<Items_pagina<DetalleTrabajo>>
            {
                Correcto = true,
                Datos = new Items_pagina<DetalleTrabajo> { Total_items = totalItems, Items = detalles },
                Mensaje = "Detalles paginados correctamente."
            };
        }
    }
}
