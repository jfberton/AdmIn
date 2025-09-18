using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;

namespace AdmIn.Common.Utilidades
{
    public class MiHash
    {
        /// <summary>
        /// Genera un hash SHA512 (método heredado para compatibilidad)
        /// </summary>
        /// <param name="input">Texto a hashear</param>
        /// <returns>Hash SHA512 en hexadecimal</returns>
        public static string GenerarHash(string input)
        {
            HashAlgorithm hashAlgorithm = SHA512.Create();

            // Convert the input string to a byte array and compute the hash.
            byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            var sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        /// <summary>
        /// Genera un hash bcrypt para contraseñas (recomendado para nuevas implementaciones)
        /// </summary>
        /// <param name="password">Contraseña a hashear</param>
        /// <param name="workFactor">Factor de trabajo (por defecto 12, más alto = más seguro pero más lento)</param>
        /// <returns>Hash bcrypt</returns>
        public static string GenerarHashBcrypt(string password, int workFactor = 12)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("La contraseña no puede estar vacía", nameof(password));

            try
            {
                Console.WriteLine($"[MIHASH] ✓ Generando hash BCrypt con work factor {workFactor}");
                return BCrypt.Net.BCrypt.HashPassword(password, workFactor);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MIHASH] ❌ Error generando hash BCrypt: {ex.Message}");
                Console.WriteLine($"[MIHASH] ⚠️ Fallback a SHA512 por error en BCrypt");
                return GenerarHash(password);
            }
        }

        /// <summary>
        /// Verifica una contraseña contra un hash bcrypt
        /// </summary>
        /// <param name="password">Contraseña en texto plano</param>
        /// <param name="hash">Hash bcrypt almacenado</param>
        /// <returns>True si la contraseña es correcta</returns>
        public static bool VerificarHashBcrypt(string password, string hash)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hash))
                return false;

            try
            {
                Console.WriteLine($"[MIHASH] ✓ Verificando password con BCrypt");
                return BCrypt.Net.BCrypt.Verify(password, hash);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MIHASH] ❌ Error verificando con BCrypt: {ex.Message}");
                Console.WriteLine($"[MIHASH] ⚠️ Fallback a SHA512 por error en BCrypt");
                
                // Fallback a SHA512 si hay error con BCrypt
                string passwordHashSHA512 = GenerarHash(password);
                return passwordHashSHA512 == hash;
            }
        }

        /// <summary>
        /// Verifica si un hash es de tipo bcrypt
        /// </summary>
        /// <param name="hash">Hash a verificar</param>
        /// <returns>True si es un hash bcrypt válido</returns>
        public static bool EsHashBcrypt(string hash)
        {
            if (string.IsNullOrEmpty(hash))
                return false;

            try
            {
                // Los hashes bcrypt empiezan con $2a$, $2b$, $2x$, o $2y$
                bool esBcrypt = hash.StartsWith("$2a$") || hash.StartsWith("$2b$") || 
                               hash.StartsWith("$2x$") || hash.StartsWith("$2y$");
                
                Console.WriteLine($"[MIHASH] ✓ Hash detectado como: {(esBcrypt ? "BCrypt" : "SHA512/Legacy")}");
                return esBcrypt;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MIHASH] ❌ Error detectando tipo de hash: {ex.Message}");
                return false;
            }
        }
    }
}
