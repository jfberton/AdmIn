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
    public class InmuebleRepository : IInmuebleRepository
    {
        public InmuebleRepository()
        {
        }

        public async Task<DTO<Inmueble>> Crear(Inmueble inmueble)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            using var transaccion = conexion.BeginTransaction();

            try
            {
                var sqlInmueble = @"INSERT INTO Inmueble 
                    (Nombre, Direccion, Pais, Estado, Ciudad, CP, Latitud, Longitud, 
                     Valor, ConstruccionM2, RentaMensual, AdministradorId, Descripcion, 
                     ImagenPrincipalId, MonedaId, Activo, FechaCreacion, FechaModificacion, 
                     UsuarioCreadorId, UsuarioModificadorId)
                    OUTPUT INSERTED.InmuebleID as Id,
                           INSERTED.Nombre,
                           INSERTED.Direccion,
                           INSERTED.Pais,
                           INSERTED.Estado,
                           INSERTED.Ciudad,
                           INSERTED.CP as CodigoPostal,
                           INSERTED.Latitud,
                           INSERTED.Longitud,
                           INSERTED.Valor,
                           INSERTED.ConstruccionM2,
                           INSERTED.RentaMensual,
                           INSERTED.AdministradorId,
                           INSERTED.Descripcion,
                           INSERTED.ImagenPrincipalId,
                           INSERTED.MonedaId,
                           INSERTED.Activo,
                           INSERTED.FechaCreacion,
                           INSERTED.FechaModificacion,
                           INSERTED.UsuarioCreadorId,
                           INSERTED.UsuarioModificadorId
                    VALUES (@Nombre, @Direccion, @Pais, @Estado, @Ciudad, @CodigoPostal, 
                           @Latitud, @Longitud, @Valor, @ConstruccionM2, @RentaMensual, 
                           @AdministradorId, @Descripcion, @ImagenPrincipalId, @MonedaId, 
                           @Activo, GETDATE(), GETDATE(), @UsuarioCreadorId, @UsuarioModificadorId);";

                var inmuebleCreado = await conexion.QuerySingleOrDefaultAsync<Inmueble>(sqlInmueble, new
                {
                    inmueble.Nombre,
                    inmueble.Direccion,
                    inmueble.Pais,
                    inmueble.Estado,
                    inmueble.Ciudad,
                    CodigoPostal = inmueble.CodigoPostal,
                    inmueble.Latitud,
                    inmueble.Longitud,
                    inmueble.Valor,
                    ConstruccionM2 = inmueble.ConstruccionM2,
                    inmueble.RentaMensual,
                    AdministradorId = (object?)inmueble.AdministradorId ?? DBNull.Value,
                    inmueble.Descripcion,
                    ImagenPrincipalId = (object?)inmueble.ImagenPrincipalId ?? DBNull.Value,
                    inmueble.MonedaId,
                    inmueble.Activo,
                    UsuarioCreadorId = (object?)inmueble.UsuarioCreadorId ?? DBNull.Value,
                    UsuarioModificadorId = (object?)inmueble.UsuarioModificadorId ?? DBNull.Value
                }, transaccion);

                if (inmuebleCreado == null)
                    throw new Exception("No se pudo crear el inmueble.");

                transaccion.Commit();

                return new DTO<Inmueble>
                {
                    Correcto = true,
                    Datos = inmuebleCreado,
                    Mensaje = "Inmueble creado correctamente."
                };
            }
            catch (Exception ex)
            {
                transaccion.Rollback();
                return new DTO<Inmueble>
                {
                    Correcto = false,
                    Mensaje = $"Error al crear inmueble: {ex.Message}"
                };
            }
        }

        public async Task<DTO<Inmueble>> Actualizar(Inmueble inmueble)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            using var transaccion = conexion.BeginTransaction();

            try
            {
                var sqlActualizarInmueble = @"UPDATE Inmueble
                    SET Nombre = @Nombre,
                        Direccion = @Direccion,
                        Pais = @Pais,
                        Estado = @Estado,
                        Ciudad = @Ciudad,
                        CP = @CodigoPostal,
                        Latitud = @Latitud,
                        Longitud = @Longitud,
                        Valor = @Valor,
                        ConstruccionM2 = @ConstruccionM2,
                        RentaMensual = @RentaMensual,
                        AdministradorId = @AdministradorId,
                        Descripcion = @Descripcion,
                        ImagenPrincipalId = @ImagenPrincipalId,
                        MonedaId = @MonedaId,
                        Activo = @Activo,
                        FechaModificacion = GETDATE(),
                        UsuarioModificadorId = @UsuarioModificadorId
                    OUTPUT INSERTED.InmuebleID as Id,
                           INSERTED.Nombre,
                           INSERTED.Direccion,
                           INSERTED.Pais,
                           INSERTED.Estado,
                           INSERTED.Ciudad,
                           INSERTED.CP as CodigoPostal,
                           INSERTED.Latitud,
                           INSERTED.Longitud,
                           INSERTED.Valor,
                           INSERTED.ConstruccionM2,
                           INSERTED.RentaMensual,
                           INSERTED.AdministradorId,
                           INSERTED.Descripcion,
                           INSERTED.ImagenPrincipalId,
                           INSERTED.MonedaId,
                           INSERTED.Activo,
                           INSERTED.FechaCreacion,
                           INSERTED.FechaModificacion,
                           INSERTED.UsuarioCreadorId,
                           INSERTED.UsuarioModificadorId
                    WHERE InmuebleID = @InmuebleID;";

                var inmuebleActualizado = await conexion.QuerySingleOrDefaultAsync<Inmueble>(sqlActualizarInmueble, new
                {
                    InmuebleID = inmueble.Id,
                    inmueble.Nombre,
                    inmueble.Direccion,
                    inmueble.Pais,
                    inmueble.Estado,
                    inmueble.Ciudad,
                    CodigoPostal = inmueble.CodigoPostal,
                    inmueble.Latitud,
                    inmueble.Longitud,
                    inmueble.Valor,
                    ConstruccionM2 = inmueble.ConstruccionM2,
                    inmueble.RentaMensual,
                    AdministradorId = (object?)inmueble.AdministradorId ?? DBNull.Value,
                    inmueble.Descripcion,
                    ImagenPrincipalId = (object?)inmueble.ImagenPrincipalId ?? DBNull.Value,
                    inmueble.MonedaId,
                    inmueble.Activo,
                    UsuarioModificadorId = (object?)inmueble.UsuarioModificadorId ?? DBNull.Value
                }, transaccion);

                if (inmuebleActualizado == null)
                    throw new Exception("No se pudo actualizar el inmueble.");

                transaccion.Commit();

                return new DTO<Inmueble>
                {
                    Correcto = true,
                    Datos = inmuebleActualizado,
                    Mensaje = "Inmueble actualizado correctamente."
                };
            }
            catch (Exception ex)
            {
                transaccion.Rollback();
                return new DTO<Inmueble>
                {
                    Correcto = false,
                    Mensaje = $"Error al actualizar inmueble: {ex.Message}"
                };
            }
        }

        public async Task<DTO<bool>> Eliminar(Inmueble inmueble)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            using var transaccion = conexion.BeginTransaction();

            try
            {
                // Verificar si tiene contratos activos
                var sqlVerificarContratos = @"SELECT COUNT(*) FROM ContratoRenta WHERE InmuebleId = @InmuebleID;";
                var tieneContratos = await conexion.QuerySingleAsync<int>(sqlVerificarContratos, new { InmuebleID = inmueble.Id }, transaccion);

                if (tieneContratos > 0)
                {
                    return new DTO<bool>
                    {
                        Correcto = false,
                        Datos = false,
                        Mensaje = "No se puede eliminar el inmueble porque tiene contratos asociados."
                    };
                }

                // Eliminar el inmueble
                var sqlEliminarInmueble = @"DELETE FROM Inmueble WHERE InmuebleID = @InmuebleID;";
                var filasAfectadas = await conexion.ExecuteAsync(sqlEliminarInmueble, new { InmuebleID = inmueble.Id }, transaccion);

                transaccion.Commit();

                return new DTO<bool>
                {
                    Correcto = filasAfectadas > 0,
                    Datos = filasAfectadas > 0,
                    Mensaje = filasAfectadas > 0 ? "Inmueble eliminado correctamente." : "No se encontró el inmueble."
                };
            }
            catch (Exception ex)
            {
                transaccion.Rollback();
                return new DTO<bool>
                {
                    Correcto = false,
                    Mensaje = $"Error al eliminar inmueble: {ex.Message}"
                };
            }
        }

        public async Task<DTO<Inmueble>> Obtener_por_id(Inmueble inmueble)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();

            var sqlInmueble = @"SELECT 
                                i.InmuebleID as Id,
                                i.Nombre,
                                i.Direccion,
                                i.Pais,
                                i.Estado,
                                i.Ciudad,
                                i.CP as CodigoPostal,
                                i.Latitud,
                                i.Longitud,
                                i.Valor,
                                i.ConstruccionM2,
                                i.RentaMensual,
                                i.AdministradorId,
                                i.Descripcion,
                                i.ImagenPrincipalId,
                                i.MonedaId,
                                i.Activo,
                                i.FechaCreacion,
                                i.FechaModificacion,
                                i.UsuarioCreadorId,
                                i.UsuarioModificadorId,
                                m.MonedaID as Moneda_Id,
                                m.Codigo as Moneda_Codigo,
                                m.Nombre as Moneda_Nombre,
                                uc.UsuarioID as UsuarioCreador_Id,
                                uc.Nombre as UsuarioCreador_Nombre,
                                um.UsuarioID as UsuarioModificador_Id,
                                um.Nombre as UsuarioModificador_Nombre,
                                adm.UsuarioID as Administrador_Id,
                                adm.Nombre as Administrador_Nombre,
                                img.Id as ImagenPrincipal_Id,
                                img.Nombre as ImagenPrincipal_Nombre,
                                img.Descripcion as ImagenPrincipal_Descripcion,
                                img.Url as ImagenPrincipal_Url,
                                img.UrlThumb as ImagenPrincipal_UrlThumb,
                                img.FechaCreacion as ImagenPrincipal_FechaCreacion
                              FROM Inmueble i
                              LEFT JOIN Moneda m ON i.MonedaId = m.MonedaID
                              LEFT JOIN Usuario uc ON i.UsuarioCreadorId = uc.UsuarioID
                              LEFT JOIN Usuario um ON i.UsuarioModificadorId = um.UsuarioID
                              LEFT JOIN Usuario adm ON i.AdministradorId = adm.UsuarioID
                              LEFT JOIN Imagen img ON i.ImagenPrincipalId = img.Id
                              WHERE i.InmuebleID = @InmuebleID;";

            var sqlCaracteristicas = @"SELECT 
                                        ci.CaracteristicaInmuebleID as Id,
                                        ci.InmuebleID,
                                        ci.CaracteristicaID,
                                        ci.Valor,
                                        c.CaracteristicaID as Caracteristica_Id,
                                        c.Nombre as Caracteristica_Nombre,
                                        c.Tipo as Caracteristica_Tipo,
                                        c.Descripcion as Caracteristica_Descripcion
                                      FROM CaracteristicaInmueble ci
                                      LEFT JOIN Caracteristica c ON ci.CaracteristicaID = c.CaracteristicaID
                                      WHERE ci.InmuebleID = @InmuebleID;";

            var inmuebleData = await conexion.QuerySingleOrDefaultAsync(sqlInmueble, new { InmuebleID = inmueble.Id });
            if (inmuebleData == null)
                return new DTO<Inmueble> { Correcto = false, Mensaje = "Inmueble no encontrado" };

            var inmuebleEncontrado = new Inmueble
            {
                Id = inmuebleData.Id,
                Nombre = inmuebleData.Nombre,
                Direccion = inmuebleData.Direccion,
                Pais = inmuebleData.Pais,
                Estado = inmuebleData.Estado,
                Ciudad = inmuebleData.Ciudad,
                CodigoPostal = inmuebleData.CodigoPostal,
                Latitud = inmuebleData.Latitud,
                Longitud = inmuebleData.Longitud,
                Valor = inmuebleData.Valor,
                ConstruccionM2 = inmuebleData.ConstruccionM2,
                RentaMensual = inmuebleData.RentaMensual,
                AdministradorId = inmuebleData.AdministradorId,
                Descripcion = inmuebleData.Descripcion,
                ImagenPrincipalId = inmuebleData.ImagenPrincipalId,
                MonedaId = inmuebleData.MonedaId,
                Activo = inmuebleData.Activo,
                FechaCreacion = inmuebleData.FechaCreacion,
                FechaModificacion = inmuebleData.FechaModificacion,
                UsuarioCreadorId = inmuebleData.UsuarioCreadorId,
                UsuarioModificadorId = inmuebleData.UsuarioModificadorId
            };

            // Mapear relaciones
            if (inmuebleData.Moneda_Id != null)
            {
                inmuebleEncontrado.Moneda = new Moneda
                {
                    Id = inmuebleData.Moneda_Id,
                    Codigo = inmuebleData.Moneda_Codigo,
                    Nombre = inmuebleData.Moneda_Nombre
                };
            }

            if (inmuebleData.UsuarioCreador_Id != null)
            {
                inmuebleEncontrado.UsuarioCreador = new Usuario
                {
                    Id = inmuebleData.UsuarioCreador_Id,
                    Nombre = inmuebleData.UsuarioCreador_Nombre
                };
            }

            if (inmuebleData.UsuarioModificador_Id != null)
            {
                inmuebleEncontrado.UsuarioModificador = new Usuario
                {
                    Id = inmuebleData.UsuarioModificador_Id,
                    Nombre = inmuebleData.UsuarioModificador_Nombre
                };
            }

            if (inmuebleData.Administrador_Id != null)
            {
                inmuebleEncontrado.Administrador = new Usuario
                {
                    Id = inmuebleData.Administrador_Id,
                    Nombre = inmuebleData.Administrador_Nombre
                };
            }

            if (inmuebleData.ImagenPrincipal_Id != null)
            {
                inmuebleEncontrado.ImagenPrincipal = new Imagen
                {
                    Id = inmuebleData.ImagenPrincipal_Id,
                    Nombre = inmuebleData.ImagenPrincipal_Nombre,
                    Descripcion = inmuebleData.ImagenPrincipal_Descripcion,
                    Url = inmuebleData.ImagenPrincipal_Url,
                    UrlThumb = inmuebleData.ImagenPrincipal_UrlThumb,
                    FechaCreacion = inmuebleData.ImagenPrincipal_FechaCreacion
                };
            }

            // Cargar características del inmueble
            var caracteristicasData = await conexion.QueryAsync(sqlCaracteristicas, new { InmuebleID = inmueble.Id });
            foreach (var caracteristicaData in caracteristicasData)
            {
                var caracteristicaInmueble = new CaracteristicaInmueble
                {
                    Id = caracteristicaData.Id,
                    InmuebleID = caracteristicaData.InmuebleID,
                    CaracteristicaID = caracteristicaData.CaracteristicaID,
                    Valor = caracteristicaData.Valor
                };

                // Mapear la característica relacionada
                if (caracteristicaData.Caracteristica_Id != null)
                {
                    caracteristicaInmueble.Caracteristica = new Caracteristica
                    {
                        Id = caracteristicaData.Caracteristica_Id,
                        Nombre = caracteristicaData.Caracteristica_Nombre,
                        Tipo = caracteristicaData.Caracteristica_Tipo,
                        Descripcion = caracteristicaData.Caracteristica_Descripcion
                    };
                }

                inmuebleEncontrado.Caracteristicas.Add(caracteristicaInmueble);
            }

            return new DTO<Inmueble>
            {
                Correcto = true,
                Datos = inmuebleEncontrado,
                Mensaje = "Inmueble obtenido correctamente"
            };
        }

        public async Task<DTO<IEnumerable<Inmueble>>> Obtener_todos()
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();

            var sqlInmuebles = @"SELECT 
                                 i.InmuebleID as Id,
                                 i.Nombre,
                                 i.Direccion,
                                 i.Pais,
                                 i.Estado,
                                 i.Ciudad,
                                 i.CP as CodigoPostal,
                                 i.Latitud,
                                 i.Longitud,
                                 i.Valor,
                                 i.ConstruccionM2,
                                 i.RentaMensual,
                                 i.AdministradorId,
                                 i.Descripcion,
                                 i.ImagenPrincipalId,
                                 i.MonedaId,
                                 i.Activo,
                                 i.FechaCreacion,
                                 i.FechaModificacion,
                                 i.UsuarioCreadorId,
                                 i.UsuarioModificadorId,
                                 m.MonedaID as Moneda_Id,
                                 m.Codigo as Moneda_Codigo,
                                 m.Nombre as Moneda_Nombre,
                                 img.Id as ImagenPrincipal_Id,
                                 img.Nombre as ImagenPrincipal_Nombre,
                                 img.Url as ImagenPrincipal_Url,
                                 img.UrlThumb as ImagenPrincipal_UrlThumb
                               FROM Inmueble i
                               LEFT JOIN Moneda m ON i.MonedaId = m.MonedaID
                               LEFT JOIN Imagen img ON i.ImagenPrincipalId = img.Id;";

            var inmueblesData = (await conexion.QueryAsync(sqlInmuebles)).ToList();
            var inmuebles = new List<Inmueble>();

            foreach (var inmuebleData in inmueblesData)
            {
                var inmueble = new Inmueble
                {
                    Id = inmuebleData.Id,
                    Nombre = inmuebleData.Nombre,
                    Direccion = inmuebleData.Direccion,
                    Pais = inmuebleData.Pais,
                    Estado = inmuebleData.Estado,
                    Ciudad = inmuebleData.Ciudad,
                    CodigoPostal = inmuebleData.CodigoPostal,
                    Latitud = inmuebleData.Latitud,
                    Longitud = inmuebleData.Longitud,
                    Valor = inmuebleData.Valor,
                    ConstruccionM2 = inmuebleData.ConstruccionM2,
                    RentaMensual = inmuebleData.RentaMensual,
                    AdministradorId = inmuebleData.AdministradorId,
                    Descripcion = inmuebleData.Descripcion,
                    ImagenPrincipalId = inmuebleData.ImagenPrincipalId,
                    MonedaId = inmuebleData.MonedaId,
                    Activo = inmuebleData.Activo,
                    FechaCreacion = inmuebleData.FechaCreacion,
                    FechaModificacion = inmuebleData.FechaModificacion,
                    UsuarioCreadorId = inmuebleData.UsuarioCreadorId,
                    UsuarioModificadorId = inmuebleData.UsuarioModificadorId
                };

                // Mapear la moneda si existe
                if (inmuebleData.Moneda_Id != null)
                {
                    inmueble.Moneda = new Moneda
                    {
                        Id = inmuebleData.Moneda_Id,
                        Codigo = inmuebleData.Moneda_Codigo,
                        Nombre = inmuebleData.Moneda_Nombre
                    };
                }

                // Mapear la imagen principal si existe
                if (inmuebleData.ImagenPrincipal_Id != null)
                {
                    inmueble.ImagenPrincipal = new Imagen
                    {
                        Id = inmuebleData.ImagenPrincipal_Id,
                        Nombre = inmuebleData.ImagenPrincipal_Nombre,
                        Url = inmuebleData.ImagenPrincipal_Url,
                        UrlThumb = inmuebleData.ImagenPrincipal_UrlThumb
                    };
                }

                inmuebles.Add(inmueble);
            }

            return new DTO<IEnumerable<Inmueble>>
            {
                Correcto = true,
                Datos = inmuebles,
                Mensaje = "Inmuebles obtenidos correctamente"
            };
        }

        public async Task<DTO<Items_pagina<Inmueble>>> Obtener_paginado(Filtros_paginado filtros)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();

            var sql = @"SELECT 
                            COUNT(*) OVER() AS TotalItems,
                            i.InmuebleID as Id,
                            i.Nombre,
                            i.Direccion,
                            i.Pais,
                            i.Estado,
                            i.Ciudad,
                            i.CP as CodigoPostal,
                            i.Latitud,
                            i.Longitud,
                            i.Valor,
                            i.ConstruccionM2,
                            i.RentaMensual,
                            i.AdministradorId,
                            i.Descripcion,
                            i.ImagenPrincipalId,
                            i.MonedaId,
                            i.Activo,
                            i.FechaCreacion,
                            i.FechaModificacion,
                            i.UsuarioCreadorId,
                            i.UsuarioModificadorId,
                            m.MonedaID as Moneda_Id,
                            m.Codigo as Moneda_Codigo,
                            m.Nombre as Moneda_Nombre,
                            img.Id as ImagenPrincipal_Id,
                            img.Nombre as ImagenPrincipal_Nombre,
                            img.Url as ImagenPrincipal_Url,
                            img.UrlThumb as ImagenPrincipal_UrlThumb
                        FROM Inmueble i
                        LEFT JOIN Moneda m ON i.MonedaId = m.MonedaID
                        LEFT JOIN Imagen img ON i.ImagenPrincipalId = img.Id
                        WHERE 
                            (@FiltroBusqueda IS NULL OR i.Nombre LIKE '%' + @FiltroBusqueda + '%' 
                             OR i.Direccion LIKE '%' + @FiltroBusqueda + '%' 
                             OR i.Pais LIKE '%' + @FiltroBusqueda + '%'
                             OR i.Estado LIKE '%' + @FiltroBusqueda + '%'
                             OR i.Ciudad LIKE '%' + @FiltroBusqueda + '%')
                        ORDER BY
                            CASE WHEN @OrdenarPor = 'Nombre' THEN i.Nombre END,
                            CASE WHEN @OrdenarPor = 'Valor' THEN i.Valor END,
                            CASE WHEN @OrdenarPor = 'FechaCreacion' THEN i.FechaCreacion END
                        OFFSET @Skip ROWS FETCH NEXT @Top ROWS ONLY;";

            var lista = await conexion.QueryAsync<dynamic>(sql, new
            {
                FiltroBusqueda = (object?)filtros.Filter ?? DBNull.Value,
                OrdenarPor = (object?)filtros.OrderBy ?? "Nombre",
                Skip = filtros.Skip < 0 ? 0 : filtros.Skip,
                Top = filtros.Top <= 0 ? 10 : filtros.Top
            });

            var inmuebles = new List<Inmueble>();
            int totalItems = 0;

            foreach (var row in lista)
            {
                totalItems = row.TotalItems;

                var inmueble = new Inmueble
                {
                    Id = row.Id,
                    Nombre = row.Nombre,
                    Direccion = row.Direccion,
                    Pais = row.Pais,
                    Estado = row.Estado,
                    Ciudad = row.Ciudad,
                    CodigoPostal = row.CodigoPostal,
                    Latitud = row.Latitud,
                    Longitud = row.Longitud,
                    Valor = row.Valor,
                    ConstruccionM2 = row.ConstruccionM2,
                    RentaMensual = row.RentaMensual,
                    AdministradorId = row.AdministradorId,
                    Descripcion = row.Descripcion,
                    ImagenPrincipalId = row.ImagenPrincipalId,
                    MonedaId = row.MonedaId,
                    Activo = row.Activo,
                    FechaCreacion = row.FechaCreacion,
                    FechaModificacion = row.FechaModificacion,
                    UsuarioCreadorId = row.UsuarioCreadorId,
                    UsuarioModificadorId = row.UsuarioModificadorId
                };

                // Mapear la moneda si existe
                if (row.Moneda_Id != null)
                {
                    inmueble.Moneda = new Moneda
                    {
                        Id = row.Moneda_Id,
                        Codigo = row.Moneda_Codigo,
                        Nombre = row.Moneda_Nombre
                    };
                }

                // Mapear la imagen principal si existe
                if (row.ImagenPrincipal_Id != null)
                {
                    inmueble.ImagenPrincipal = new Imagen
                    {
                        Id = row.ImagenPrincipal_Id,
                        Nombre = row.ImagenPrincipal_Nombre,
                        Url = row.ImagenPrincipal_Url,
                        UrlThumb = row.ImagenPrincipal_UrlThumb
                    };
                }

                inmuebles.Add(inmueble);
            }

            return new DTO<Items_pagina<Inmueble>>
            {
                Correcto = true,
                Datos = new Items_pagina<Inmueble>
                {
                    Total_items = totalItems,
                    Items = inmuebles
                },
                Mensaje = "Inmuebles paginados correctamente."
            };
        }

        public async Task<DTO<IEnumerable<string>>> Obtener_estados()
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();

            var sql = @"SELECT DISTINCT Estado FROM Inmueble WHERE Estado IS NOT NULL AND Estado != '' ORDER BY Estado;";
            var estados = await conexion.QueryAsync<string>(sql);

            return new DTO<IEnumerable<string>>
            {
                Correcto = true,
                Datos = estados,
                Mensaje = "Estados obtenidos correctamente"
            };
        }

        public async Task<DTO<IEnumerable<Inmueble>>> Obtener_por_estado(string estado)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();

            var sql = @"SELECT * FROM Inmueble WHERE Estado = @Estado;";
            var inmuebles = await conexion.QueryAsync<Inmueble>(sql, new { Estado = estado });

            return new DTO<IEnumerable<Inmueble>>
            {
                Correcto = true,
                Datos = inmuebles,
                Mensaje = "Inmuebles obtenidos correctamente"
            };
        }

        public async Task<DTO<IEnumerable<Inmueble>>> Obtener_por_administrador(int administradorId)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();

            var sql = @"SELECT * FROM Inmueble WHERE AdministradorId = @AdministradorId;";
            var inmuebles = await conexion.QueryAsync<Inmueble>(sql, new { AdministradorId = administradorId });

            return new DTO<IEnumerable<Inmueble>>
            {
                Correcto = true,
                Datos = inmuebles,
                Mensaje = "Inmuebles obtenidos correctamente"
            };
        }

        public async Task<DTO<IEnumerable<Inmueble>>> Obtener_por_ubicacion(string pais, string estado, string ciudad)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();

            var sql = @"SELECT * FROM Inmueble 
                        WHERE (@Pais IS NULL OR Pais = @Pais)
                        AND (@Estado IS NULL OR Estado = @Estado)
                        AND (@Ciudad IS NULL OR Ciudad = @Ciudad);";

            var inmuebles = await conexion.QueryAsync<Inmueble>(sql, new 
            { 
                Pais = string.IsNullOrEmpty(pais) ? null : pais,
                Estado = string.IsNullOrEmpty(estado) ? null : estado,
                Ciudad = string.IsNullOrEmpty(ciudad) ? null : ciudad
            });

            return new DTO<IEnumerable<Inmueble>>
            {
                Correcto = true,
                Datos = inmuebles,
                Mensaje = "Inmuebles obtenidos correctamente"
            };
        }

        public async Task<DTO<IEnumerable<Inmueble>>> Obtener_por_rango_precio(decimal precioMin, decimal precioMax)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();

            var sql = @"SELECT * FROM Inmueble 
                        WHERE Valor BETWEEN @PrecioMin AND @PrecioMax
                        ORDER BY Valor;";

            var inmuebles = await conexion.QueryAsync<Inmueble>(sql, new { PrecioMin = precioMin, PrecioMax = precioMax });

            return new DTO<IEnumerable<Inmueble>>
            {
                Correcto = true,
                Datos = inmuebles,
                Mensaje = "Inmuebles obtenidos correctamente"
            };
        }

        public async Task<DTO<IEnumerable<CaracteristicaInmueble>>> Obtener_caracteristicas_inmueble(int inmuebleId)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();

            var sql = @"SELECT 
                            ci.CaracteristicaInmuebleID as Id,
                            ci.InmuebleID,
                            ci.CaracteristicaID,
                            ci.Valor,
                            c.CaracteristicaID as Caracteristica_Id,
                            c.Nombre as Caracteristica_Nombre,
                            c.Tipo as Caracteristica_Tipo,
                            c.Descripcion as Caracteristica_Descripcion
                        FROM CaracteristicaInmueble ci
                        LEFT JOIN Caracteristica c ON ci.CaracteristicaID = c.CaracteristicaID
                        WHERE ci.InmuebleID = @InmuebleId;";

            var caracteristicasData = await conexion.QueryAsync(sql, new { InmuebleId = inmuebleId });
            var caracteristicas = new List<CaracteristicaInmueble>();

            foreach (var data in caracteristicasData)
            {
                var caracteristica = new CaracteristicaInmueble
                {
                    Id = data.Id,
                    InmuebleID = data.InmuebleID,
                    CaracteristicaID = data.CaracteristicaID,
                    Valor = data.Valor
                };

                if (data.Caracteristica_Id != null)
                {
                    caracteristica.Caracteristica = new Caracteristica
                    {
                        Id = data.Caracteristica_Id,
                        Nombre = data.Caracteristica_Nombre,
                        Tipo = data.Caracteristica_Tipo,
                        Descripcion = data.Caracteristica_Descripcion
                    };
                }

                caracteristicas.Add(caracteristica);
            }

            return new DTO<IEnumerable<CaracteristicaInmueble>>
            {
                Correcto = true,
                Datos = caracteristicas,
                Mensaje = "Características obtenidas correctamente"
            };
        }

        public async Task<DTO<CaracteristicaInmueble>> Agregar_caracteristica_inmueble(CaracteristicaInmueble caracteristica)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            using var transaccion = conexion.BeginTransaction();

            try
            {
                var sql = @"INSERT INTO CaracteristicaInmueble (InmuebleID, CaracteristicaID, Valor)
                           OUTPUT INSERTED.CaracteristicaInmuebleID as Id,
                                  INSERTED.InmuebleID,
                                  INSERTED.CaracteristicaID,
                                  INSERTED.Valor
                           VALUES (@InmuebleID, @CaracteristicaID, @Valor);";

                var caracteristicaCreada = await conexion.QuerySingleOrDefaultAsync<CaracteristicaInmueble>(sql, new
                {
                    caracteristica.InmuebleID,
                    caracteristica.CaracteristicaID,
                    caracteristica.Valor
                }, transaccion);

                if (caracteristicaCreada == null)
                    throw new Exception("No se pudo agregar la característica al inmueble.");

                transaccion.Commit();

                return new DTO<CaracteristicaInmueble>
                {
                    Correcto = true,
                    Datos = caracteristicaCreada,
                    Mensaje = "Característica agregada correctamente."
                };
            }
            catch (Exception ex)
            {
                transaccion.Rollback();
                return new DTO<CaracteristicaInmueble>
                {
                    Correcto = false,
                    Mensaje = $"Error al agregar característica: {ex.Message}"
                };
            }
        }

        public async Task<DTO<CaracteristicaInmueble>> Actualizar_caracteristica_inmueble(CaracteristicaInmueble caracteristica)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            using var transaccion = conexion.BeginTransaction();

            try
            {
                var sql = @"UPDATE CaracteristicaInmueble
                           SET Valor = @Valor
                           OUTPUT INSERTED.CaracteristicaInmuebleID as Id,
                                  INSERTED.InmuebleID,
                                  INSERTED.CaracteristicaID,
                                  INSERTED.Valor
                           WHERE CaracteristicaInmuebleID = @Id;";

                var caracteristicaActualizada = await conexion.QuerySingleOrDefaultAsync<CaracteristicaInmueble>(sql, new
                {
                    caracteristica.Id,
                    caracteristica.Valor
                }, transaccion);

                if (caracteristicaActualizada == null)
                    throw new Exception("No se pudo actualizar la característica.");

                transaccion.Commit();

                return new DTO<CaracteristicaInmueble>
                {
                    Correcto = true,
                    Datos = caracteristicaActualizada,
                    Mensaje = "Característica actualizada correctamente."
                };
            }
            catch (Exception ex)
            {
                transaccion.Rollback();
                return new DTO<CaracteristicaInmueble>
                {
                    Correcto = false,
                    Mensaje = $"Error al actualizar característica: {ex.Message}"
                };
            }
        }

        public async Task<DTO<bool>> Eliminar_caracteristica_inmueble(int caracteristicaInmuebleId)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            using var transaccion = conexion.BeginTransaction();

            try
            {
                var sql = @"DELETE FROM CaracteristicaInmueble WHERE CaracteristicaInmuebleID = @Id;";
                var filasAfectadas = await conexion.ExecuteAsync(sql, new { Id = caracteristicaInmuebleId }, transaccion);

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
    }
}