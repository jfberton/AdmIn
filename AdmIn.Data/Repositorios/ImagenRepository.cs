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
    public class ImagenRepository : IImagenRepository
    {
        public ImagenRepository()
        {
        }

        public async Task<DTO<Imagen>> Crear(Imagen imagen)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            using var transaccion = conexion.BeginTransaction();

            try
            {
                var sql = @"INSERT INTO Imagen (Nombre, Descripcion, Url, UrlThumb)
                           OUTPUT INSERTED.Id,
                                  INSERTED.Nombre,
                                  INSERTED.Descripcion,
                                  INSERTED.FechaCreacion,
                                  INSERTED.Url,
                                  INSERTED.UrlThumb
                           VALUES (@Nombre, @Descripcion, @Url, @UrlThumb);";

                var imagenCreada = await conexion.QuerySingleOrDefaultAsync<Imagen>(sql, new
                {
                    imagen.Nombre,
                    imagen.Descripcion,
                    imagen.Url,
                    imagen.UrlThumb
                }, transaccion);

                if (imagenCreada == null)
                    throw new Exception("No se pudo crear la imagen.");

                transaccion.Commit();

                return new DTO<Imagen>
                {
                    Correcto = true,
                    Datos = imagenCreada,
                    Mensaje = "Imagen creada correctamente."
                };
            }
            catch (Exception ex)
            {
                transaccion.Rollback();
                return new DTO<Imagen>
                {
                    Correcto = false,
                    Mensaje = $"Error al crear imagen: {ex.Message}"
                };
            }
        }

        public async Task<DTO<Imagen>> Actualizar(Imagen imagen)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            using var transaccion = conexion.BeginTransaction();

            try
            {
                var sql = @"UPDATE Imagen
                           SET Nombre = @Nombre,
                               Descripcion = @Descripcion,
                               Url = @Url,
                               UrlThumb = @UrlThumb
                           OUTPUT INSERTED.Id,
                                  INSERTED.Nombre,
                                  INSERTED.Descripcion,
                                  INSERTED.FechaCreacion,
                                  INSERTED.Url,
                                  INSERTED.UrlThumb
                           WHERE Id = @Id;";

                var imagenActualizada = await conexion.QuerySingleOrDefaultAsync<Imagen>(sql, new
                {
                    imagen.Id,
                    imagen.Nombre,
                    imagen.Descripcion,
                    imagen.Url,
                    imagen.UrlThumb
                }, transaccion);

                if (imagenActualizada == null)
                    throw new Exception("No se pudo actualizar la imagen.");

                transaccion.Commit();

                return new DTO<Imagen>
                {
                    Correcto = true,
                    Datos = imagenActualizada,
                    Mensaje = "Imagen actualizada correctamente."
                };
            }
            catch (Exception ex)
            {
                transaccion.Rollback();
                return new DTO<Imagen>
                {
                    Correcto = false,
                    Mensaje = $"Error al actualizar imagen: {ex.Message}"
                };
            }
        }

        public async Task<DTO<bool>> Eliminar(Imagen imagen)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            using var transaccion = conexion.BeginTransaction();

            try
            {
                // Verificar si está siendo usada como imagen principal
                var sqlVerificar = @"SELECT COUNT(*) FROM Inmueble WHERE ImagenPrincipalId = @Id;";
                var enUso = await conexion.QuerySingleAsync<int>(sqlVerificar, new { Id = imagen.Id }, transaccion);

                if (enUso > 0)
                {
                    return new DTO<bool>
                    {
                        Correcto = false,
                        Datos = false,
                        Mensaje = "No se puede eliminar la imagen porque está siendo utilizada como imagen principal de uno o más inmuebles."
                    };
                }

                var sql = @"DELETE FROM Imagen WHERE Id = @Id;";
                var filasAfectadas = await conexion.ExecuteAsync(sql, new { Id = imagen.Id }, transaccion);

                transaccion.Commit();

                return new DTO<bool>
                {
                    Correcto = filasAfectadas > 0,
                    Datos = filasAfectadas > 0,
                    Mensaje = filasAfectadas > 0 ? "Imagen eliminada correctamente." : "No se encontró la imagen."
                };
            }
            catch (Exception ex)
            {
                transaccion.Rollback();
                return new DTO<bool>
                {
                    Correcto = false,
                    Mensaje = $"Error al eliminar imagen: {ex.Message}"
                };
            }
        }

        public async Task<DTO<Imagen>> Obtener_por_id(Imagen imagen)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();

            var sql = @"SELECT Id, Nombre, Descripcion, FechaCreacion, Url, UrlThumb 
                        FROM Imagen 
                        WHERE Id = @Id;";

            var imagenEncontrada = await conexion.QuerySingleOrDefaultAsync<Imagen>(sql, new { Id = imagen.Id });

            if (imagenEncontrada == null)
                return new DTO<Imagen> { Correcto = false, Mensaje = "Imagen no encontrada" };

            return new DTO<Imagen>
            {
                Correcto = true,
                Datos = imagenEncontrada,
                Mensaje = "Imagen obtenida correctamente"
            };
        }

        public async Task<DTO<IEnumerable<Imagen>>> Obtener_todos()
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();

            var sql = @"SELECT Id, Nombre, Descripcion, FechaCreacion, Url, UrlThumb 
                        FROM Imagen 
                        ORDER BY FechaCreacion DESC;";

            var imagenes = await conexion.QueryAsync<Imagen>(sql);

            return new DTO<IEnumerable<Imagen>>
            {
                Correcto = true,
                Datos = imagenes,
                Mensaje = "Imágenes obtenidas correctamente"
            };
        }

        public async Task<DTO<Items_pagina<Imagen>>> Obtener_paginado(Filtros_paginado filtros)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();

            var sql = @"SELECT 
                            COUNT(*) OVER() AS TotalItems,
                            Id,
                            Nombre,
                            Descripcion,
                            FechaCreacion,
                            Url,
                            UrlThumb
                        FROM Imagen
                        WHERE 
                            (@FiltroBusqueda IS NULL OR Nombre LIKE '%' + @FiltroBusqueda + '%' 
                             OR Descripcion LIKE '%' + @FiltroBusqueda + '%')
                        ORDER BY
                            CASE WHEN @OrdenarPor = 'Nombre' THEN Nombre END,
                            CASE WHEN @OrdenarPor = 'FechaCreacion' THEN FechaCreacion END
                        OFFSET @Skip ROWS FETCH NEXT @Top ROWS ONLY;";

            var lista = await conexion.QueryAsync<dynamic>(sql, new
            {
                FiltroBusqueda = (object?)filtros.Filter ?? DBNull.Value,
                OrdenarPor = (object?)filtros.OrderBy ?? "FechaCreacion",
                Skip = filtros.Skip < 0 ? 0 : filtros.Skip,
                Top = filtros.Top <= 0 ? 10 : filtros.Top
            });

            var imagenes = new List<Imagen>();
            int totalItems = 0;

            foreach (var row in lista)
            {
                totalItems = row.TotalItems;

                var imagen = new Imagen
                {
                    Id = row.Id,
                    Nombre = row.Nombre,
                    Descripcion = row.Descripcion,
                    FechaCreacion = row.FechaCreacion,
                    Url = row.Url,
                    UrlThumb = row.UrlThumb
                };

                imagenes.Add(imagen);
            }

            return new DTO<Items_pagina<Imagen>>
            {
                Correcto = true,
                Datos = new Items_pagina<Imagen>
                {
                    Total_items = totalItems,
                    Items = imagenes
                },
                Mensaje = "Imágenes paginadas correctamente."
            };
        }

        public async Task<DTO<IEnumerable<Imagen>>> Obtener_por_inmueble(int inmuebleId)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();

            // Nota: Este método asume que existe una tabla de relación InmuebleImagen
            // Si no existe, se puede implementar una lógica diferente
            var sql = @"SELECT DISTINCT img.Id, img.Nombre, img.Descripcion, img.FechaCreacion, img.Url, img.UrlThumb 
                        FROM Imagen img
                        WHERE img.Id IN (
                            SELECT ImagenPrincipalId FROM Inmueble WHERE InmuebleID = @InmuebleId AND ImagenPrincipalId IS NOT NULL
                        )
                        ORDER BY img.FechaCreacion DESC;";

            var imagenes = await conexion.QueryAsync<Imagen>(sql, new { InmuebleId = inmuebleId });

            return new DTO<IEnumerable<Imagen>>
            {
                Correcto = true,
                Datos = imagenes,
                Mensaje = "Imágenes del inmueble obtenidas correctamente"
            };
        }

        public async Task<DTO<bool>> Asociar_a_inmueble(Guid imagenId, int inmuebleId)
        {
            // Método de ejemplo - la implementación depende de la estructura de la base de datos
            // Si existe una tabla de relación InmuebleImagen, se insertaría aquí
            return new DTO<bool>
            {
                Correcto = true,
                Datos = true,
                Mensaje = "Funcionalidad pendiente de implementar según estructura de BD"
            };
        }

        public async Task<DTO<bool>> Desasociar_de_inmueble(Guid imagenId, int inmuebleId)
        {
            // Método de ejemplo - la implementación depende de la estructura de la base de datos
            return new DTO<bool>
            {
                Correcto = true,
                Datos = true,
                Mensaje = "Funcionalidad pendiente de implementar según estructura de BD"
            };
        }

        public async Task<DTO<bool>> Establecer_como_principal(Guid imagenId, int inmuebleId)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            using var transaccion = conexion.BeginTransaction();

            try
            {
                // Verificar que la imagen existe
                var sqlVerificarImagen = @"SELECT COUNT(*) FROM Imagen WHERE Id = @ImagenId;";
                var existeImagen = await conexion.QuerySingleAsync<int>(sqlVerificarImagen, new { ImagenId = imagenId }, transaccion);

                if (existeImagen == 0)
                {
                    return new DTO<bool>
                    {
                        Correcto = false,
                        Datos = false,
                        Mensaje = "La imagen especificada no existe."
                    };
                }

                // Actualizar el inmueble para establecer la imagen principal
                var sql = @"UPDATE Inmueble 
                           SET ImagenPrincipalId = @ImagenId 
                           WHERE InmuebleID = @InmuebleId;";

                var filasAfectadas = await conexion.ExecuteAsync(sql, new { ImagenId = imagenId, InmuebleId = inmuebleId }, transaccion);

                transaccion.Commit();

                return new DTO<bool>
                {
                    Correcto = filasAfectadas > 0,
                    Datos = filasAfectadas > 0,
                    Mensaje = filasAfectadas > 0 ? "Imagen establecida como principal correctamente." : "No se encontró el inmueble."
                };
            }
            catch (Exception ex)
            {
                transaccion.Rollback();
                return new DTO<bool>
                {
                    Correcto = false,
                    Mensaje = $"Error al establecer imagen principal: {ex.Message}"
                };
            }
        }

        public async Task<DTO<bool>> Establecer_imagen_perfil_usuario(Guid imagenId, int usuarioId)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            using var transaccion = conexion.BeginTransaction();

            try
            {
                // Verificar que la imagen existe
                var sqlVerificarImagen = @"SELECT COUNT(*) FROM Imagen WHERE Id = @ImagenId;";
                var existeImagen = await conexion.QuerySingleAsync<int>(sqlVerificarImagen, new { ImagenId = imagenId }, transaccion);

                if (existeImagen == 0)
                {
                    return new DTO<bool>
                    {
                        Correcto = false,
                        Datos = false,
                        Mensaje = "La imagen especificada no existe."
                    };
                }

                // Actualizar el usuario para establecer la imagen de perfil
                var sql = @"UPDATE Usuario 
                           SET ImagenPerfilId = @ImagenId 
                           WHERE UsuarioID = @UsuarioId;";

                var filasAfectadas = await conexion.ExecuteAsync(sql, new { ImagenId = imagenId, UsuarioId = usuarioId }, transaccion);

                transaccion.Commit();

                return new DTO<bool>
                {
                    Correcto = filasAfectadas > 0,
                    Datos = filasAfectadas > 0,
                    Mensaje = filasAfectadas > 0 ? "Imagen de perfil establecida correctamente." : "No se encontró el usuario."
                };
            }
            catch (Exception ex)
            {
                transaccion.Rollback();
                return new DTO<bool>
                {
                    Correcto = false,
                    Mensaje = $"Error al establecer imagen de perfil: {ex.Message}"
                };
            }
        }
    }
}