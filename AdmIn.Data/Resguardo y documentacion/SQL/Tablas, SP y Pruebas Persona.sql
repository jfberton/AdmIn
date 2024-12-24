-- Tabla Persona
CREATE TABLE PERSONA (
    PER_ID INT IDENTITY(1,1) PRIMARY KEY,
    PER_RFC NVARCHAR(20) NOT NULL,
    PER_NOMBRE NVARCHAR(100) NOT NULL,
    PER_APATERNO NVARCHAR(100),
    PER_AMATERNO NVARCHAR(100),
    PER_EMAIL NVARCHAR(100),
    PER_NACIONALIDAD NVARCHAR(50),
    PER_ESPERSONA BIT NOT NULL,
    PER_TITULAR BIT
);


-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Inserta un nuevo registro en la tabla PERSONA.
-- Entradas: @RFC NVARCHAR(20), @Nombre NVARCHAR(100), @ApellidoPaterno NVARCHAR(100), 
--           @ApellidoMaterno NVARCHAR(100), @Email NVARCHAR(100), 
--           @Nacionalidad NVARCHAR(50), @EsPersona BIT, @EsTitular BIT.
-- Salidas: ID del registro insertado.
-- ====================================================================================
CREATE PROCEDURE sp_Persona_Insertar
    @RFC NVARCHAR(20),
    @Nombre NVARCHAR(100),
    @ApellidoPaterno NVARCHAR(100),
    @ApellidoMaterno NVARCHAR(100),
    @Email NVARCHAR(100),
    @Nacionalidad NVARCHAR(50),
    @EsPersona BIT,
    @EsTitular BIT,
    @NuevoId INT OUTPUT 
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO PERSONA (
        PER_RFC, PER_NOMBRE, PER_APATERNO, PER_AMATERNO, 
        PER_EMAIL, PER_NACIONALIDAD, PER_ESPERSONA, PER_TITULAR
    )
    VALUES (
        @RFC, @Nombre, @ApellidoPaterno, @ApellidoMaterno, 
        @Email, @Nacionalidad, @EsPersona, @EsTitular
    );

    SET @NuevoId = SCOPE_IDENTITY();
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve todos los registros de la tabla PERSONA.
-- Entradas: Ninguna.
-- Salidas: Todos los registros de PERSONA.
-- ====================================================================================
CREATE PROCEDURE sp_Persona_ObtenerTodos
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM PERSONA;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve un registro específico de PERSONA basado en su ID.
-- Entradas: @Id INT - ID de la persona.
-- Salidas: Registro correspondiente al ID.
-- ====================================================================================
CREATE PROCEDURE sp_Persona_ObtenerPorId
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM PERSONA WHERE PER_ID = @Id;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Actualiza un registro en PERSONA basado en su ID.
-- Entradas: @Id INT, @RFC NVARCHAR(20), @Nombre NVARCHAR(100), 
--           @ApellidoPaterno NVARCHAR(100), @ApellidoMaterno NVARCHAR(100), 
--           @Email NVARCHAR(100), @Nacionalidad NVARCHAR(50), 
--           @EsPersona BIT, @EsTitular BIT.
-- Salidas: ID del registro actualizado.
-- ====================================================================================
CREATE PROCEDURE sp_Persona_Actualizar
    @Id INT,
    @RFC NVARCHAR(20),
    @Nombre NVARCHAR(100),
    @ApellidoPaterno NVARCHAR(100),
    @ApellidoMaterno NVARCHAR(100),
    @Email NVARCHAR(100),
    @Nacionalidad NVARCHAR(50),
    @EsPersona BIT,
    @EsTitular BIT
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE PERSONA
    SET PER_RFC = @RFC,
        PER_NOMBRE = @Nombre,
        PER_APATERNO = @ApellidoPaterno,
        PER_AMATERNO = @ApellidoMaterno,
        PER_EMAIL = @Email,
        PER_NACIONALIDAD = @Nacionalidad,
        PER_ESPERSONA = @EsPersona,
        PER_TITULAR = @EsTitular
    WHERE PER_ID = @Id;

    SELECT @Id AS ID;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Elimina un registro de PERSONA basado en su ID.
-- Entradas: @Id INT - ID de la persona.
-- Salidas: Ninguna.
-- ====================================================================================
CREATE PROCEDURE sp_Persona_Eliminar
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	DELETE FROM PERSONA WHERE PER_ID = @Id;
END;
GO


-- ====================================================================================
-- Script de Pruebas para PERSONA y sus Procedimientos Almacenados
-- ====================================================================================

-- Validación inicial de los registros existentes
PRINT 'Registros iniciales en PERSONA:';
EXEC sp_Persona_ObtenerTodos;
GO

-- Prueba del procedimiento de inserción
DECLARE @NuevoId INT;

EXEC sp_Persona_Insertar 
    @RFC = 'RFC123456789', 
    @Nombre = 'Juan', 
    @ApellidoPaterno = 'Perez', 
    @ApellidoMaterno = 'Lopez', 
    @Email = 'juan.perez@mail.com', 
    @Nacionalidad = 'Mexicana', 
    @EsPersona = 1, 
    @EsTitular = 1, 
    @NuevoId = @NuevoId OUTPUT;
PRINT 'Nuevo ID insertado: ' + CAST(@NuevoId AS NVARCHAR);

EXEC sp_Persona_Insertar 
    @RFC = 'RFC987654321', 
    @Nombre = 'Maria', 
    @ApellidoPaterno = 'Gomez', 
    @ApellidoMaterno = 'Sanchez', 
    @Email = 'maria.gomez@mail.com', 
    @Nacionalidad = 'Argentina', 
    @EsPersona = 1, 
    @EsTitular = 0, 
    @NuevoId = @NuevoId OUTPUT;
PRINT 'Nuevo ID insertado: ' + CAST(@NuevoId AS NVARCHAR);
GO

-- Validación de todos los registros después de las inserciones
PRINT 'Registros actuales en PERSONA después de inserciones:';
EXEC sp_Persona_ObtenerTodos;
GO

-- Prueba de obtención por ID
DECLARE @TestId INT = 1; -- Ajustar según los IDs generados
PRINT 'Prueba: Obtener registro por ID';
EXEC sp_Persona_ObtenerPorId @Id = @TestId;
GO

-- Prueba de actualización de un registro
DECLARE @ActualizarId INT = 2; -- Ajustar según ID existente
EXEC sp_Persona_Actualizar 
    @Id = @ActualizarId, 
    @RFC = 'RFC555555555', 
    @Nombre = 'Carlos', 
    @ApellidoPaterno = 'Ramirez', 
    @ApellidoMaterno = 'Martinez', 
    @Email = 'carlos.ramirez@mail.com', 
    @Nacionalidad = 'Chilena', 
    @EsPersona = 1, 
    @EsTitular = 0;
PRINT 'Actualización de registro correcta';
GO

-- Validación después de la actualización
PRINT 'Registros después de la actualización:';
EXEC sp_Persona_ObtenerTodos;
GO

-- Prueba de eliminación de un registro
DECLARE @EliminarId INT = 2; -- Ajustar según ID existente
EXEC sp_Persona_Eliminar @Id = @EliminarId;
PRINT 'Registro eliminado correctamente';

-- Validación posterior a la eliminación
PRINT 'Registros después de la eliminación:';
EXEC sp_Persona_ObtenerTodos;
GO
