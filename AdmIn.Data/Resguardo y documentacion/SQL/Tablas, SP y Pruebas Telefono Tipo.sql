-- Tabla Tipo de Teléfono
CREATE TABLE TELEFONO_TIPO (
    TEL_TIP_ID INT IDENTITY(1,1) PRIMARY KEY,
    TEL_TIP_DESCRIPCION NVARCHAR(50) NOT NULL
);

--STORES PROCEDURES

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Inserta un nuevo registro en la tabla TELEFONO_TIPO.
-- Entradas: @Descripcion NVARCHAR(50) - Descripción del tipo de teléfono.
-- Salidas: ID del registro insertado.
-- ====================================================================================
CREATE PROCEDURE sp_Telefono_Tipo_Insertar
    @Descripcion NVARCHAR(50),
	@NuevoId INT OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO TELEFONO_TIPO (TEL_TIP_DESCRIPCION)
    VALUES (@Descripcion);

    SET @NuevoId = SCOPE_IDENTITY();
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve todos los registros de la tabla TELEFONO_TIPO.
-- Entradas: Ninguna.
-- Salidas: Todos los registros de TELEFONO_TIPO.
-- ====================================================================================
CREATE PROCEDURE sp_Telefono_Tipo_ObtenerTodos
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM TELEFONO_TIPO;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Devuelve un registro específico de TELEFONO_TIPO basado en su ID.
-- Entradas: @Id INT - ID del tipo de teléfono.
-- Salidas: Registro correspondiente al ID.
-- ====================================================================================
CREATE PROCEDURE sp_Telefono_Tipo_ObtenerPorId
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM TELEFONO_TIPO WHERE TEL_TIP_ID = @Id;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Actualiza un registro en TELEFONO_TIPO basado en su ID.
-- Entradas: @Id INT, @Descripcion NVARCHAR(50).
-- Salidas: ID del registro actualizado.
-- ====================================================================================
CREATE PROCEDURE sp_Telefono_Tipo_Actualizar
    @Id INT,
    @Descripcion NVARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE TELEFONO_TIPO
    SET TEL_TIP_DESCRIPCION = @Descripcion
    WHERE TEL_TIP_ID = @Id;

    SELECT @Id AS ID;
END;
GO

-- ====================================================================================
-- Autor: Federico
-- Fecha: 12/12/2024
-- Descripción: Elimina un registro de TELEFONO_TIPO basado en su ID.
-- Entradas: @Id INT - ID del tipo de teléfono.
-- Salidas: Ninguna.
-- ====================================================================================
CREATE PROCEDURE sp_Telefono_Tipo_Eliminar
    @Id INT
AS
BEGIN
	SET NOCOUNT ON;
	DELETE FROM TELEFONO_TIPO WHERE TEL_TIP_ID = @Id;
END;
GO


-- ====================================================================================
-- Script de Pruebas para TELEFONO_TIPO y sus Procedimientos Almacenados
-- ====================================================================================

-- Validación inicial de los valores por defecto
PRINT 'Registros iniciales en TELEFONO_TIPO:';
EXEC sp_Telefono_Tipo_ObtenerTodos;
GO

-- Prueba del procedimiento de inserción
DECLARE @NuevoId INT;

EXEC @NuevoId = sp_Telefono_Tipo_Insertar @Descripcion = 'Emergencia', @NuevoId = @NuevoId OUTPUT;
PRINT 'Nuevo ID insertado: ' + CAST(@NuevoId AS NVARCHAR);

EXEC @NuevoId = sp_Telefono_Tipo_Insertar @Descripcion = 'Contacto Personal', @NuevoId = @NuevoId OUTPUT;
PRINT 'Nuevo ID insertado: ' + CAST(@NuevoId AS NVARCHAR);
GO

-- Validación de todos los registros después de inserciones
PRINT 'Registros actuales en TELEFONO_TIPO después de inserciones:';
EXEC sp_Telefono_Tipo_ObtenerTodos;
GO

-- Prueba de obtención por ID (usando un ID por defecto y uno nuevo)
DECLARE @TestId INT = 1; -- ID de un registro por defecto
PRINT 'Prueba: Obtener registro por defecto (ID = 1)';
EXEC sp_Telefono_Tipo_ObtenerPorId @Id = @TestId;

DECLARE @TestIdNuevo INT = 7; -- Ajustar según el nuevo ID generado
PRINT 'Prueba: Obtener registro nuevo';
EXEC sp_Telefono_Tipo_ObtenerPorId @Id = @TestIdNuevo;
GO

-- Prueba de actualización de un registro
DECLARE @ActualizarId INT = 2; -- ID de un registro por defecto
EXEC sp_Telefono_Tipo_Actualizar @Id = @ActualizarId, @Descripcion = 'Trabajo (Actualizado)';
PRINT 'Actualización de registro por defecto correcta';
GO

-- Prueba de eliminación de un registro nuevo
DECLARE @EliminarId INT = 7; -- Ajustar según el nuevo ID generado
EXEC sp_Telefono_Tipo_Eliminar @Id = @EliminarId;
PRINT 'Registro nuevo eliminado correctamente';

-- Validación posterior a la eliminación
PRINT 'Registros después de la eliminación:';
EXEC sp_Telefono_Tipo_ObtenerTodos;
GO
