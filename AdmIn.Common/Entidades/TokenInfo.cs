using System;

namespace AdmIn.Common.Entidades
{
    /// <summary>
    /// Información completa de un token de reseteo de contraseña con el usuario asociado
    /// </summary>
 public class TokenInfo
    {
        public Usuario Usuario { get; set; } = new Usuario();
   public PasswordResetToken Token { get; set; } = new PasswordResetToken();
    }
}
