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
    public class CaracteristicaRepository : ICaracteristicaRepository
    {
        public CaracteristicaRepository()
        {
        }

        public async Task<DTO<Caracteristica>> Crear(Caracteristica caracteristica)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            using var transaccion = conexion.BeginTransaction();

            try
            {
                var sql = @"INSERT INTO Caracteristica (Nombre, Tipo, Descripcion)
                           OUTPUT INSERTED.CaracteristicaID as Id,
                                  INSERTED.Nombre,
                                  INSERTED.Tipo,
                                  INSERTED.Descripcion
                           VALUES (@Nombre, @Tipo, @Descripcion);";

                var caracteristicaCreada = await conexion.QuerySingleOrDefaultAsync<Caracteristica>(sql, new
                {
                    caracteristica.Nombre,
                    caracteristica.Tipo,
                    caracteristica.Descripcion
                }, transaccion);

                if (caracteristicaCreada == null)
                    throw new Exception("No se pudo crear la característica.");

                transaccion.Commit();

                return new DTO<Caracteristica>
                {
                    Correcto = true,
                    Datos = caracteristicaCreada,
                    Mensaje = "Característica creada correctamente."
                };
            }
            catch (Exception ex)
            {
                transaccion.Rollback();
                return new DTO<Caracteristica>
                {
                    Correcto = false,
                    Mensaje = $"Error al crear característica: {ex.Message}"
                };
            }
        }

        public async Task<DTO<Caracteristica>> Actualizar(Caracteristica caracteristica)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            using var transaccion = conexion.BeginTransaction();

            try
            {
                var sql = @"UPDATE Caracteristica
                           SET Nombre = @Nombre,
                               Tipo = @Tipo,
                               Descripcion = @Descripcion
                           OUTPUT INSERTED.CaracteristicaID as Id,
                                  INSERTED.Nombre,
                                  INSERTED.Tipo,
                                  INSERTED.Descripcion
                           WHERE CaracteristicaID = @Id;";

                var caracteristicaActualizada = await conexion.QuerySingleOrDefaultAsync<Caracteristica>(sql, new
                {
                    caracteristica.Id,
                    caracteristica.Nombre,
                    caracteristica.Tipo,
                    caracteristica.Descripcion
                }, transaccion);

                if (caracteristicaActualizada == null)
                    throw new Exception("No se pudo actualizar la característica.");

                transaccion.Commit();

                return new DTO<Caracteristica>
                {
                    Correcto = true,
                    Datos = caracteristicaActualizada,
                    Mensaje = "Característica actualizada correctamente."
                };
            }
            catch (Exception ex)
            {
                transaccion.Rollback();
                return new DTO<Caracteristica>
                {
                    Correcto = false,
                    Mensaje = $"Error al actualizar característica: {ex.Message}"
                };
            }
        }

        public async Task<DTO<bool>> Eliminar(Caracteristica caracteristica)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            using var transaccion = conexion.BeginTransaction();

            try
            {
                // Verificar si está siendo usada en inmuebles
                var sqlVerificar = @"SELECT COUNT(*) FROM CaracteristicaInmueble WHERE CaracteristicaID = @Id;";
                var enUso = await conexion.QuerySingleAsync<int>(sqlVerificar, new { Id = caracteristica.Id }, transaccion);

                if (enUso > 0)
                {
                    return new DTO<bool>
                    {
                        Correcto = false,
                        Datos = false,
                        Mensaje = "No se puede eliminar la característica porque está siendo utilizada en inmuebles."
                    };
                }

                var sql = @"DELETE FROM Caracteristica WHERE CaracteristicaID = @Id;";
                var filasAfectadas = await conexion.ExecuteAsync(sql, new { Id = caracteristica.Id }, transaccion);

                transaccion.Commit();

                return new DTO<bool>
                {
                    Correcto = filasAfectadas > 0,
                    Datos = filasAfectadas > 0,
                    Mensaje = filasAfectadas > 0 ? "Característica eliminada correctamente." : "No se encontró la característica."
                };
            }
            catch (Exception ex)
            {
                transaccion.Rollback();
                return new DTO<bool>
                {
                    Correcto = false,
                    Mensaje = $"Error al eliminar característica: {ex.Message}"
                };
            }
        }

        public async Task<DTO<Caracteristica>> Obtener_por_id(Caracteristica caracteristica)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();

            var sql = @"SELECT CaracteristicaID as Id, Nombre, Tipo, Descripcion 
                        FROM Caracteristica 
                        WHERE CaracteristicaID = @Id;";

            var caracteristicaEncontrada = await conexion.QuerySingleOrDefaultAsync<Caracteristica>(sql, new { Id = caracteristica.Id });

            if (caracteristicaEncontrada == null)
                return new DTO<Caracteristica> { Correcto = false, Mensaje = "Característica no encontrada" };

            return new DTO<Caracteristica>
            {
                Correcto = true,
                Datos = caracteristicaEncontrada,
                Mensaje = "Característica obtenida correctamente"
            };
        }

        public async Task<DTO<IEnumerable<Caracteristica>>> Obtener_todos()
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();

            var sql = @"SELECT CaracteristicaID as Id, Nombre, Tipo, Descripcion 
                        FROM Caracteristica 
                        ORDER BY Tipo, Nombre;";

            var caracteristicas = await conexion.QueryAsync<Caracteristica>(sql);

            return new DTO<IEnumerable<Caracteristica>>
            {
                Correcto = true,
                Datos = caracteristicas,
                Mensaje = "Características obtenidas correctamente"
            };
        }

        public async Task<DTO<Items_pagina<Caracteristica>>> Obtener_paginado(Filtros_paginado filtros)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();

            var sql = @"SELECT 
                            COUNT(*) OVER() AS TotalItems,
                            CaracteristicaID as Id,
                            Nombre,
                            Tipo,
                            Descripcion
                        FROM Caracteristica
                        WHERE 
                            (@FiltroBusqueda IS NULL OR Nombre LIKE '%' + @FiltroBusqueda + '%' 
                             OR Tipo LIKE '%' + @FiltroBusqueda + '%' 
                             OR Descripcion LIKE '%' + @FiltroBusqueda + '%')
                        ORDER BY
                            CASE WHEN @OrdenarPor = 'Nombre' THEN Nombre END,
                            CASE WHEN @OrdenarPor = 'Tipo' THEN Tipo END
                        OFFSET @Skip ROWS FETCH NEXT @Top ROWS ONLY;";

            var lista = await conexion.QueryAsync<dynamic>(sql, new
            {
                FiltroBusqueda = (object?)filtros.Filter ?? DBNull.Value,
                OrdenarPor = (object?)filtros.OrderBy ?? "Nombre",
                Skip = filtros.Skip < 0 ? 0 : filtros.Skip,
                Top = filtros.Top <= 0 ? 10 : filtros.Top
            });

            var caracteristicas = new List<Caracteristica>();
            int totalItems = 0;

            foreach (var row in lista)
            {
                totalItems = row.TotalItems;

                var caracteristica = new Caracteristica
                {
                    Id = row.Id,
                    Nombre = row.Nombre,
                    Tipo = row.Tipo,
                    Descripcion = row.Descripcion
                };

                caracteristicas.Add(caracteristica);
            }

            return new DTO<Items_pagina<Caracteristica>>
            {
                Correcto = true,
                Datos = new Items_pagina<Caracteristica>
                {
                    Total_items = totalItems,
                    Items = caracteristicas
                },
                Mensaje = "Características paginadas correctamente."
            };
        }

        public async Task<DTO<IEnumerable<Caracteristica>>> Obtener_por_tipo(string tipo)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();

            var sql = @"SELECT CaracteristicaID as Id, Nombre, Tipo, Descripcion 
                        FROM Caracteristica 
                        WHERE Tipo = @Tipo
                        ORDER BY Nombre;";

            var caracteristicas = await conexion.QueryAsync<Caracteristica>(sql, new { Tipo = tipo });

            return new DTO<IEnumerable<Caracteristica>>
            {
                Correcto = true,
                Datos = caracteristicas,
                Mensaje = "Características obtenidas correctamente"
            };
        }

        public async Task<DTO<IEnumerable<string>>> Obtener_tipos()
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();

            var sql = @"SELECT DISTINCT Tipo FROM Caracteristica WHERE Tipo IS NOT NULL AND Tipo != '' ORDER BY Tipo;";
            var tipos = await conexion.QueryAsync<string>(sql);

            return new DTO<IEnumerable<string>>
            {
                Correcto = true,
                Datos = tipos,
                Mensaje = "Tipos obtenidos correctamente"
            };
        }
    }
}