using AdmIn.Common.Entidades;
using AdmIn.Common.Repositorios;
using AdmIn.Common;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;

namespace AdmIn.Data.Repositorios
{
  public class PasswordResetTokenRepository : IPasswordResetTokenRepository
    {
 private readonly ILogger<PasswordResetTokenRepository> _logger;

        public PasswordResetTokenRepository(ILogger<PasswordResetTokenRepository> logger)
        {
            _logger = logger;
    }

        public async Task<DTO<int>> Crear(PasswordResetToken token)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
       await conexion.OpenAsync();
            try
        {
       _logger.LogInformation("========== CREATE PASSWORD RESET TOKEN START ==========");
       _logger.LogInformation($"UsuarioId: {token.UsuarioId}");
          _logger.LogInformation($"PersonaId: {token.PersonaId}");
       _logger.LogInformation($"Token: {token.Token?.Substring(0, Math.Min(10, token.Token?.Length ?? 0))}...");
   _logger.LogInformation($"CreatedAt: {token.CreatedAt:yyyy-MM-dd HH:mm:ss}");
    _logger.LogInformation($"ExpiresAt: {token.ExpiresAt:yyyy-MM-dd HH:mm:ss}");
                _logger.LogInformation($"Purpose: {token.Purpose}");

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

      _logger.LogInformation($"? Token created with ID: {id}");
      _logger.LogInformation("========== CREATE PASSWORD RESET TOKEN END ==========");

          return new DTO<int> { Correcto = true, Datos = id, Mensaje = "Token creado" };
         }
    catch (Exception ex)
    {
         _logger.LogError($"? Error creating token: {ex.Message}");
      _logger.LogError($"Stack Trace: {ex.StackTrace}");
      return new DTO<int> { Correcto = false, Mensaje = ex.Message };
            }
   }

        public async Task<DTO<PasswordResetToken>> Obtener_por_token(string token)
        {
            using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
   try
            {
           _logger.LogInformation("========== GET PASSWORD RESET TOKEN START ==========");
   _logger.LogInformation($"Searching for token: {token?.Substring(0, Math.Min(10, token?.Length ?? 0))}...");

       var sql = @"SELECT Id, UsuarioId, PersonaId, Token, CreatedAt, ExpiresAt, ConsumedAt, IsConsumed, Purpose
            FROM PasswordResetTokens WHERE Token = @Token";

 _logger.LogInformation($"Executing query: {sql}");
      _logger.LogInformation($"Parameter @Token: {token}");

 var result = await conexion.QuerySingleOrDefaultAsync<PasswordResetToken>(sql, new { Token = token });

 if (result == null)
         {
        _logger.LogWarning("? Token NOT FOUND in database");
    _logger.LogInformation("========== GET PASSWORD RESET TOKEN END (NOT FOUND) ==========");
        return new DTO<PasswordResetToken> { Correcto = false, Mensaje = "Token no encontrado" };
     }

                _logger.LogInformation("? Token FOUND in database");
       _logger.LogInformation($"Token Details:");
         _logger.LogInformation($"  Id: {result.Id}");
      _logger.LogInformation($"  UsuarioId: {result.UsuarioId}");
                _logger.LogInformation($"  PersonaId: {result.PersonaId}");
       _logger.LogInformation($"  Token: {result.Token?.Substring(0, Math.Min(10, result.Token?.Length ?? 0))}...");
       _logger.LogInformation($"  CreatedAt: {result.CreatedAt:yyyy-MM-dd HH:mm:ss}");
                _logger.LogInformation($"  ExpiresAt: {result.ExpiresAt:yyyy-MM-dd HH:mm:ss}");
         _logger.LogInformation($"  ConsumedAt: {result.ConsumedAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? "null"}");
        _logger.LogInformation($"  IsConsumed: {result.IsConsumed}");
            _logger.LogInformation($"  Purpose: {result.Purpose ?? "null"}");

    var dto = new DTO<PasswordResetToken>
         {
      Correcto = true,
  Datos = result,
         Mensaje = "Token encontrado"
                };

                _logger.LogInformation($"? Returning DTO - Correcto: {dto.Correcto}, Datos is null: {dto.Datos == null}");
         _logger.LogInformation("========== GET PASSWORD RESET TOKEN END (SUCCESS) ==========");

       return dto;
      }
            catch (Exception ex)
         {
      _logger.LogError($"? ERROR getting token: {ex.Message}");
       _logger.LogError($"Exception Type: {ex.GetType().Name}");
         _logger.LogError($"Stack Trace: {ex.StackTrace}");

      if (ex.InnerException != null)
{
 _logger.LogError($"Inner Exception: {ex.InnerException.Message}");
   }

        _logger.LogInformation("========== GET PASSWORD RESET TOKEN END (ERROR) ==========");
       return new DTO<PasswordResetToken> { Correcto = false, Mensaje = ex.Message };
            }
     }

        public async Task<DTO<bool>> Marcar_consumido(int tokenId)
        {
     using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
  try
      {
                _logger.LogInformation($"========== MARK TOKEN AS CONSUMED START ==========");
         _logger.LogInformation($"Token ID: {tokenId}");

         var sql = @"UPDATE PasswordResetTokens SET IsConsumed = 1, ConsumedAt = SYSUTCDATETIME() WHERE Id = @Id";
  var filas = await conexion.ExecuteAsync(sql, new { Id = tokenId });

    _logger.LogInformation($"? Rows affected: {filas}");
  _logger.LogInformation("========== MARK TOKEN AS CONSUMED END ==========");

            return new DTO<bool>
  {
         Correcto = filas > 0,
      Datos = filas > 0,
        Mensaje = filas > 0 ? "Token marcado como consumido" : "Token no encontrado"
                };
       }
    catch (Exception ex)
          {
         _logger.LogError($"? Error marking token as consumed: {ex.Message}");
          _logger.LogError($"Stack Trace: {ex.StackTrace}");
      return new DTO<bool> { Correcto = false, Mensaje = ex.Message };
   }
    }

        public async Task<DTO<bool>> Expirar_tokens_vencidos()
     {
          using var conexion = new SqlConnection(InfoSQL.Conexion);
            await conexion.OpenAsync();
         try
 {
         _logger.LogInformation("========== EXPIRE OLD TOKENS START ==========");

var sql = @"UPDATE PasswordResetTokens SET IsConsumed = 1 WHERE ExpiresAt < SYSUTCDATETIME() AND IsConsumed = 0";
  var filas = await conexion.ExecuteAsync(sql);

    _logger.LogInformation($"? Expired {filas} tokens");
      _logger.LogInformation("========== EXPIRE OLD TOKENS END ==========");

                return new DTO<bool> { Correcto = true, Datos = true, Mensaje = $"{filas} tokens expirados" };
            }
     catch (Exception ex)
{
     _logger.LogError($"? Error expiring tokens: {ex.Message}");
       _logger.LogError($"Stack Trace: {ex.StackTrace}");
                return new DTO<bool> { Correcto = false, Mensaje = ex.Message };
      }
        }
    }
}
