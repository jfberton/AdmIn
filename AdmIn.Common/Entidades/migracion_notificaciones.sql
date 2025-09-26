-- Migración para la tabla Notificacion

CREATE TABLE Notificacion (
    NotificacionId INT IDENTITY PRIMARY KEY,
    UsuarioId INT NOT NULL,
    Tipo NVARCHAR(100) NOT NULL,
    Mensaje NVARCHAR(1000) NOT NULL,
    Leida BIT NOT NULL DEFAULT 0,
    Fecha DATETIME NOT NULL DEFAULT GETDATE(),
    Payload NVARCHAR(MAX) NULL,
    CONSTRAINT FK_Notificacion_Usuario FOREIGN KEY (UsuarioId) REFERENCES Usuario(UsuarioId)
);

CREATE INDEX IX_Notificacion_Usuario_Leida ON Notificacion(UsuarioId, Leida);
