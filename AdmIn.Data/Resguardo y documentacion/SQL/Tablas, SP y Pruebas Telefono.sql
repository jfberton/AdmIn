-- Tabla Teléfono
CREATE TABLE TELEFONO (
    TEL_ID INT IDENTITY(1,1) PRIMARY KEY,
    TEL_NUMERO NVARCHAR(20) NOT NULL,
    TEL_TIP_ID INT NOT NULL FOREIGN KEY REFERENCES TELEFONO_TIPO(TEL_TIP_ID)
);

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Inserta un nuevo registro en la tabla TELEFONO.
-- Entradas: @Numero NVARCHAR(20), @TipoId INT.
-- Salidas: ID del registro insertado.
-- ====================================================================================
CREATE PROCEDURE sp_Telefono_Insertar
    @Numero NVARCHAR(20),
    @TipoId INT,
    @NuevoId INT OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO TELEFONO (TEL_NUMERO, TEL_TIP_ID)
    VALUES (@Numero, @TipoId);

    SET @NuevoId = SCOPE_IDENTITY();
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve todos los registros de la tabla TELEFONO.
-- Entradas: Ninguna.
-- Salidas: Todos los registros de TELEFONO.
-- ====================================================================================
CREATE PROCEDURE sp_Telefono_ObtenerTodos
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM TELEFONO;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve un registro específico de TELEFONO basado en su ID.
-- Entradas: @Id INT - ID del teléfono.
-- Salidas: Registro correspondiente al ID.
-- ====================================================================================
CREATE PROCEDURE sp_Telefono_ObtenerPorId
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM TELEFONO WHERE TEL_ID = @Id;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Actualiza un registro en TELEFONO basado en su ID.
-- Entradas: @Id INT, @Numero NVARCHAR(20), @TipoId INT.
-- Salidas: ID del registro actualizado.
-- ====================================================================================
CREATE PROCEDURE sp_Telefono_Actualizar
    @Id INT,
    @Numero NVARCHAR(20),
    @TipoId INT
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE TELEFONO
    SET TEL_NUMERO = @Numero,
        TEL_TIP_ID = @TipoId
    WHERE TEL_ID = @Id;

    SELECT @Id AS ID;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Elimina un registro de TELEFONO basado en su ID.
-- Entradas: @Id INT - ID del teléfono.
-- Salidas: Ninguna.
-- ====================================================================================
CREATE PROCEDURE sp_Telefono_Eliminar
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	DELETE FROM TELEFONO WHERE TEL_ID = @Id;
END;
GO

-- ====================================================================================
-- Script de Pruebas para TELEFONO y sus Procedimientos Almacenados
-- ====================================================================================

-- Validación inicial de los registros existentes
PRINT 'Registros iniciales en TELEFONO:';
EXEC sp_Telefono_ObtenerTodos;
GO

-- Prueba del procedimiento de inserción
DECLARE @NuevoId INT;

EXEC sp_Telefono_Insertar @Numero = '123456789', @TipoId = 1, @NuevoId = @NuevoId OUTPUT;
PRINT 'Nuevo ID insertado: ' + CAST(@NuevoId AS NVARCHAR);

EXEC sp_Telefono_Insertar @Numero = '987654321', @TipoId = 2, @NuevoId = @NuevoId OUTPUT;
PRINT 'Nuevo ID insertado: ' + CAST(@NuevoId AS NVARCHAR);
GO

-- Validación de todos los registros después de las inserciones
PRINT 'Registros actuales en TELEFONO después de inserciones:';
EXEC sp_Telefono_ObtenerTodos;
GO

-- Prueba de obtención por ID
DECLARE @TestId INT = 1; -- Ajustar según los IDs generados
PRINT 'Prueba: Obtener registro por ID';
EXEC sp_Telefono_ObtenerPorId @Id = @TestId;
GO

-- Prueba de actualización de un registro
DECLARE @ActualizarId INT = 2; -- Ajustar según ID existente
EXEC sp_Telefono_Actualizar @Id = @ActualizarId, @Numero = '555555555', @TipoId = 3; -- Ajustar TipoId existente
PRINT 'Actualización de registro correcta';
GO

-- Validación después de la actualización
PRINT 'Registros después de la actualización:';
EXEC sp_Telefono_ObtenerTodos;
GO

-- Prueba de eliminación de un registro
DECLARE @EliminarId INT = 2; -- Ajustar según ID existente
EXEC sp_Telefono_Eliminar @Id = @EliminarId;
PRINT 'Registro eliminado correctamente';

-- Validación posterior a la eliminación
PRINT 'Registros después de la eliminación:';
EXEC sp_Telefono_ObtenerTodos;
GO
