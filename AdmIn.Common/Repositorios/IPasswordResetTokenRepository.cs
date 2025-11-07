using AdmIn.Common.Entidades;
using AdmIn.Common.Utilidades;
using System.Threading.Tasks;

namespace AdmIn.Common.Repositorios
{
 public interface IPasswordResetTokenRepository
 {
 Task<DTO<int>> Crear(PasswordResetToken token);
 Task<DTO<PasswordResetToken>> Obtener_por_token(string token);
 Task<DTO<bool>> Marcar_consumido(int tokenId);
 Task<DTO<bool>> Expirar_tokens_vencidos();
 }
}