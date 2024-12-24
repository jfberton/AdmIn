-- Tabla Clientes
CREATE TABLE CLIENTE (
    CLI_ID INT IDENTITY(1,1) PRIMARY KEY,
    PER_ID INT NOT NULL FOREIGN KEY REFERENCES PERSONA(PER_ID),
    ADM_ID INT NOT NULL FOREIGN KEY REFERENCES ADMINISTRADOR(ADM_ID)
);

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Inserta un nuevo registro en la tabla CLIENTE.
-- Entradas: @PersonaId INT.
-- Salidas: ID del registro insertado.
-- ====================================================================================
CREATE PROCEDURE sp_Cliente_Insertar
    @PersonaId INT,
	@AdminId INT, 
    @NuevoId INT OUTPUT 
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO CLIENTE (PER_ID, ADM_ID)
    VALUES (@PersonaId, @AdminId);

    SET @NuevoId = SCOPE_IDENTITY();
END;

GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve todos los registros de la tabla CLIENTE.
-- Entradas: Ninguna.
-- Salidas: Todos los registros de CLIENTE.
-- ====================================================================================
CREATE PROCEDURE sp_Cliente_ObtenerTodos
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM CLIENTE;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve un registro específico de CLIENTE basado en su ID.
-- Entradas: @Id INT - ID del cliente.
-- Salidas: Registro correspondiente al ID.
-- ====================================================================================
CREATE PROCEDURE sp_Cliente_ObtenerPorId
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM CLIENTE WHERE CLI_ID = @Id;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Actualiza un registro en CLIENTE basado en su ID.
-- Entradas: @Id INT, @PersonaId INT.
-- Salidas: ID del registro actualizado.
-- ====================================================================================
CREATE PROCEDURE sp_Cliente_Actualizar
    @Id INT,
    @PersonaId INT
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE CLIENTE
    SET PER_ID = @PersonaId
    WHERE CLI_ID = @Id;

    SELECT @Id AS ID;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Elimina un registro de CLIENTE basado en su ID.
-- Entradas: @Id INT - ID del cliente.
-- Salidas: Ninguna.
-- ====================================================================================
CREATE PROCEDURE sp_Cliente_Eliminar
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	DELETE FROM CLIENTE WHERE CLI_ID = @Id;
END;
GO

-- ====================================================================================
-- Script de Pruebas para CLIENTE
-- ====================================================================================

-- =============================================
-- PRIMERA PARTE: CREACIÓN DE REGISTROS BASE
-- =============================================
PRINT '*** Creación de registros base para CLIENTE ***';
DECLARE @PersonaId INT, @AdminId INT, @ClienteId INT;

-- Insertar registros en PERSONA
PRINT 'Creación de PERSONA';
EXEC sp_Persona_Insertar 
    @RFC = 'RFC222222',
    @Nombre = 'Juan',
    @ApellidoPaterno = 'Perez',
    @ApellidoMaterno = 'Lopez',
    @Email = 'juan.perez@example.com',
    @Nacionalidad = 'Argentina',
    @EsPersona = 1,
    @EsTitular = 0,
    @NuevoId = @PersonaId OUTPUT;
PRINT 'ID de Persona: ' + CAST(@PersonaId AS NVARCHAR);

-- Insertar registros en ADMINISTRADOR
PRINT 'Creación de ADMINISTRADOR';
EXEC sp_Administrador_Insertar 
    @PersonaId = @PersonaId,
    @NuevoId = @AdminId OUTPUT;
PRINT 'ID de Administrador: ' + CAST(@AdminId AS NVARCHAR);

-- =============================================
-- SEGUNDA PARTE: PRUEBA DE INSERCIÓN EN CLIENTE
-- =============================================
PRINT '*** Creación de registros en la tabla CLIENTE ***';

-- Insertar registros en CLIENTE
EXEC sp_Cliente_Insertar 
    @PersonaId = @PersonaId,
    @AdminId = @AdminId, -- Proporcionamos el ID del administrador creado previamente
    @NuevoId = @ClienteId OUTPUT;
PRINT 'ID del Cliente: ' + CAST(@ClienteId AS NVARCHAR);

-- =============================================
-- VALIDACIÓN INICIAL DE REGISTROS
-- =============================================
PRINT '*** Validación inicial: Mostrar todos los registros existentes en CLIENTE ***';
EXEC sp_Cliente_ObtenerTodos;

-- =============================================
-- PRUEBA DE OBTENCIÓN POR ID
-- =============================================
PRINT '*** Prueba: Obtener registro de CLIENTE por ID ***';
EXEC sp_Cliente_ObtenerPorId @Id = @ClienteId;

-- =============================================
-- PRUEBA DE ACTUALIZACIÓN
-- =============================================
PRINT '*** Prueba: Actualizar un registro en CLIENTE ***';

-- Insertar nueva PERSONA para actualizar
DECLARE @NuevaPersonaId INT;
EXEC sp_Persona_Insertar 
    @RFC = 'RFC333333',
    @Nombre = 'Maria',
    @ApellidoPaterno = 'Gonzalez',
    @ApellidoMaterno = 'Sanchez',
    @Email = 'maria.gonzalez@example.com',
    @Nacionalidad = 'Uruguay',
    @EsPersona = 1,
    @EsTitular = 0,
    @NuevoId = @NuevaPersonaId OUTPUT;
PRINT 'ID de Nueva Persona: ' + CAST(@NuevaPersonaId AS NVARCHAR);

-- Actualizar CLIENTE
EXEC sp_Cliente_Actualizar 
    @Id = @ClienteId,
    @PersonaId = @NuevaPersonaId;
PRINT 'Registro actualizado con ID: ' + CAST(@ClienteId AS NVARCHAR);

-- Validar actualización
PRINT '*** Validación después de la actualización ***';
EXEC sp_Cliente_ObtenerPorId @Id = @ClienteId;

-- =============================================
-- PRUEBA DE ELIMINACIÓN
-- =============================================
PRINT '*** Prueba: Eliminar un registro en CLIENTE ***';

DECLARE @EliminarId INT = @ClienteId;

EXEC sp_Cliente_Eliminar @Id = @EliminarId;
PRINT 'Registro eliminado con ID: ' + CAST(@EliminarId AS NVARCHAR);

-- Validar eliminación
PRINT '*** Validación después de la eliminación ***';
EXEC sp_Cliente_ObtenerTodos;

-- =============================================
-- VALIDACIÓN FINAL
-- =============================================
PRINT '*** Validación final: Mostrar todos los registros existentes en CLIENTE ***';
EXEC sp_Cliente_ObtenerTodos;
