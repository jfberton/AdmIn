-- Tabla Administradores
CREATE TABLE ADMINISTRADOR (
    ADM_ID INT IDENTITY(1,1) PRIMARY KEY,
    PER_ID INT NOT NULL FOREIGN KEY REFERENCES PERSONA(PER_ID),
    ADM_SUPERIOR_ID INT FOREIGN KEY REFERENCES ADMINISTRADOR(ADM_ID) NULL
);

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Inserta un nuevo registro en la tabla ADMINISTRADOR.
-- Entradas: @PersonaId INT, @SuperiorId INT (opcional).
-- Salidas: ID del registro insertado.
-- ====================================================================================
CREATE PROCEDURE sp_Administrador_Insertar
    @PersonaId INT,
    @SuperiorId INT = NULL,
	@NuevoId INT OUTPUT 
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO ADMINISTRADOR (PER_ID, ADM_SUPERIOR_ID)
    VALUES (@PersonaId, @SuperiorId);

    SET @NuevoId = SCOPE_IDENTITY();
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve todos los registros de la tabla ADMINISTRADOR.
-- Entradas: Ninguna.
-- Salidas: Todos los registros de ADMINISTRADOR.
-- ====================================================================================
CREATE PROCEDURE sp_Administrador_ObtenerTodos
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM ADMINISTRADOR;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve un registro específico de ADMINISTRADOR basado en su ID.
-- Entradas: @Id INT - ID del administrador.
-- Salidas: Registro correspondiente al ID.
-- ====================================================================================
CREATE PROCEDURE sp_Administrador_ObtenerPorId
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM ADMINISTRADOR WHERE ADM_ID = @Id;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Actualiza un registro en ADMINISTRADOR basado en su ID.
-- Entradas: @Id INT, @PersonaId INT, @SuperiorId INT (opcional).
-- Salidas: ID del registro actualizado.
-- ====================================================================================
CREATE PROCEDURE sp_Administrador_Actualizar
    @Id INT,
    @PersonaId INT,
    @SuperiorId INT = NULL
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE ADMINISTRADOR
    SET PER_ID = @PersonaId,
        ADM_SUPERIOR_ID = @SuperiorId
    WHERE ADM_ID = @Id;

    SELECT @Id AS ID;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Elimina un registro de ADMINISTRADOR basado en su ID.
-- Entradas: @Id INT - ID del administrador.
-- Salidas: Ninguna.
-- ====================================================================================
CREATE PROCEDURE sp_Administrador_Eliminar
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	DELETE FROM ADMINISTRADOR WHERE ADM_ID = @Id;
END;
GO

-- ====================================================================================
-- Script de Pruebas para ADMINISTRADOR considerando la creación de PERSONA
-- ====================================================================================

-- =============================================
-- PRIMERA PARTE: CREACIÓN DE DATOS DEPENDIENTES
-- =============================================
PRINT '*** Creación de registros en la tabla PERSONA usando el procedimiento almacenado ***';
DECLARE @PersonaId1 INT, @PersonaId2 INT, @PersonaId3 INT;

-- Insertar registros en PERSONA usando el store procedure
EXEC sp_Persona_Insertar 
    @RFC = 'RFC001',
    @Nombre = 'Juan', 
    @ApellidoPaterno = 'Perez', 
    @ApellidoMaterno = 'Lopez',
    @Email = 'juan.perez@example.com',
    @Nacionalidad = 'Argentina',
    @EsPersona = 1,
    @EsTitular = 1,
    @NuevoId = @PersonaId1 OUTPUT;
PRINT 'ID de Persona 1: ' + CAST(@PersonaId1 AS NVARCHAR);

EXEC sp_Persona_Insertar 
    @RFC = 'RFC002',
    @Nombre = 'Maria', 
    @ApellidoPaterno = 'Gomez', 
    @ApellidoMaterno = 'Diaz',
    @Email = 'maria.gomez@example.com',
    @Nacionalidad = 'Argentina',
    @EsPersona = 1,
    @EsTitular = 0,
    @NuevoId = @PersonaId2 OUTPUT;
PRINT 'ID de Persona 2: ' + CAST(@PersonaId2 AS NVARCHAR);

EXEC sp_Persona_Insertar 
    @RFC = 'RFC003',
    @Nombre = 'Carlos', 
    @ApellidoPaterno = 'Fernandez', 
    @ApellidoMaterno = 'Sosa',
    @Email = 'carlos.fernandez@example.com',
    @Nacionalidad = 'Argentina',
    @EsPersona = 1,
    @EsTitular = 1,
    @NuevoId = @PersonaId3 OUTPUT;
PRINT 'ID de Persona 3: ' + CAST(@PersonaId3 AS NVARCHAR);

-- =============================================
-- PRUEBAS SOBRE ADMINISTRADOR
-- =============================================

PRINT '*** Validación inicial de los registros existentes en ADMINISTRADOR ***';
EXEC sp_Administrador_ObtenerTodos;

-- Prueba del procedimiento de inserción
DECLARE @NuevoId1 INT, @NuevoId2 INT, @NuevoId3 INT;

-- Insertar Administrador sin superior
EXEC sp_Administrador_Insertar 
    @PersonaId = @PersonaId1, 
    @SuperiorId = NULL, 
    @NuevoId = @NuevoId1 OUTPUT;
PRINT 'Nuevo Administrador ID (sin superior): ' + CAST(@NuevoId1 AS NVARCHAR);

-- Insertar Administrador con superior
EXEC sp_Administrador_Insertar 
    @PersonaId = @PersonaId2, 
    @SuperiorId = @NuevoId1, 
    @NuevoId = @NuevoId2 OUTPUT;
PRINT 'Nuevo Administrador ID (con superior): ' + CAST(@NuevoId2 AS NVARCHAR);

EXEC sp_Administrador_Insertar 
    @PersonaId = @PersonaId3, 
    @SuperiorId = @NuevoId1, 
    @NuevoId = @NuevoId3 OUTPUT;
PRINT 'Nuevo Administrador ID (con superior): ' + CAST(@NuevoId3 AS NVARCHAR);

-- Validación después de las inserciones
PRINT '*** Registros actuales en ADMINISTRADOR después de inserciones ***';
EXEC sp_Administrador_ObtenerTodos;

-- Prueba de obtención por ID
DECLARE @TestId INT = @NuevoId1; -- Usar un ID generado
PRINT 'Prueba: Obtener registro de ADMINISTRADOR por ID';
EXEC sp_Administrador_ObtenerPorId @Id = @TestId;

-- Prueba de actualización de un registro
DECLARE @ActualizarId INT = @NuevoId2; -- ID existente
EXEC sp_Administrador_Actualizar 
    @Id = @ActualizarId, 
    @PersonaId = @PersonaId3, 
    @SuperiorId = @NuevoId1;
PRINT 'Actualización del registro completada';

-- Validación después de la actualización
PRINT '*** Registros después de la actualización en ADMINISTRADOR ***';
EXEC sp_Administrador_ObtenerTodos;

-- Prueba de eliminación de un registro
DECLARE @EliminarId INT = @NuevoId3; -- ID existente
EXEC sp_Administrador_Eliminar @Id = @EliminarId;
PRINT 'Eliminación del registro completada';

-- Validación después de la eliminación
PRINT '*** Registros en ADMINISTRADOR después de la eliminación ***';
EXEC sp_Administrador_ObtenerTodos;
