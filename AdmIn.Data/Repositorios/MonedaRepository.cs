using AdmIn.Common;
using AdmIn.Common.Entidades;
using AdmIn.Common.Repositorios;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace AdmIn.Data.Repositorios
{
    public class MonedaRepository : IMonedaRepository
    {
        public MonedaRepository()
        {
        }

        public async Task<DTO<Moneda>> Crear(Moneda moneda)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            using var transaccion = conexion.BeginTransaction();

            try
            {
                var sqlMoneda = @"INSERT INTO Moneda (Codigo, Nombre) 
                                  OUTPUT INSERTED.MonedaID as Id, INSERTED.Codigo, INSERTED.Nombre
                                  VALUES (@Codigo, @Nombre);";

                var monedaCreada = await conexion.QuerySingleOrDefaultAsync<Moneda>(sqlMoneda, new
                {
                    moneda.Codigo,
                    moneda.Nombre
                }, transaccion);

                if (monedaCreada == null)
                    throw new Exception("No se pudo crear la moneda.");

                transaccion.Commit();

                return new DTO<Moneda>
                {
                    Correcto = true,
                    Datos = monedaCreada,
                    Mensaje = "Moneda creada correctamente."
                };
            }
            catch (Exception ex)
            {
                transaccion.Rollback();
                return new DTO<Moneda>
                {
                    Correcto = false,
                    Mensaje = $"Error al crear moneda: {ex.Message}"
                };
            }
        }

        public async Task<DTO<Moneda>> Actualizar(Moneda moneda)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            using var transaccion = conexion.BeginTransaction();

            try
            {
                var sqlActualizarMoneda = @"UPDATE Moneda
                                           SET Codigo = @Codigo, Nombre = @Nombre
                                           OUTPUT INSERTED.MonedaID as Id, INSERTED.Codigo, INSERTED.Nombre
                                           WHERE MonedaID = @MonedaID;";

                var monedaActualizada = await conexion.QuerySingleOrDefaultAsync<Moneda>(sqlActualizarMoneda, new
                {
                    MonedaID = moneda.Id,
                    moneda.Codigo,
                    moneda.Nombre
                }, transaccion);

                if (monedaActualizada == null)
                    throw new Exception("No se pudo actualizar la moneda.");

                transaccion.Commit();

                return new DTO<Moneda>
                {
                    Correcto = true,
                    Datos = monedaActualizada,
                    Mensaje = "Moneda actualizada correctamente."
                };
            }
            catch (Exception ex)
            {
                transaccion.Rollback();
                return new DTO<Moneda>
                {
                    Correcto = false,
                    Mensaje = $"Error al actualizar moneda: {ex.Message}"
                };
            }
        }

        public async Task<DTO<bool>> Eliminar(Moneda moneda)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            using var transaccion = conexion.BeginTransaction();

            try
            {
                var sqlEliminarMoneda = @"DELETE FROM Moneda WHERE MonedaID = @MonedaID;";
                var filasAfectadas = await conexion.ExecuteAsync(sqlEliminarMoneda, new { MonedaID = moneda.Id }, transaccion);

                transaccion.Commit();

                return new DTO<bool>
                {
                    Correcto = filasAfectadas > 0,
                    Datos = filasAfectadas > 0,
                    Mensaje = filasAfectadas > 0 ? "Moneda eliminada correctamente." : "No se encontró la moneda."
                };
            }
            catch (Exception ex)
            {
                transaccion.Rollback();
                return new DTO<bool>
                {
                    Correcto = false,
                    Mensaje = $"Error al eliminar moneda: {ex.Message}"
                };
            }
        }

        public async Task<DTO<Moneda>> Obtener_por_id(Moneda moneda)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();

            var sqlMoneda = @"SELECT MonedaID as Id, Codigo, Nombre FROM Moneda WHERE MonedaID = @MonedaID;";

            var monedaEncontrada = await conexion.QuerySingleOrDefaultAsync<Moneda>(sqlMoneda, new { MonedaID = moneda.Id });
            if (monedaEncontrada == null)
                return new DTO<Moneda> { Correcto = false, Mensaje = "Moneda no encontrada" };

            return new DTO<Moneda>
            {
                Correcto = true,
                Datos = monedaEncontrada,
                Mensaje = "Moneda obtenida correctamente"
            };
        }

        public async Task<DTO<IEnumerable<Moneda>>> Obtener_todos()
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();

            var sqlMonedas = @"SELECT MonedaID as Id, Codigo, Nombre FROM Moneda ORDER BY Nombre;";

            var monedas = (await conexion.QueryAsync<Moneda>(sqlMonedas)).ToList();

            return new DTO<IEnumerable<Moneda>>
            {
                Correcto = true,
                Datos = monedas,
                Mensaje = "Monedas obtenidas correctamente"
            };
        }

        public async Task<DTO<Items_pagina<Moneda>>> Obtener_paginado(Filtros_paginado filtros)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();

            var sql = @"SELECT 
                            COUNT(*) OVER() AS TotalItems,
                            MonedaID as Id,
                            Codigo,
                            Nombre
                        FROM Moneda
                        WHERE 
                            (@FiltroBusqueda IS NULL OR Codigo LIKE '%' + @FiltroBusqueda + '%' OR Nombre LIKE '%' + @FiltroBusqueda + '%')
                        ORDER BY
                            CASE WHEN @OrdenarPor = 'Codigo' THEN Codigo END,
                            CASE WHEN @OrdenarPor = 'Nombre' THEN Nombre END
                        OFFSET @Skip ROWS FETCH NEXT @Top ROWS ONLY;";

            var lista = await conexion.QueryAsync<dynamic>(sql, new
            {
                FiltroBusqueda = (object?)filtros.Filter ?? DBNull.Value,
                OrdenarPor = (object?)filtros.OrderBy ?? "Nombre",
                Skip = filtros.Skip < 0 ? 0 : filtros.Skip,
                Top = filtros.Top <= 0 ? 10 : filtros.Top
            });

            var monedas = new List<Moneda>();
            int totalItems = 0;

            foreach (var row in lista)
            {
                totalItems = row.TotalItems;

                monedas.Add(new Moneda
                {
                    Id = row.Id,
                    Codigo = row.Codigo,
                    Nombre = row.Nombre
                });
            }

            return new DTO<Items_pagina<Moneda>>
            {
                Correcto = true,
                Datos = new Items_pagina<Moneda>
                {
                    Total_items = totalItems,
                    Items = monedas
                },
                Mensaje = "Monedas paginadas correctamente."
            };
        }
    }
}