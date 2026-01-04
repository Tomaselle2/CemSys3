using CemSys3.Interfaces.Usuario;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace CemSys3.Business.Usuario
{
    public class UsuarioService : IUsuario
    {
        //encripta la contraseña para usuarios nuevos
        public string HashPassword(string password)
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

            return $"{Convert.ToBase64String(salt)}.{hashed}";
        }

        //verifica la contraseña ingresada con la almacenada
        public bool VerificarPassword(string plainPassword, string storedHash) 
        {
            if (string.IsNullOrEmpty(plainPassword) || string.IsNullOrEmpty(storedHash))
                return false;

            try
            {
                // Extraer salt y hash almacenado
                var parts = storedHash.Split('.');
                if (parts.Length != 2)
                    return false;

                var salt = Convert.FromBase64String(parts[0]);
                var storedSubHash = parts[1];

                // Calcular hash de la contraseña proporcionada con el mismo salt
                var hashedInput = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: plainPassword,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8));

                // Comparación segura
                return SecureCompare(hashedInput, storedSubHash);
            }
            catch
            {
                return false;
            }
        }


        // Método privado de comparación segura
        private bool SecureCompare(string a, string b)
        {
            if (a == null || b == null)
                return false;

            // Comparación de tiempo constante
            int minLength = Math.Min(a.Length, b.Length);
            int maxLength = Math.Max(a.Length, b.Length);

            bool result = (a.Length == b.Length);

            for (int i = 0; i < minLength; i++)
            {
                result &= (a[i] == b[i]);
            }

            return result;
        }
    }
}
