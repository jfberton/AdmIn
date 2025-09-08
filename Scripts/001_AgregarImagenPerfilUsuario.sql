-- =============================================
-- Script: Agregar campo ImagenPerfilId a la tabla Usuario
-- Autor: Sistema AdmIn  
-- Fecha: 2025-01-07
-- Descripción: Agrega campo para almacenar imagen de perfil del usuario
-- =============================================

BEGIN TRY
    BEGIN TRANSACTION

    -- Verificar si la columna ya existe antes de agregarla
    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                   WHERE TABLE_NAME = 'Usuario' AND COLUMN_NAME = 'ImagenPerfilId')
    BEGIN
        ALTER TABLE Usuario ADD ImagenPerfilId UNIQUEIDENTIFIER NULL;
        PRINT 'Campo ImagenPerfilId agregado exitosamente a la tabla Usuario';
    END
    ELSE
    BEGIN
        PRINT 'El campo ImagenPerfilId ya existe en la tabla Usuario';
    END

    -- Agregar foreign key constraint con la tabla Imagen
    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS 
                   WHERE CONSTRAINT_NAME = 'FK_Usuario_Imagen_Perfil')
    BEGIN
        ALTER TABLE Usuario 
        ADD CONSTRAINT FK_Usuario_Imagen_Perfil 
        FOREIGN KEY (ImagenPerfilId) REFERENCES Imagen(Id) 
        ON DELETE SET NULL;
        PRINT 'Constraint FK_Usuario_Imagen_Perfil creado exitosamente';
    END

    -- Crear índice para mejorar performance
    IF NOT EXISTS (SELECT * FROM sys.indexes 
                   WHERE name = 'IX_Usuario_ImagenPerfilId' AND object_id = OBJECT_ID('Usuario'))
    BEGIN
        CREATE NONCLUSTERED INDEX IX_Usuario_ImagenPerfilId ON Usuario (ImagenPerfilId);
        PRINT 'Índice IX_Usuario_ImagenPerfilId creado exitosamente';
    END

    COMMIT TRANSACTION
    PRINT 'Script completado exitosamente';

END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION
    PRINT 'Error: ' + ERROR_MESSAGE();
END CATCH