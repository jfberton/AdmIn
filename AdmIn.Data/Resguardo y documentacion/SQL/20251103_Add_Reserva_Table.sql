-- Script:20251103_Add_Reserva_Table.sql
-- Descripción: Crea la tabla `Reserva`, agrega índice único para evitar más de una reserva activa por inmueble,
-- agrega columna `ReservaId` en `ContratoRenta` (si aplica) y seed de estados de `InmuebleCondicion`.
-- Ejecución: Ejecutar desde SQL Server Management Studio o el administrador de la base de datos del proyecto.

SET XACT_ABORT ON;
BEGIN TRANSACTION;

--1) Crear tabla Reserva si no existe
IF OBJECT_ID('dbo.Reserva','U') IS NULL
BEGIN
 CREATE TABLE dbo.Reserva (
 Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
 InmuebleId INT NOT NULL,
 UsuarioReservadorId INT NOT NULL,
 FechaCreacion DATETIME2 NOT NULL CONSTRAINT DF_Reserva_FechaCreacion DEFAULT (SYSUTCDATETIME()),
 FechaVencimiento DATETIME2 NOT NULL,
 Costo DECIMAL(18,2) NOT NULL,
 MonedaId INT NULL,
 Estado INT NOT NULL DEFAULT(0), -- enum:0=Activa,1=Convertida,2=Vencida,3=Cancelada
 ContratoRentaId INT NULL,
 AplicadoAlContrato BIT NOT NULL DEFAULT(0),
 MontoAplicadoAlContrato DECIMAL(18,2) NULL,
 FechaModificacion DATETIME2 NOT NULL CONSTRAINT DF_Reserva_FechaModificacion DEFAULT (SYSUTCDATETIME()),
 UsuarioCreadorId INT NULL,
 UsuarioModificadorId INT NULL
 );

 -- Índices
 CREATE INDEX IX_Reserva_Inmueble_Estado ON dbo.Reserva(InmuebleId, Estado);

 -- Índice único filtrado para garantizar sólo UNA reserva activa por inmueble (Estado =0 => Activa)
 -- Nota: índice filtrado requiere SQL Server2008+.
 CREATE UNIQUE INDEX UX_Reserva_Inmueble_Activa ON dbo.Reserva(InmuebleId) WHERE Estado =0;

 -- Claves foráneas condicionales: sólo se crean si existen las tablas destino
 IF OBJECT_ID('dbo.Inmueble','U') IS NOT NULL
 BEGIN
 IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Reserva_Inmueble')
 BEGIN
 ALTER TABLE dbo.Reserva ADD CONSTRAINT FK_Reserva_Inmueble FOREIGN KEY (InmuebleId) REFERENCES dbo.Inmueble(InmuebleID);
 END
 END

 IF OBJECT_ID('dbo.Usuario','U') IS NOT NULL
 BEGIN
 IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Reserva_UsuarioReservador')
 BEGIN
 ALTER TABLE dbo.Reserva ADD CONSTRAINT FK_Reserva_UsuarioReservador FOREIGN KEY (UsuarioReservadorId) REFERENCES dbo.Usuario(UsuarioID);
 END
 END

 IF OBJECT_ID('dbo.Moneda','U') IS NOT NULL
 BEGIN
 IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Reserva_Moneda')
 BEGIN
 ALTER TABLE dbo.Reserva ADD CONSTRAINT FK_Reserva_Moneda FOREIGN KEY (MonedaId) REFERENCES dbo.Moneda(MonedaID);
 END
 END

 -- Si existe la tabla ContratoRenta (uso en C#), crea FK hacia ella
 IF OBJECT_ID('dbo.ContratoRenta','U') IS NOT NULL
 BEGIN
 IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Reserva_ContratoRenta')
 BEGIN
 ALTER TABLE dbo.Reserva ADD CONSTRAINT FK_Reserva_ContratoRenta FOREIGN KEY (ContratoRentaId) REFERENCES dbo.ContratoRenta(Id);
 END
 END
END;

--2) Agregar columna ReservaId en ContratoRenta si la tabla existe y la columna no existe
IF OBJECT_ID('dbo.ContratoRenta','U') IS NOT NULL
BEGIN
 IF COL_LENGTH('dbo.ContratoRenta','ReservaId') IS NULL
 BEGIN
 ALTER TABLE dbo.ContratoRenta ADD ReservaId INT NULL;
 END

 -- Crear FK desde ContratoRenta.ReservaId hacia Reserva.Id si Reserva ya existe
 IF OBJECT_ID('dbo.Reserva','U') IS NOT NULL
 BEGIN
 IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_ContratoRenta_Reserva')
 BEGIN
 ALTER TABLE dbo.ContratoRenta ADD CONSTRAINT FK_ContratoRenta_Reserva FOREIGN KEY (ReservaId) REFERENCES dbo.Reserva(Id);
 END
 END
END;

--3) Seed (UPSERT) de valores en tabla InmuebleCondicion para los estados esperados
-- Valores actuales:
-- Disponible El inmueble está disponible para alquiler/venta #10b981 Id=1 Orden=1
-- Ocupado El inmueble está actualmente ocupado #374151 Id=2 Orden=2
-- Reservado El inmueble está reservado pero no ocupado #3b82f6 Id=3 Orden=3
-- En reparación El inmueble está en proceso de reparación #f59e0b Id=4 Orden=4
-- En mantenimiento El inmueble está en mantenimiento preventivo #6b7280 Id=5 Orden=5

IF OBJECT_ID('dbo.InmuebleCondicion','U') IS NOT NULL
BEGIN
 -- Upsert helper: si existe actualiza, si no inserta
 MERGE dbo.InmuebleCondicion AS target
 USING (VALUES
 (1, 'Disponible', 'El inmueble está disponible para alquiler/venta', '#10b981',1),
 (2, 'Ocupado', 'El inmueble está actualmente ocupado', '#374151',2),
 (3, 'Reservado', 'El inmueble está reservado pero no ocupado', '#3b82f6',3),
 (4, 'En reparación', 'El inmueble está en proceso de reparación', '#f59e0b',4),
 (5, 'En mantenimiento', 'El inmueble está en mantenimiento preventivo', '#6b7280',5)
 ) AS src (Id, Nombre, Descripcion, Color, Orden)
 ON (target.Id = src.Id)
 WHEN MATCHED THEN
 UPDATE SET target.Nombre = src.Nombre,
 target.Descripcion = src.Descripcion,
 target.Color = src.Color,
 target.Activo =1,
 target.Orden = src.Orden,
 target.FechaModificacion = SYSUTCDATETIME()
 WHEN NOT MATCHED BY TARGET THEN
 INSERT (Id, Nombre, Descripcion, Color, Activo, Orden, FechaCreacion, FechaModificacion)
 VALUES (src.Id, src.Nombre, src.Descripcion, src.Color,1, src.Orden, SYSUTCDATETIME(), SYSUTCDATETIME());
END;

--4) Recomendaciones: revisar permisos y backups antes de ejecutar en producción

COMMIT TRANSACTION;

PRINT 'Script ejecutado correctamente. Verifique la existencia de las tablas referenciadas y los permisos.';
GO
