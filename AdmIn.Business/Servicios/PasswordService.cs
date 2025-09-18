using AdmIn.Common.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmIn.Business.Servicios
{
    /// <summary>
    /// Interfaz para el servicio de gestión de contraseñas
    /// </summary>
    public interface IPasswordService
    {
        /// <summary>
        /// Genera un hash bcrypt para una nueva contraseña
        /// </summary>
        string HashPassword(string password);

        /// <summary>
        /// Verifica una contraseña contra un hash (compatible con bcrypt y SHA512)
        /// </summary>
        bool VerifyPassword(string password, string hash);

        /// <summary>
        /// Migra una contraseña de SHA512 a bcrypt si es necesario
        /// </summary>
        string? MigratePasswordIfNeeded(string password, string hash);

        /// <summary>
        /// Verifica si un hash necesita migración
        /// </summary>
        bool NeedsMigration(string hash);

        /// <summary>
        /// Genera una contraseña temporal segura
        /// </summary>
        string GenerateTemporaryPassword(int length = 12);

        /// <summary>
        /// Valida la fortaleza de una contraseña
        /// </summary>
        PasswordStrengthResult ValidatePasswordStrength(string password);
    }

    /// <summary>
    /// Servicio de gestión de contraseñas que utiliza bcrypt como método principal
    /// y mantiene compatibilidad con SHA512 para migración gradual
    /// </summary>
    public class PasswordService : IPasswordService
    {
        private readonly int _bcryptWorkFactor;

        public PasswordService(int bcryptWorkFactor = 12)
        {
            _bcryptWorkFactor = bcryptWorkFactor;
        }

        public string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("La contraseña no puede estar vacía", nameof(password));

            return MiHash.GenerarHashBcrypt(password, _bcryptWorkFactor);
        }

        public bool VerifyPassword(string password, string hash)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hash))
                return false;

            if (MiHash.EsHashBcrypt(hash))
            {
                return MiHash.VerificarHashBcrypt(password, hash);
            }
            else
            {
                // Compatibilidad con SHA512
                string sha512Hash = MiHash.GenerarHash(password);
                return sha512Hash == hash;
            }
        }

        public string? MigratePasswordIfNeeded(string password, string hash)
        {
            if (MiHash.EsHashBcrypt(hash))
                return null; // Ya es bcrypt, no necesita migración

            if (VerifyPassword(password, hash))
            {
                return HashPassword(password); // Crear nuevo hash bcrypt
            }

            return null;
        }

        public bool NeedsMigration(string hash)
        {
            return PasswordMigrationUtility.NecesitaMigracion(hash);
        }

        public string GenerateTemporaryPassword(int length = 12)
        {
            if (length < 8)
                length = 8;

            const string upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowerCase = "abcdefghijklmnopqrstuvwxyz";
            const string digits = "0123456789";
            const string specialChars = "!@#$%^&*";

            var random = new Random();
            var password = new StringBuilder();

            // Asegurar al menos un carácter de cada tipo
            password.Append(upperCase[random.Next(upperCase.Length)]);
            password.Append(lowerCase[random.Next(lowerCase.Length)]);
            password.Append(digits[random.Next(digits.Length)]);
            password.Append(specialChars[random.Next(specialChars.Length)]);

            // Completar con caracteres aleatorios
            const string allChars = upperCase + lowerCase + digits + specialChars;
            for (int i = 4; i < length; i++)
            {
                password.Append(allChars[random.Next(allChars.Length)]);
            }

            // Mezclar los caracteres
            var chars = password.ToString().ToCharArray();
            for (int i = chars.Length - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (chars[i], chars[j]) = (chars[j], chars[i]);
            }

            return new string(chars);
        }

        public PasswordStrengthResult ValidatePasswordStrength(string password)
        {
            var result = new PasswordStrengthResult();

            if (string.IsNullOrEmpty(password))
            {
                result.IsValid = false;
                result.Errors.Add("La contraseña no puede estar vacía");
                return result;
            }

            // Longitud mínima
            if (password.Length < 8)
            {
                result.Errors.Add("La contraseña debe tener al menos 8 caracteres");
            }

            // Verificar mayúsculas
            if (!password.Any(char.IsUpper))
            {
                result.Errors.Add("La contraseña debe contener al menos una letra mayúscula");
            }

            // Verificar minúsculas
            if (!password.Any(char.IsLower))
            {
                result.Errors.Add("La contraseña debe contener al menos una letra minúscula");
            }

            // Verificar números
            if (!password.Any(char.IsDigit))
            {
                result.Errors.Add("La contraseña debe contener al menos un número");
            }

            // Verificar caracteres especiales
            if (!password.Any(c => "!@#$%^&*()_+-=[]{}|;:,.<>?".Contains(c)))
            {
                result.Warnings.Add("Se recomienda usar al menos un carácter especial");
            }

            // Longitud recomendada
            if (password.Length >= 12)
            {
                result.Score += 2;
            }
            else if (password.Length >= 10)
            {
                result.Score += 1;
            }

            result.IsValid = result.Errors.Count == 0;
            return result;
        }
    }

    /// <summary>
    /// Resultado de la validación de fortaleza de contraseña
    /// </summary>
    public class PasswordStrengthResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
        public int Score { get; set; } = 0;
        public string Strength => Score switch
        {
            >= 4 => "Muy Fuerte",
            >= 3 => "Fuerte",
            >= 2 => "Moderada",
            >= 1 => "Débil",
            _ => "Muy Débil"
        };
    }
}