-- Tabla Tipo de direccion persona
CREATE TABLE DIRECCION_TIPO (
    DIR_TIP_ID INT IDENTITY(1,1) PRIMARY KEY,
    DIR_TIP_TIPO NVARCHAR(50)
);

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Inserta un nuevo registro en la tabla DIRECCION_TIPO.
-- Entradas: @Tipo NVARCHAR(50).
-- Salidas: ID del registro insertado.
-- ====================================================================================
CREATE PROCEDURE sp_Direccion_Tipo_Insertar
    @Tipo NVARCHAR(50),
	@NuevoId INT OUTPUT 
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO DIRECCION_TIPO (DIR_TIP_TIPO)
    VALUES (@Tipo);

    SET @NuevoId = SCOPE_IDENTITY();
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve todos los registros de la tabla DIRECCION_TIPO.
-- Entradas: Ninguna.
-- Salidas: Todos los registros de DIRECCION_TIPO.
-- ====================================================================================
CREATE PROCEDURE sp_Direccion_Tipo_ObtenerTodos
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM DIRECCION_TIPO;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve un registro específico de DIRECCION_TIPO basado en su ID.
-- Entradas: @Id INT - ID del tipo de dirección.
-- Salidas: Registro correspondiente al ID.
-- ====================================================================================
CREATE PROCEDURE sp_Direccion_Tipo_ObtenerPorId
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM DIRECCION_TIPO WHERE DIR_TIP_ID = @Id;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Actualiza un registro en DIRECCION_TIPO basado en su ID.
-- Entradas: @Id INT, @Tipo NVARCHAR(50).
-- Salidas: ID del registro actualizado.
-- ====================================================================================
CREATE PROCEDURE sp_Direccion_Tipo_Actualizar
    @Id INT,
    @Tipo NVARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE DIRECCION_TIPO
    SET DIR_TIP_TIPO = @Tipo
    WHERE DIR_TIP_ID = @Id;

    SELECT @Id AS ID;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Elimina un registro de DIRECCION_TIPO basado en su ID.
-- Entradas: @Id INT - ID del tipo de dirección.
-- Salidas: Ninguna.
-- ====================================================================================
CREATE PROCEDURE sp_Direccion_Tipo_Eliminar
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	DELETE FROM DIRECCION_TIPO WHERE DIR_TIP_ID = @Id;
END;
GO

-- ====================================================================================
-- Script de Pruebas para DIRECCION_TIPO y sus Procedimientos Almacenados
-- ====================================================================================

-- Validación inicial de los registros existentes
PRINT 'Registros iniciales en DIRECCION_TIPO:';
EXEC sp_Direccion_Tipo_ObtenerTodos;
GO

-- Prueba del procedimiento de inserción
DECLARE @NuevoId INT;

EXEC sp_Direccion_Tipo_Insertar @Tipo = 'Personal', @NuevoId = @NuevoId OUTPUT;
PRINT 'Nuevo ID insertado: ' + CAST(@NuevoId AS NVARCHAR);

EXEC sp_Direccion_Tipo_Insertar @Tipo = 'Trabajo', @NuevoId = @NuevoId OUTPUT;
PRINT 'Nuevo ID insertado: ' + CAST(@NuevoId AS NVARCHAR);
GO

-- Validación de todos los registros después de las inserciones
PRINT 'Registros actuales en DIRECCION_TIPO después de inserciones:';
EXEC sp_Direccion_Tipo_ObtenerTodos;
GO

-- Prueba de obtención por ID
DECLARE @TestId INT = 1; -- Ajustar según los IDs generados
PRINT 'Prueba: Obtener registro por ID';
EXEC sp_Direccion_Tipo_ObtenerPorId @Id = @TestId;
GO

-- Prueba de actualización de un registro
DECLARE @ActualizarId INT = 1; -- Ajustar según ID existente
EXEC sp_Direccion_Tipo_Actualizar @Id = @ActualizarId, @Tipo = 'Domicilio Actualizado';
PRINT 'Actualización de registro correcta';
GO

-- Validación después de la actualización
PRINT 'Registros después de la actualización:';
EXEC sp_Direccion_Tipo_ObtenerTodos;
GO

-- Prueba de eliminación de un registro
DECLARE @EliminarId INT = 2; -- Ajustar según ID existente
EXEC sp_Direccion_Tipo_Eliminar @Id = @EliminarId;
PRINT 'Registro eliminado correctamente';

-- Validación posterior a la eliminación
PRINT 'Registros después de la eliminación:';
EXEC sp_Direccion_Tipo_ObtenerTodos;
GO

