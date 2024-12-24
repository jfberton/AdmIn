-- Tabla Especialidad de Aliados
CREATE TABLE ALIADO_ESPECIALIDAD (
    ALI_ESP_ID INT IDENTITY(1,1) PRIMARY KEY,
    ALI_ESP_DESCRIPCION NVARCHAR(100) NOT NULL
);

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Inserta un nuevo registro en la tabla ALIADO_ESPECIALIDAD.
-- Entradas: @Descripcion NVARCHAR(100).
-- Salidas: ID del registro insertado.
-- ====================================================================================
CREATE PROCEDURE sp_Aliado_Especialidad_Insertar
    @Descripcion NVARCHAR(100),
	@NuevoId INT OUTPUT 
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO ALIADO_ESPECIALIDAD (ALI_ESP_DESCRIPCION)
    VALUES (@Descripcion);

	SET @NuevoId = SCOPE_IDENTITY();
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve todos los registros de la tabla ALIADO_ESPECIALIDAD.
-- Entradas: Ninguna.
-- Salidas: Todos los registros de ALIADO_ESPECIALIDAD.
-- ====================================================================================
CREATE PROCEDURE sp_Aliado_Especialidad_ObtenerTodos
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM ALIADO_ESPECIALIDAD;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve un registro específico de ALIADO_ESPECIALIDAD basado en su ID.
-- Entradas: @Id INT - ID de la especialidad del aliado.
-- Salidas: Registro correspondiente al ID.
-- ====================================================================================
CREATE PROCEDURE sp_Aliado_Especialidad_ObtenerPorId
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM ALIADO_ESPECIALIDAD WHERE ALI_ESP_ID = @Id;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Actualiza un registro en ALIADO_ESPECIALIDAD basado en su ID.
-- Entradas: @Id INT, @Descripcion NVARCHAR(100).
-- Salidas: ID del registro actualizado.
-- ====================================================================================
CREATE PROCEDURE sp_Aliado_Especialidad_Actualizar
    @Id INT,
    @Descripcion NVARCHAR(100)
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE ALIADO_ESPECIALIDAD
    SET ALI_ESP_DESCRIPCION = @Descripcion
    WHERE ALI_ESP_ID = @Id;

    SELECT @Id AS ID;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Elimina un registro de ALIADO_ESPECIALIDAD basado en su ID.
-- Entradas: @Id INT - ID de la especialidad del aliado.
-- Salidas: Ninguna.
-- ====================================================================================
CREATE PROCEDURE sp_Aliado_Especialidad_Eliminar
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	DELETE FROM ALIADO_ESPECIALIDAD WHERE ALI_ESP_ID = @Id;
END;
GO

-- ====================================================================================
-- Script de Pruebas para ALIADO_ESPECIALIDAD
-- ====================================================================================

-- =============================================
-- PRIMERA PARTE: CREACIÓN DE REGISTROS
-- =============================================
PRINT '*** Creación de registros en la tabla ALIADO_ESPECIALIDAD usando el procedimiento almacenado ***';
DECLARE @EspecialidadId1 INT, @EspecialidadId2 INT, @EspecialidadId3 INT;

-- Insertar registros en ALIADO_ESPECIALIDAD usando el procedimiento almacenado
EXEC sp_Aliado_Especialidad_Insertar 
    @Descripcion = 'Especialidad Médica',
    @NuevoId = @EspecialidadId1 OUTPUT;
PRINT 'ID de Especialidad 1: ' + CAST(@EspecialidadId1 AS NVARCHAR);

EXEC sp_Aliado_Especialidad_Insertar 
    @Descripcion = 'Especialidad Jurídica',
    @NuevoId = @EspecialidadId2 OUTPUT;
PRINT 'ID de Especialidad 2: ' + CAST(@EspecialidadId2 AS NVARCHAR);

EXEC sp_Aliado_Especialidad_Insertar 
    @Descripcion = 'Especialidad Técnica',
    @NuevoId = @EspecialidadId3 OUTPUT;
PRINT 'ID de Especialidad 3: ' + CAST(@EspecialidadId3 AS NVARCHAR);

-- =============================================
-- VALIDACIÓN INICIAL DE REGISTROS
-- =============================================
PRINT '*** Validación inicial: Mostrar todos los registros existentes en ALIADO_ESPECIALIDAD ***';
EXEC sp_Aliado_Especialidad_ObtenerTodos;

-- =============================================
-- PRUEBA DE OBTENCIÓN POR ID
-- =============================================
PRINT '*** Prueba: Obtener registro de ALIADO_ESPECIALIDAD por ID ***';
DECLARE @TestId INT = @EspecialidadId1;
EXEC sp_Aliado_Especialidad_ObtenerPorId @Id = @TestId;

-- =============================================
-- PRUEBA DE ACTUALIZACIÓN
-- =============================================
PRINT '*** Prueba: Actualizar un registro en ALIADO_ESPECIALIDAD ***';
DECLARE @ActualizarId INT = @EspecialidadId2;

EXEC sp_Aliado_Especialidad_Actualizar 
    @Id = @ActualizarId,
    @Descripcion = 'Especialidad Jurídica Actualizada';
PRINT 'Registro actualizado con ID: ' + CAST(@ActualizarId AS NVARCHAR);

-- Validar actualización
PRINT '*** Validación después de la actualización ***';
EXEC sp_Aliado_Especialidad_ObtenerTodos;

-- =============================================
-- PRUEBA DE ELIMINACIÓN
-- =============================================
PRINT '*** Prueba: Eliminar un registro en ALIADO_ESPECIALIDAD ***';
DECLARE @EliminarId INT = @EspecialidadId3;

EXEC sp_Aliado_Especialidad_Eliminar @Id = @EliminarId;
PRINT 'Registro eliminado con ID: ' + CAST(@EliminarId AS NVARCHAR);

-- Validar eliminación
PRINT '*** Validación después de la eliminación ***';
EXEC sp_Aliado_Especialidad_ObtenerTodos;

-- =============================================
-- VALIDACIÓN FINAL
-- =============================================
PRINT '*** Validación final: Ver registros existentes en ALIADO_ESPECIALIDAD ***';
EXEC sp_Aliado_Especialidad_ObtenerTodos;
