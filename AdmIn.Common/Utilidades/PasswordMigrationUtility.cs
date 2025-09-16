using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmIn.Common.Utilidades
{
    /// <summary>
    /// Utilidad para migrar contraseñas de SHA512 a bcrypt
    /// </summary>
    public static class PasswordMigrationUtility
    {
        /// <summary>
        /// Convierte un hash SHA512 existente a bcrypt cuando el usuario hace login
        /// Esto permite una migración gradual y transparente
        /// </summary>
        /// <param name="passwordTextoPlano">Contraseña en texto plano proporcionada por el usuario</param>
        /// <param name="hashSHA512Almacenado">Hash SHA512 actualmente almacenado</param>
        /// <returns>Nuevo hash bcrypt si la contraseña es correcta, null si no coincide</returns>
        public static string? MigrarDeSHA512ABcrypt(string passwordTextoPlano, string hashSHA512Almacenado)
        {
            if (string.IsNullOrEmpty(passwordTextoPlano) || string.IsNullOrEmpty(hashSHA512Almacenado))
                return null;

            // Verificar que la contraseña coincida con el hash SHA512
            string hashSHA512Calculado = MiHash.GenerarHash(passwordTextoPlano);
            
            if (hashSHA512Calculado == hashSHA512Almacenado)
            {
                // La contraseña es correcta, generar nuevo hash bcrypt
                return MiHash.GenerarHashBcrypt(passwordTextoPlano);
            }

            return null;
        }

        /// <summary>
        /// Verifica si necesita migración de contraseña
        /// </summary>
        /// <param name="hashAlmacenado">Hash almacenado en la base de datos</param>
        /// <returns>True si necesita migración (es SHA512), False si ya es bcrypt</returns>
        public static bool NecesitaMigracion(string hashAlmacenado)
        {
            if (string.IsNullOrEmpty(hashAlmacenado))
                return false;

            return !MiHash.EsHashBcrypt(hashAlmacenado);
        }

        /// <summary>
        /// Estadísticas de migración para monitoreo
        /// </summary>
        public class EstadisticasMigracion
        {
            public int TotalUsuarios { get; set; }
            public int UsuariosConBcrypt { get; set; }
            public int UsuariosConSHA512 { get; set; }
            public double PorcentajeMigrado => TotalUsuarios > 0 ? (double)UsuariosConBcrypt / TotalUsuarios * 100 : 0;
        }
    }
}