-- Tabla Aliados
CREATE TABLE ALIADO (
    ALI_ID INT IDENTITY(1,1) PRIMARY KEY,
    ADM_ID INT NOT NULL FOREIGN KEY REFERENCES ADMINISTRADOR(ADM_ID),
    PER_ID INT NOT NULL FOREIGN KEY REFERENCES PERSONA(PER_ID),
	ALI_ESP_ID INT NOT NULL FOREIGN KEY REFERENCES ALIADO_ESPECIALIDAD(ALI_ESP_ID)
);

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Inserta un nuevo registro en la tabla ALIADO.
-- Entradas: @AdminId INT, @PersonaId INT, @EspecialidadId INT.
-- Salidas: ID del registro insertado.
-- ====================================================================================
CREATE PROCEDURE sp_Aliado_Insertar
    @AdminId INT,
    @PersonaId INT,
    @EspecialidadId INT,
	@NuevoId INT OUTPUT 
	
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO ALIADO (ADM_ID, PER_ID, ALI_ESP_ID)
    VALUES (@AdminId, @PersonaId, @EspecialidadId);

    SET @NuevoId = SCOPE_IDENTITY();
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve todos los registros de la tabla ALIADO.
-- Entradas: Ninguna.
-- Salidas: Todos los registros de ALIADO.
-- ====================================================================================
CREATE PROCEDURE sp_Aliado_ObtenerTodos
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM ALIADO;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve un registro específico de ALIADO basado en su ID.
-- Entradas: @Id INT - ID del aliado.
-- Salidas: Registro correspondiente al ID.
-- ====================================================================================
CREATE PROCEDURE sp_Aliado_ObtenerPorId
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM ALIADO WHERE ALI_ID = @Id;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Actualiza un registro en ALIADO basado en su ID.
-- Entradas: @Id INT, @AdminId INT, @PersonaId INT, @EspecialidadId INT.
-- Salidas: ID del registro actualizado.
-- ====================================================================================
CREATE PROCEDURE sp_Aliado_Actualizar
    @Id INT,
    @AdminId INT,
    @PersonaId INT,
    @EspecialidadId INT
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE ALIADO
    SET ADM_ID = @AdminId,
        PER_ID = @PersonaId,
        ALI_ESP_ID = @EspecialidadId
    WHERE ALI_ID = @Id;

    SELECT @Id AS ID;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Elimina un registro de ALIADO basado en su ID.
-- Entradas: @Id INT - ID del aliado.
-- Salidas: Ninguna.
-- ====================================================================================
CREATE PROCEDURE sp_Aliado_Eliminar
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	DELETE FROM ALIADO WHERE ALI_ID = @Id;
END;
GO

-- ====================================================================================
-- Script de Pruebas para ALIADO
-- ====================================================================================

-- =============================================
-- PRIMERA PARTE: CREACIÓN DE REGISTROS BASE
-- =============================================
PRINT '*** Creación de registros base: PERSONA ***';
DECLARE @PersonaId1 INT, @PersonaId2 INT;

-- Insertar registros en PERSONA
EXEC sp_Persona_Insertar 
    @RFC = 'RFC123456',
    @Nombre = 'Juan',
    @ApellidoPaterno = 'Perez',
    @ApellidoMaterno = 'Gomez',
    @Email = 'juan.perez@example.com',
    @Nacionalidad = 'Argentina',
    @EsPersona = 1,
    @EsTitular = 0,
    @NuevoId = @PersonaId1 OUTPUT;
PRINT 'ID de Persona 1: ' + CAST(@PersonaId1 AS NVARCHAR);

EXEC sp_Persona_Insertar 
    @RFC = 'RFC654321',
    @Nombre = 'Ana',
    @ApellidoPaterno = 'Lopez',
    @ApellidoMaterno = 'Diaz',
    @Email = 'ana.lopez@example.com',
    @Nacionalidad = 'México',
    @EsPersona = 1,
    @EsTitular = 1,
    @NuevoId = @PersonaId2 OUTPUT;
PRINT 'ID de Persona 2: ' + CAST(@PersonaId2 AS NVARCHAR);

-- =============================================
PRINT '*** Creación de registros base: ALIADO_ESPECIALIDAD ***';
DECLARE @EspecialidadId1 INT, @EspecialidadId2 INT;

-- Insertar registros en ALIADO_ESPECIALIDAD
EXEC sp_Aliado_Especialidad_Insertar 
    @Descripcion = 'Médico General',
    @NuevoId = @EspecialidadId1 OUTPUT;
PRINT 'ID de Especialidad 1: ' + CAST(@EspecialidadId1 AS NVARCHAR);

EXEC sp_Aliado_Especialidad_Insertar 
    @Descripcion = 'Abogado Civil',
    @NuevoId = @EspecialidadId2 OUTPUT;
PRINT 'ID de Especialidad 2: ' + CAST(@EspecialidadId2 AS NVARCHAR);

-- =============================================
PRINT '*** Creación de registros base: ADMINISTRADOR ***';
DECLARE @AdminId1 INT, @AdminId2 INT;

-- Insertar registros en ADMINISTRADOR
EXEC sp_Administrador_Insertar 
    @PersonaId = @PersonaId1,
    @NuevoId = @AdminId1 OUTPUT;
PRINT 'ID de Administrador 1: ' + CAST(@AdminId1 AS NVARCHAR);

EXEC sp_Administrador_Insertar 
    @PersonaId = @PersonaId2,
    @NuevoId = @AdminId2 OUTPUT;
PRINT 'ID de Administrador 2: ' + CAST(@AdminId2 AS NVARCHAR);

-- =============================================
-- SEGUNDA PARTE: PRUEBA DE INSERCIÓN EN ALIADO
-- =============================================
PRINT '*** Creación de registros en la tabla ALIADO ***';
DECLARE @AliadoId1 INT, @AliadoId2 INT;

-- Insertar registros en ALIADO
EXEC sp_Aliado_Insertar 
    @AdminId = @AdminId1,
    @PersonaId = @PersonaId1,
    @EspecialidadId = @EspecialidadId1,
    @NuevoId = @AliadoId1 OUTPUT;
PRINT 'ID de Aliado 1: ' + CAST(@AliadoId1 AS NVARCHAR);

EXEC sp_Aliado_Insertar 
    @AdminId = @AdminId2,
    @PersonaId = @PersonaId2,
    @EspecialidadId = @EspecialidadId2,
    @NuevoId = @AliadoId2 OUTPUT;
PRINT 'ID de Aliado 2: ' + CAST(@AliadoId2 AS NVARCHAR);

-- =============================================
-- VALIDACIÓN INICIAL DE REGISTROS
-- =============================================
PRINT '*** Validación inicial: Mostrar todos los registros existentes en ALIADO ***';
EXEC sp_Aliado_ObtenerTodos;

-- =============================================
-- PRUEBA DE OBTENCIÓN POR ID
-- =============================================
PRINT '*** Prueba: Obtener registro de ALIADO por ID ***';
EXEC sp_Aliado_ObtenerPorId @Id = @AliadoId1;

-- =============================================
-- PRUEBA DE ACTUALIZACIÓN
-- =============================================
PRINT '*** Prueba: Actualizar un registro en ALIADO ***';
DECLARE @ActualizarId INT = @AliadoId2;

EXEC sp_Aliado_Actualizar 
    @Id = @ActualizarId,
    @AdminId = @AdminId1,
    @PersonaId = @PersonaId2,
    @EspecialidadId = @EspecialidadId1;
PRINT 'Registro actualizado con ID: ' + CAST(@ActualizarId AS NVARCHAR);

-- Validar actualización
PRINT '*** Validación después de la actualización ***';
EXEC sp_Aliado_ObtenerPorId @Id = @ActualizarId;

-- =============================================
-- PRUEBA DE ELIMINACIÓN
-- =============================================
PRINT '*** Prueba: Eliminar un registro en ALIADO ***';
DECLARE @EliminarId INT = @AliadoId1;

EXEC sp_Aliado_Eliminar @Id = @EliminarId;
PRINT 'Registro eliminado con ID: ' + CAST(@EliminarId AS NVARCHAR);

-- Validar eliminación
PRINT '*** Validación después de la eliminación ***';
EXEC sp_Aliado_ObtenerTodos;

-- =============================================
-- VALIDACIÓN FINAL
-- =============================================
PRINT '*** Validación final: Mostrar todos los registros existentes en ALIADO ***';
EXEC sp_Aliado_ObtenerTodos;
