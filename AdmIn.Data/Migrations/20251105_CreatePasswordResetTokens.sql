-- Migration: Create table PasswordResetTokens
-- Date:2025-11-05

CREATE TABLE PasswordResetTokens (
 Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
 UsuarioId INT NULL,
 PersonaId INT NULL,
 Token NVARCHAR(256) NOT NULL,
 CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
 ExpiresAt DATETIME2 NOT NULL,
 ConsumedAt DATETIME2 NULL,
 IsConsumed BIT NOT NULL DEFAULT 0,
 Purpose NVARCHAR(100) NULL
);

CREATE UNIQUE INDEX IX_PasswordResetTokens_Token ON PasswordResetTokens(Token);

-- Optional foreign keys (uncomment if Usuario and Persona tables exist and names match)
-- ALTER TABLE PasswordResetTokens ADD CONSTRAINT FK_PasswordResetTokens_Usuario FOREIGN KEY (UsuarioId) REFERENCES Usuario(UsuarioID);
-- ALTER TABLE PasswordResetTokens ADD CONSTRAINT FK_PasswordResetTokens_Persona FOREIGN KEY (PersonaId) REFERENCES Persona(PER_ID);
