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
    public class TrabajoProveedorRepository : ITrabajoProveedorRepository
    {
        public async Task<DTO<TrabajoProveedor>> Crear(TrabajoProveedor trabajo)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            using var transaccion = conexion.BeginTransaction();
            try
            {
                var sql = @"INSERT INTO TrabajoProveedor (InmuebleId, ProveedorId, Fecha, Descripcion, Estado, Costo, Contratoid, FacturaURL, FechaCreacion, UsuarioCreador, FechaInicio, CostoAproximado)
                            OUTPUT INSERTED.Id, INSERTED.InmuebleId, INSERTED.ProveedorId, INSERTED.Fecha, INSERTED.Descripcion, INSERTED.Estado, INSERTED.Costo, INSERTED.Contratoid, INSERTED.FacturaURL, INSERTED.FechaCreacion, INSERTED.UsuarioCreador, INSERTED.FechaInicio, INSERTED.CostoAproximado
                            VALUES (@InmuebleId, @ProveedorId, @Fecha, @Descripcion, @Estado, @Costo, @Contratoid, @FacturaURL, @FechaCreacion, @UsuarioCreador, @FechaInicio, @CostoAproximado);";
                var creado = await conexion.QuerySingleOrDefaultAsync<TrabajoProveedor>(sql, trabajo, transaccion);
                if (creado == null)
                    throw new Exception("No se pudo crear el trabajo.");
                transaccion.Commit();
                return new DTO<TrabajoProveedor> { Correcto = true, Datos = creado, Mensaje = "Trabajo creado correctamente." };
            }
            catch (Exception ex)
            {
                transaccion.Rollback();
                return new DTO<TrabajoProveedor> { Correcto = false, Mensaje = $"Error al crear trabajo: {ex.Message}" };
            }
        }

        public async Task<DTO<TrabajoProveedor>> Actualizar(TrabajoProveedor trabajo)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            using var transaccion = conexion.BeginTransaction();
            try
            {
                var sql = @"UPDATE TrabajoProveedor SET InmuebleId=@InmuebleId, ProveedorId=@ProveedorId, Fecha=@Fecha, Descripcion=@Descripcion, Estado=@Estado, Costo=@Costo, Contratoid=@Contratoid, FacturaURL=@FacturaURL, FechaCreacion=@FechaCreacion, UsuarioCreador=@UsuarioCreador, FechaInicio=@FechaInicio, CostoAproximado=@CostoAproximado
                            OUTPUT INSERTED.Id, INSERTED.InmuebleId, INSERTED.ProveedorId, INSERTED.Fecha, INSERTED.Descripcion, INSERTED.Estado, INSERTED.Costo, INSERTED.Contratoid, INSERTED.FacturaURL, INSERTED.FechaCreacion, INSERTED.UsuarioCreador, INSERTED.FechaInicio, INSERTED.CostoAproximado
                            WHERE Id=@Id;";
                var actualizado = await conexion.QuerySingleOrDefaultAsync<TrabajoProveedor>(sql, trabajo, transaccion);
                if (actualizado == null)
                    throw new Exception("No se pudo actualizar el trabajo.");
                transaccion.Commit();
                return new DTO<TrabajoProveedor> { Correcto = true, Datos = actualizado, Mensaje = "Trabajo actualizado correctamente." };
            }
            catch (Exception ex)
            {
                transaccion.Rollback();
                return new DTO<TrabajoProveedor> { Correcto = false, Mensaje = $"Error al actualizar trabajo: {ex.Message}" };
            }
        }

        public async Task<DTO<bool>> Eliminar(TrabajoProveedor trabajo)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            using var transaccion = conexion.BeginTransaction();
            try
            {
                var sql = "DELETE FROM TrabajoProveedor WHERE Id=@Id;";
                var filas = await conexion.ExecuteAsync(sql, new { trabajo.Id }, transaccion);
                transaccion.Commit();
                return new DTO<bool> { Correcto = filas > 0, Datos = filas > 0, Mensaje = filas > 0 ? "Trabajo eliminado correctamente." : "No se encontró el trabajo." };
            }
            catch (Exception ex)
            {
                transaccion.Rollback();
                return new DTO<bool> { Correcto = false, Mensaje = $"Error al eliminar trabajo: {ex.Message}" };
            }
        }

        public async Task<DTO<TrabajoProveedor>> Obtener_por_id(TrabajoProveedor trabajo)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            var sql = "SELECT * FROM TrabajoProveedor WHERE Id=@Id;";
            var encontrado = await conexion.QuerySingleOrDefaultAsync<TrabajoProveedor>(sql, new { trabajo.Id });
            if (encontrado == null)
                return new DTO<TrabajoProveedor> { Correcto = false, Mensaje = "Trabajo no encontrado" };
            return new DTO<TrabajoProveedor> { Correcto = true, Datos = encontrado, Mensaje = "Trabajo obtenido correctamente" };
        }

        public async Task<DTO<IEnumerable<TrabajoProveedor>>> Obtener_todos()
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            var sql = "SELECT * FROM TrabajoProveedor ORDER BY FechaCreacion DESC;";
            var lista = await conexion.QueryAsync<TrabajoProveedor>(sql);
            return new DTO<IEnumerable<TrabajoProveedor>> { Correcto = true, Datos = lista, Mensaje = "Trabajos obtenidos correctamente" };
        }

        public async Task<DTO<Items_pagina<TrabajoProveedor>>> Obtener_paginado(Filtros_paginado filtros)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            var sql = @"SELECT COUNT(*) OVER() AS TotalItems, *
                        FROM TrabajoProveedor
                        WHERE (@FiltroBusqueda IS NULL OR Descripcion LIKE '%' + @FiltroBusqueda + '%' OR Estado LIKE '%' + @FiltroBusqueda + '%')
                        ORDER BY FechaCreacion DESC
                        OFFSET @Skip ROWS FETCH NEXT @Top ROWS ONLY;";
            var lista = await conexion.QueryAsync<dynamic>(sql, new
            {
                FiltroBusqueda = (object?)filtros.Filter ?? DBNull.Value,
                Skip = filtros.Skip < 0 ? 0 : filtros.Skip,
                Top = filtros.Top <= 0 ? 10 : filtros.Top
            });
            var trabajos = new List<TrabajoProveedor>();
            int totalItems = 0;
            foreach (var row in lista)
            {
                totalItems = row.TotalItems;
                trabajos.Add(new TrabajoProveedor
                {
                    Id = row.Id,
                    InmuebleId = row.InmuebleId,
                    ProveedorId = row.ProveedorId,
                    Fecha = row.Fecha,
                    Descripcion = row.Descripcion,
                    Estado = row.Estado,
                    Costo = row.Costo,
                    Contratoid = row.Contratoid,
                    FacturaURL = row.FacturaURL,
                    FechaCreacion = row.FechaCreacion,
                    UsuarioCreador = row.UsuarioCreador,
                    FechaInicio = row.FechaInicio,
                    CostoAproximado = row.CostoAproximado
                });
            }
            return new DTO<Items_pagina<TrabajoProveedor>>
            {
                Correcto = true,
                Datos = new Items_pagina<TrabajoProveedor> { Total_items = totalItems, Items = trabajos },
                Mensaje = "Trabajos paginados correctamente."
            };
        }
    }
}
