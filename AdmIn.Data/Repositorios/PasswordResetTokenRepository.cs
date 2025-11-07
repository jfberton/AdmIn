using AdmIn.Common.Entidades;
using AdmIn.Common.Repositorios;
using AdmIn.Common;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using System;

namespace AdmIn.Data.Repositorios
{
    public class PasswordResetTokenRepository : IPasswordResetTokenRepository
    {
        public async Task<DTO<int>> Crear(PasswordResetToken token)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            try
            {
                var sql = @"INSERT INTO PasswordResetTokens (UsuarioId, PersonaId, Token, CreatedAt, ExpiresAt, IsConsumed, Purpose)
                            VALUES (@UsuarioId, @PersonaId, @Token, @CreatedAt, @ExpiresAt, @IsConsumed, @Purpose);
                            SELECT CAST(SCOPE_IDENTITY() as int);";

                var id = await conexion.ExecuteScalarAsync<int>(sql, new
                {
                    token.UsuarioId,
                    token.PersonaId,
                    token.Token,
                    CreatedAt = token.CreatedAt,
                    ExpiresAt = token.ExpiresAt,
                    IsConsumed = token.IsConsumed,
                    Purpose = token.Purpose
                });

                return new DTO<int> { Correcto = true, Datos = id, Mensaje = "Token creado" };
            }
            catch (Exception ex)
            {
                return new DTO<int> { Correcto = false, Mensaje = ex.Message };
            }
        }

        public async Task<DTO<PasswordResetToken>> Obtener_por_token(string token)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            try
            {
                var sql = @"SELECT Id, UsuarioId, PersonaId, Token, CreatedAt, ExpiresAt, ConsumedAt, IsConsumed, Purpose
                            FROM PasswordResetTokens WHERE Token = @Token";
                var result = await conexion.QuerySingleOrDefaultAsync<PasswordResetToken>(sql, new { Token = token });
                if (result == null) return new DTO<PasswordResetToken> { Correcto = false, Mensaje = "Token no encontrado" };
                return new DTO<PasswordResetToken> { Correcto = true, Datos = result };
            }
            catch (Exception ex)
            {
                return new DTO<PasswordResetToken> { Correcto = false, Mensaje = ex.Message };
            }
        }

        public async Task<DTO<bool>> Marcar_consumido(int tokenId)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            try
            {
                var sql = @"UPDATE PasswordResetTokens SET IsConsumed =1, ConsumedAt = SYSUTCDATETIME() WHERE Id = @Id";
                var filas = await conexion.ExecuteAsync(sql, new { Id = tokenId });
                return new DTO<bool> { Correcto = filas > 0, Datos = filas > 0 };
            }
            catch (Exception ex)
            {
                return new DTO<bool> { Correcto = false, Mensaje = ex.Message };
            }
        }

        public async Task<DTO<bool>> Expirar_tokens_vencidos()
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
            try
            {
                var sql = @"UPDATE PasswordResetTokens SET IsConsumed =1 WHERE ExpiresAt < SYSUTCDATETIME() AND IsConsumed =0";
                var filas = await conexion.ExecuteAsync(sql);
                return new DTO<bool> { Correcto = true, Datos = true, Mensaje = $"{filas} tokens expirados" };
            }
            catch (Exception ex)
            {
                return new DTO<bool> { Correcto = false, Mensaje = ex.Message };
            }
        }
    }
}