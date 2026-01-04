using CemSys3.DTOs.Generics;
using CemSys3.DTOs.Usuario;
using CemSys3.Interfaces.Usuario;
using CemSys3.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace CemSys3.Business.Usuario
{
    public class UsuarioService : IUsuario
    {
        private readonly AppDbContext _contex;

        public UsuarioService(AppDbContext contex)
        {
            _contex = contex;
        }

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

        //modifica un usuario existente como administrador
        public async Task<GenericResultDTO> Modificar(UsuarioRequestDTO dto)
        {
            var usuarioExistente = await _contex.Usuarios.FindAsync(dto.Id); //busco el usuario por id

            if (usuarioExistente == null)
            {
                return new GenericResultDTO
                {
                    Success = false,
                    Message = "Usuario no encontrado"
                };
            }

            //actualizo los datos del usuario
            usuarioExistente.Nombre = dto.NombreEmpleado;
            usuarioExistente.Apellido = dto.ApellidoEmpleado;
            usuarioExistente.Correo = dto.Correo;
            usuarioExistente.Usuario1 = dto.NombreUsuario;
            usuarioExistente.RolId = dto.IdRol;

            try
            {
                _contex.Usuarios.Update(usuarioExistente);
                await _contex.SaveChangesAsync();
                return new GenericResultDTO
                {
                    Success = true,
                    Message = "Usuario modificado correctamente",
                    Id = usuarioExistente.Id
                };
            }
            catch (Exception ex)
            {
                return new GenericResultDTO
                {
                    Success = false,
                    Message = "Ocurrió un error al modificar el usuario. " + ex.Message
                };
            }

        }

        //rigistra un nuevo usuario
        public async Task<GenericResultDTO> Registrar(UsuarioRequestDTO dto)
        {
            try
            {
                CemSys3.Models.Usuario usuario = new CemSys3.Models.Usuario
                {
                    Nombre = dto.NombreEmpleado,
                    Apellido = dto.ApellidoEmpleado,
                    Correo = dto.Correo,
                    Usuario1 = dto.NombreUsuario,
                    RolId = dto.IdRol,
                    Clave = HashPassword("1234"), // contraseña por defecto para nuevo usuarios
                    Visibilidad = true
                };

                _contex.Usuarios.Add(usuario);
                await _contex.SaveChangesAsync();

                return new GenericResultDTO
                {
                    Success = true,
                    Message = "Usuario registrado correctamente",
                    Id = usuario.Id
                };
            }
            catch (Exception ex)
            {
                return new GenericResultDTO
                {
                    Success = false,
                    Message = "Ocurrió un error al registrar el usuario. " + ex.Message
                };
            }
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
