using System;

namespace AdmIn.Common.Entidades
{
 public class PasswordResetToken
 {
 public int Id { get; set; }
 public int? UsuarioId { get; set; }
 public int? PersonaId { get; set; }
 public string Token { get; set; } = string.Empty;
 public DateTime CreatedAt { get; set; }
 public DateTime ExpiresAt { get; set; }
 public DateTime? ConsumedAt { get; set; }
 public bool IsConsumed { get; set; }
 public string? Purpose { get; set; }
 }
}