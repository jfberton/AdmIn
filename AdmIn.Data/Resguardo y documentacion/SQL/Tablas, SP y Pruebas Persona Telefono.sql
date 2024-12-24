-- Tabla Persona telefono
CREATE TABLE PERSONA_TELEFONO (
    PER_TEL_ID INT IDENTITY(1,1) PRIMARY KEY,
    PER_ID INT NOT NULL FOREIGN KEY REFERENCES PERSONA(PER_ID),
    TEL_ID INT NOT NULL FOREIGN KEY REFERENCES TELEFONO(TEL_ID)
);


-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Inserta un nuevo registro en la tabla PERSONA_TELEFONO.
-- Entradas: @PersonaId INT, @TelefonoId INT.
-- Salidas: ID del registro insertado.
-- ====================================================================================
CREATE PROCEDURE sp_Persona_Telefono_Insertar
    @PersonaId INT,
    @TelefonoId INT,
    @NuevoId INT OUTPUT 
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO PERSONA_TELEFONO (PER_ID, TEL_ID)
    VALUES (@PersonaId, @TelefonoId);

    SET @NuevoId = SCOPE_IDENTITY();
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve todos los registros de la tabla PERSONA_TELEFONO.
-- Entradas: Ninguna.
-- Salidas: Todos los registros de PERSONA_TELEFONO.
-- ====================================================================================
CREATE PROCEDURE sp_Persona_Telefono_ObtenerTodos
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM PERSONA_TELEFONO;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve un registro específico de PERSONA_TELEFONO basado en su ID.
-- Entradas: @Id INT - ID del registro persona-teléfono.
-- Salidas: Registro correspondiente al ID.
-- ====================================================================================
CREATE PROCEDURE sp_Persona_Telefono_ObtenerPorId
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM PERSONA_TELEFONO WHERE PER_TEL_ID = @Id;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Actualiza un registro en PERSONA_TELEFONO basado en su ID.
-- Entradas: @Id INT, @PersonaId INT, @TelefonoId INT.
-- Salidas: ID del registro actualizado.
-- ====================================================================================
CREATE PROCEDURE sp_Persona_Telefono_Actualizar
    @Id INT,
    @PersonaId INT,
    @TelefonoId INT
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE PERSONA_TELEFONO
    SET PER_ID = @PersonaId,
        TEL_ID = @TelefonoId
    WHERE PER_TEL_ID = @Id;

    SELECT @Id AS ID;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Elimina un registro de PERSONA_TELEFONO basado en su ID.
-- Entradas: @Id INT - ID del registro persona-teléfono.
-- Salidas: Ninguna.
-- ====================================================================================
CREATE PROCEDURE sp_Persona_Telefono_Eliminar
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	DELETE FROM PERSONA_TELEFONO WHERE PER_TEL_ID = @Id;
END;
GO

-- ====================================================================================
-- Script de Pruebas para PERSONA_TELEFONO y sus Procedimientos Almacenados
-- ====================================================================================

-- Validación inicial de los registros existentes
PRINT 'Registros iniciales en PERSONA_TELEFONO:';
EXEC sp_Persona_Telefono_ObtenerTodos;
GO

-- Prueba del procedimiento de inserción
DECLARE @NuevoId INT;

EXEC sp_Persona_Telefono_Insertar 
    @PersonaId = 1, -- Ajustar con un ID existente en PERSONA
    @TelefonoId = 1, -- Ajustar con un ID existente en TELEFONO
    @NuevoId = @NuevoId OUTPUT;
PRINT 'Nuevo ID insertado: ' + CAST(@NuevoId AS NVARCHAR);

EXEC sp_Persona_Telefono_Insertar 
    @PersonaId = 1, -- Ajustar con un ID existente en PERSONA
    @TelefonoId = 2, -- Ajustar con un ID existente en TELEFONO
    @NuevoId = @NuevoId OUTPUT;
PRINT 'Nuevo ID insertado: ' + CAST(@NuevoId AS NVARCHAR);
GO

-- Validación de todos los registros después de las inserciones
PRINT 'Registros actuales en PERSONA_TELEFONO después de inserciones:';
EXEC sp_Persona_Telefono_ObtenerTodos;
GO

-- Prueba de obtención por ID
DECLARE @TestId INT = 1; -- Ajustar según los IDs generados
PRINT 'Prueba: Obtener registro por ID';
EXEC sp_Persona_Telefono_ObtenerPorId @Id = @TestId;
GO

-- Prueba de actualización de un registro
DECLARE @ActualizarId INT = 1; -- Ajustar según ID existente
EXEC sp_Persona_Telefono_Actualizar 
    @Id = @ActualizarId, 
    @PersonaId = 1, -- Ajustar con un ID existente en PERSONA
    @TelefonoId = 3; -- Ajustar con un ID existente en TELEFONO
PRINT 'Actualización de registro correcta';
GO

-- Validación después de la actualización
PRINT 'Registros después de la actualización:';
EXEC sp_Persona_Telefono_ObtenerTodos;
GO

-- Prueba de eliminación de un registro
DECLARE @EliminarId INT = 1; -- Ajustar según ID existente
EXEC sp_Persona_Telefono_Eliminar @Id = @EliminarId;
PRINT 'Registro eliminado correctamente';

-- Validación posterior a la eliminación
PRINT 'Registros después de la eliminación:';
EXEC sp_Persona_Telefono_ObtenerTodos;
GO
