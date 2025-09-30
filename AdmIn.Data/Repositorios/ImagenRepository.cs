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
                // 1. Verificar si está siendo usada como imagen principal
                var sqlVerificarPrincipal = @"SELECT COUNT(*) FROM Inmueble WHERE ImagenPrincipalId = @Id;";
                var enUsoPrincipal = await conexion.QuerySingleAsync<int>(sqlVerificarPrincipal, new { Id = imagen.Id }, transaccion);

                if (enUsoPrincipal > 0)
                {
                    return new DTO<bool>
                    {
                        Correcto = false,
                        Datos = false,
                        Mensaje = "No se puede eliminar la imagen porque está siendo utilizada como imagen principal de uno o más inmuebles. Primero cambie la imagen principal."
                    };
                }

                // 2. Verificar y eliminar referencias en tabla InmuebleImagen (si existe)
                try
                {
                    var sqlVerificarTablaRelacion = @"
                        IF EXISTS (SELECT * FROM sysobjects WHERE name='InmuebleImagen' AND xtype='U')
                        BEGIN
                            SELECT COUNT(*) FROM InmuebleImagen WHERE ImagenId = @Id;
                        END
                        ELSE
                        BEGIN
                            SELECT 0;
                        END";
                    
                    var enUsoRelacion = await conexion.QuerySingleAsync<int>(sqlVerificarTablaRelacion, new { Id = imagen.Id }, transaccion);
                    
                    if (enUsoRelacion > 0)
                    {
                        // Eliminar las referencias en la tabla de relación
                        var sqlEliminarRelaciones = @"DELETE FROM InmuebleImagen WHERE ImagenId = @Id;";
                        await conexion.ExecuteAsync(sqlEliminarRelaciones, new { Id = imagen.Id }, transaccion);
                    }
                }
                catch (Exception)
                {
                    // Si la tabla InmuebleImagen no existe, continuar
                }

                // 3. Verificar si está siendo usada como imagen de perfil de usuario
                var sqlVerificarPerfil = @"SELECT COUNT(*) FROM Usuario WHERE ImagenPerfilId = @Id;";
                var enUsoPerfil = await conexion.QuerySingleAsync<int>(sqlVerificarPerfil, new { Id = imagen.Id }, transaccion);

                if (enUsoPerfil > 0)
                {
                    // Opcionalmente, podríamos limpiar las referencias de perfil
                    // var sqlLimpiarPerfil = @"UPDATE Usuario SET ImagenPerfilId = NULL WHERE ImagenPerfilId = @Id;";
                    // await conexion.ExecuteAsync(sqlLimpiarPerfil, new { Id = imagen.Id }, transaccion);
                    
                    return new DTO<bool>
                    {
                        Correcto = false,
                        Datos = false,
                        Mensaje = "No se puede eliminar la imagen porque está siendo utilizada como imagen de perfil de uno o más usuarios."
                    };
                }

                // 4. Eliminar la imagen de la tabla principal
                var sqlEliminarImagen = @"DELETE FROM Imagen WHERE Id = @Id;";
                var filasAfectadas = await conexion.ExecuteAsync(sqlEliminarImagen, new { Id = imagen.Id }, transaccion);

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

            try
            {
                // Verificar si existe la tabla de relación InmuebleImagen
                var sqlVerificarTabla = @"SELECT COUNT(*) FROM sysobjects WHERE name='InmuebleImagen' AND xtype='U'";
                var existeTabla = await conexion.QuerySingleAsync<int>(sqlVerificarTabla);

                var imagenes = new List<Imagen>();

                if (existeTabla > 0)
                {
                    // Obtener todas las imágenes asociadas a través de InmuebleImagen
                    var sqlImgs = @"
                        SELECT i.Id, i.Nombre, i.Descripcion, i.FechaCreacion, i.Url, i.UrlThumb, ii.Orden
                        FROM InmuebleImagen ii
                        INNER JOIN Imagen i ON ii.ImagenId = i.Id
                        WHERE ii.InmuebleId = @InmuebleId
                        ORDER BY ii.Orden, i.FechaCreacion DESC;";

                    var imgsFromRel = (await conexion.QueryAsync<dynamic>(sqlImgs, new { InmuebleId = inmuebleId })).ToList();

                    foreach (var row in imgsFromRel)
                    {
                        imagenes.Add(new Imagen
                        {
                            Id = row.Id,
                            Nombre = row.Nombre,
                            Descripcion = row.Descripcion,
                            FechaCreacion = row.FechaCreacion,
                            Url = row.Url,
                            UrlThumb = row.UrlThumb
                        });
                    }

                    // Asegurar que la imagen principal esté incluida y al inicio
                    var principalId = await conexion.QuerySingleOrDefaultAsync<Guid?>("SELECT ImagenPrincipalId FROM Inmueble WHERE InmuebleID = @InmuebleId", new { InmuebleId = inmuebleId });
                    if (principalId.HasValue)
                    {
                        if (!imagenes.Any(i => i.Id == principalId.Value))
                        {
                            var sqlPrincipal = @"SELECT Id, Nombre, Descripcion, FechaCreacion, Url, UrlThumb FROM Imagen WHERE Id = @Id";
                            var principalImg = await conexion.QuerySingleOrDefaultAsync<Imagen>(sqlPrincipal, new { Id = principalId.Value });
                            if (principalImg != null)
                            {
                                imagenes.Insert(0, principalImg);
                            }
                        }
                        else
                        {
                            // Reordenar para poner principal primero
                            imagenes = imagenes.OrderBy(i => i.Id == principalId.Value ? 0 : 1).ThenBy(i => i.FechaCreacion).ToList();
                        }
                    }

                    return new DTO<IEnumerable<Imagen>>
                    {
                        Correcto = true,
                        Datos = imagenes,
                        Mensaje = "Imágenes del inmueble obtenidas correctamente"
                    };
                }
                else
                {
                    // Si no existe la tabla de relación, devolver solo la imagen principal (si existe)
                    var sqlFallback = @"
                        SELECT img.Id, img.Nombre, img.Descripcion, img.FechaCreacion, img.Url, img.UrlThumb 
                        FROM Imagen img
                        INNER JOIN Inmueble i ON img.Id = i.ImagenPrincipalId
                        WHERE i.InmuebleID = @InmuebleId;";

                    var imagenesFallback = await conexion.QueryAsync<Imagen>(sqlFallback, new { InmuebleId = inmuebleId });
                    return new DTO<IEnumerable<Imagen>>
                    {
                        Correcto = true,
                        Datos = imagenesFallback,
                        Mensaje = "Imagen principal del inmueble obtenida correctamente"
                    };
                }
            }
            catch (Exception ex)
            {
                // En caso de error, devolver lista vacía pero indicar mensaje para facilitar diagnóstico
                return new DTO<IEnumerable<Imagen>>
                {
                    Correcto = false,
                    Datos = new List<Imagen>(),
                    Mensaje = $"Error al obtener imágenes del inmueble: {ex.Message}"
                };
            }
        }

        public async Task<DTO<bool>> Asociar_a_inmueble(Guid imagenId, int inmuebleId)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            using var transaccion = conexion.BeginTransaction();

            try
            {
                // Verificar si la tabla InmuebleImagen existe, si no, crearla
                var sqlVerificarTabla = @"
                    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='InmuebleImagen' AND xtype='U')
                    CREATE TABLE InmuebleImagen (
                        Id INT IDENTITY(1,1) PRIMARY KEY,
                        InmuebleId INT NOT NULL,
                        ImagenId UNIQUEIDENTIFIER NOT NULL,
                        FechaAsociacion DATETIME DEFAULT GETDATE(),
                        Orden INT DEFAULT 0,
                        FOREIGN KEY (InmuebleId) REFERENCES Inmueble(InmuebleID),
                        FOREIGN KEY (ImagenId) REFERENCES Imagen(Id),
                        UNIQUE(InmuebleId, ImagenId)
                    );";

                await conexion.ExecuteAsync(sqlVerificarTabla, transaction: transaccion);

                // Insertar la asociación
                var sqlInsertar = @"
                    IF NOT EXISTS (SELECT 1 FROM InmuebleImagen WHERE InmuebleId = @InmuebleId AND ImagenId = @ImagenId)
                    INSERT INTO InmuebleImagen (InmuebleId, ImagenId, Orden)
                    VALUES (@InmuebleId, @ImagenId, 
                        ISNULL((SELECT MAX(Orden) FROM InmuebleImagen WHERE InmuebleId = @InmuebleId), 0) + 1);";

                var filasAfectadas = await conexion.ExecuteAsync(sqlInsertar, new
                {
                    InmuebleId = inmuebleId,
                    ImagenId = imagenId
                }, transaccion);

                transaccion.Commit();

                return new DTO<bool>
                {
                    Correcto = true,
                    Datos = true,
                    Mensaje = "Imagen asociada al inmueble correctamente"
                };
            }
            catch (Exception ex)
            {
                transaccion.Rollback();
                return new DTO<bool>
                {
                    Correcto = false,
                    Mensaje = $"Error al asociar imagen al inmueble: {ex.Message}"
                };
            }
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

                // Verificar que el inmueble existe
                var sqlVerificarInmueble = @"SELECT COUNT(*) FROM Inmueble WHERE InmuebleID = @InmuebleId;";
                var existeInmueble = await conexion.QuerySingleAsync<int>(sqlVerificarInmueble, new { InmuebleId = inmuebleId }, transaccion);

                if (existeInmueble == 0)
                {
                    return new DTO<bool>
                    {
                        Correcto = false,
                        Datos = false,
                        Mensaje = "El inmueble especificado no existe."
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
                    Mensaje = filasAfectadas > 0 ? "Imagen establecida como principal correctamente." : "No se pudo actualizar el inmueble."
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

                // Verificar que el usuario existe
                var sqlVerificarUsuario = @"SELECT COUNT(*) FROM Usuario WHERE UsuarioID = @UsuarioId;";
                var existeUsuario = await conexion.QuerySingleAsync<int>(sqlVerificarUsuario, new { UsuarioId = usuarioId }, transaccion);

                if (existeUsuario == 0)
                {
                    return new DTO<bool>
                    {
                        Correcto = false,
                        Datos = false,
                        Mensaje = "El usuario especificado no existe."
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
                    Mensaje = filasAfectadas > 0 ? "Imagen de perfil establecida correctamente." : "No se pudo actualizar el usuario."
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