-- Tabla Propiedad Tipo
CREATE TABLE PROPIEDAD_TIPO (
    PRO_TIP_ID INT IDENTITY(1,1) PRIMARY KEY,
    PRO_TIP_DESCRIPCION NVARCHAR(100) NOT NULL
);

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Inserta un nuevo registro en la tabla PROPIEDAD_TIPO.
-- Entradas: @Descripcion NVARCHAR(100).
-- Salidas: ID del registro insertado.
-- ====================================================================================
CREATE PROCEDURE sp_Propiedad_Tipo_Insertar
    @Descripcion NVARCHAR(100),
	@NuevoId INT OUTPUT 
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO PROPIEDAD_TIPO (PRO_TIP_DESCRIPCION)
    VALUES (@Descripcion);

	SET @NuevoId = SCOPE_IDENTITY();
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve todos los registros de la tabla PROPIEDAD_TIPO.
-- Entradas: Ninguna.
-- Salidas: Todos los registros de PROPIEDAD_TIPO.
-- ====================================================================================
CREATE PROCEDURE sp_Propiedad_Tipo_ObtenerTodos
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM PROPIEDAD_TIPO;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve un registro específico de PROPIEDAD_TIPO basado en su ID.
-- Entradas: @Id INT - ID del tipo de propiedad.
-- Salidas: Registro correspondiente al ID.
-- ====================================================================================
CREATE PROCEDURE sp_Propiedad_Tipo_ObtenerPorId
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM PROPIEDAD_TIPO WHERE PRO_TIP_ID = @Id;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Actualiza un registro en PROPIEDAD_TIPO basado en su ID.
-- Entradas: @Id INT, @Descripcion NVARCHAR(100).
-- Salidas: ID del registro actualizado.
-- ====================================================================================
CREATE PROCEDURE sp_Propiedad_Tipo_Actualizar
    @Id INT,
    @Descripcion NVARCHAR(100)
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE PROPIEDAD_TIPO
    SET PRO_TIP_DESCRIPCION = @Descripcion
    WHERE PRO_TIP_ID = @Id;

    SELECT @Id AS ID;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Elimina un registro de PROPIEDAD_TIPO basado en su ID.
-- Entradas: @Id INT - ID del tipo de propiedad.
-- Salidas: Ninguna.
-- ====================================================================================
CREATE PROCEDURE sp_Propiedad_Tipo_Eliminar
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	DELETE FROM PROPIEDAD_TIPO WHERE PRO_TIP_ID = @Id;
END;
GO

-- =============================================
-- Script de Pruebas: PROPIEDAD_TIPO 
-- =============================================

SET NOCOUNT ON;

PRINT '*** Validación inicial: Mostrar todos los registros existentes en PROPIEDAD_TIPO ***';
SELECT * FROM PROPIEDAD_TIPO;

-- Variables para pruebas
DECLARE @TipoId INT;
DECLARE @Descripcion NVARCHAR(100) = 'Apartamento';

-- *** Prueba: Insertar un nuevo registro en PROPIEDAD_TIPO ***
PRINT '*** Prueba: Insertar un nuevo registro en PROPIEDAD_TIPO ***';
EXEC sp_Propiedad_Tipo_Insertar
    @Descripcion = @Descripcion,
    @NuevoId = @TipoId OUTPUT;

PRINT 'ID del nuevo tipo de propiedad: ' + CAST(@TipoId AS NVARCHAR);

-- *** Validación: Mostrar todos los registros después de la inserción ***
PRINT '*** Validación: Mostrar todos los registros después de la inserción ***';
SELECT * FROM PROPIEDAD_TIPO;

-- *** Prueba: Obtener registro de PROPIEDAD_TIPO por ID ***
PRINT '*** Prueba: Obtener registro de PROPIEDAD_TIPO por ID ***';
EXEC sp_Propiedad_Tipo_ObtenerPorId
    @Id = @TipoId;

-- *** Prueba: Actualizar un registro en PROPIEDAD_TIPO ***
PRINT '*** Prueba: Actualizar un registro en PROPIEDAD_TIPO ***';
SET @Descripcion = 'Casa';
EXEC sp_Propiedad_Tipo_Actualizar
    @Id = @TipoId,
    @Descripcion = @Descripcion;

-- *** Validación: Mostrar el registro actualizado ***
PRINT '*** Validación: Mostrar el registro actualizado ***';
EXEC sp_Propiedad_Tipo_ObtenerPorId
    @Id = @TipoId;

-- *** Prueba: Eliminar un registro en PROPIEDAD_TIPO ***
PRINT '*** Prueba: Eliminar un registro en PROPIEDAD_TIPO ***';
EXEC sp_Propiedad_Tipo_Eliminar
    @Id = @TipoId;

-- *** Validación: Mostrar todos los registros después de la eliminación ***
PRINT '*** Validación: Mostrar todos los registros después de la eliminación ***';
SELECT * FROM PROPIEDAD_TIPO;

-- *** Validación final: Confirmar que no quedan registros pendientes ***
PRINT '*** Validación final: Confirmar que no quedan registros pendientes ***';
SELECT * FROM PROPIEDAD_TIPO;
