-- Tabla Relación Persona-Dirección
CREATE TABLE PERSONA_DIRECCION (
    PER_DIR_ID INT IDENTITY(1,1) PRIMARY KEY,
    PER_ID INT NOT NULL FOREIGN KEY REFERENCES PERSONA(PER_ID),
    DIR_ID INT NOT NULL FOREIGN KEY REFERENCES DIRECCION(DIR_ID),
    DIR_TIP_ID INT NOT NULL FOREIGN KEY REFERENCES DIRECCION_TIPO(DIR_TIP_ID)
);

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Inserta un nuevo registro en la tabla PERSONA_DIRECCION.
-- Entradas: @PersonaId INT, @DireccionId INT, @TipoId INT.
-- Salidas: ID del registro insertado.
-- ====================================================================================
CREATE PROCEDURE sp_Persona_Direccion_Insertar
    @PersonaId INT,
    @DireccionId INT,
    @TipoId INT,
	@NuevoId INT OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO PERSONA_DIRECCION (PER_ID, DIR_ID, DIR_TIP_ID)
    VALUES (@PersonaId, @DireccionId, @TipoId);

    SET @NuevoId = SCOPE_IDENTITY();
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve todos los registros de la tabla PERSONA_DIRECCION.
-- Entradas: Ninguna.
-- Salidas: Todos los registros de PERSONA_DIRECCION.
-- ====================================================================================
CREATE PROCEDURE sp_Persona_Direccion_ObtenerTodos
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM PERSONA_DIRECCION;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve un registro específico de PERSONA_DIRECCION basado en su ID.
-- Entradas: @Id INT - ID de la relación persona-dirección.
-- Salidas: Registro correspondiente al ID.
-- ====================================================================================
CREATE PROCEDURE sp_Persona_Direccion_ObtenerPorId
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM PERSONA_DIRECCION WHERE PER_DIR_ID = @Id;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Actualiza un registro en PERSONA_DIRECCION basado en su ID.
-- Entradas: @Id INT, @PersonaId INT, @DireccionId INT, @TipoId INT.
-- Salidas: ID del registro actualizado.
-- ====================================================================================
CREATE PROCEDURE sp_Persona_Direccion_Actualizar
    @Id INT,
    @PersonaId INT,
    @DireccionId INT,
    @TipoId INT
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE PERSONA_DIRECCION
    SET PER_ID = @PersonaId,
        DIR_ID = @DireccionId,
        DIR_TIP_ID = @TipoId
    WHERE PER_DIR_ID = @Id;

    SELECT @Id AS ID;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Elimina un registro de PERSONA_DIRECCION basado en su ID.
-- Entradas: @Id INT - ID de la relación persona-dirección.
-- Salidas: Ninguna.
-- ====================================================================================
CREATE PROCEDURE sp_Persona_Direccion_Eliminar
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	DELETE FROM PERSONA_DIRECCION WHERE PER_DIR_ID = @Id;
END;
GO

-- ====================================================================================
-- Script de Pruebas para PERSONA_DIRECCION y sus Procedimientos Almacenados
-- ====================================================================================

-- Validación inicial de los registros existentes
PRINT 'Registros iniciales en PERSONA_DIRECCION:';
EXEC sp_Persona_Direccion_ObtenerTodos;
GO

-- Prueba del procedimiento de inserción
DECLARE @NuevoId INT;

-- Suponiendo IDs preexistentes para Persona, Dirección y Tipo de Dirección
EXEC sp_Persona_Direccion_Insertar 
    @PersonaId = 1, 
    @DireccionId = 1, 
    @TipoId = 1, 
    @NuevoId = @NuevoId OUTPUT;
PRINT 'Nuevo ID insertado: ' + CAST(@NuevoId AS NVARCHAR);

EXEC sp_Persona_Direccion_Insertar 
    @PersonaId = 2, 
    @DireccionId = 2, 
    @TipoId = 2, 
    @NuevoId = @NuevoId OUTPUT;
PRINT 'Nuevo ID insertado: ' + CAST(@NuevoId AS NVARCHAR);
GO

-- Validación de todos los registros después de las inserciones
PRINT 'Registros actuales en PERSONA_DIRECCION después de inserciones:';
EXEC sp_Persona_Direccion_ObtenerTodos;
GO

-- Prueba de obtención por ID
DECLARE @TestId INT = 1; -- Ajustar según los IDs generados
PRINT 'Prueba: Obtener registro por ID';
EXEC sp_Persona_Direccion_ObtenerPorId @Id = @TestId;
GO

-- Prueba de actualización de un registro
DECLARE @ActualizarId INT = 1; -- Ajustar según ID existente
EXEC sp_Persona_Direccion_Actualizar 
    @Id = @ActualizarId, 
    @PersonaId = 3, 
    @DireccionId = 3, 
    @TipoId = 3;
PRINT 'Actualización de registro correcta';
GO

-- Validación después de la actualización
PRINT 'Registros después de la actualización:';
EXEC sp_Persona_Direccion_ObtenerTodos;
GO

-- Prueba de eliminación de un registro
DECLARE @EliminarId INT = 2; -- Ajustar según ID existente
EXEC sp_Persona_Direccion_Eliminar @Id = @EliminarId;
PRINT 'Registro eliminado correctamente';

-- Validación posterior a la eliminación
PRINT 'Registros después de la eliminación:';
EXEC sp_Persona_Direccion_ObtenerTodos;
GO

