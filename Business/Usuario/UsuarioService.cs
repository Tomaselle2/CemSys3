using CemSys3.DTOs.Generics;
using CemSys3.DTOs.Usuario;
using CemSys3.Interfaces.Usuario;
using CemSys3.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
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

        //cambia la visibilidad de un usuario
        public async Task Delete(int Id)
        {
            var usuario = await _contex.Usuarios.FindAsync(Id);
            if (usuario != null)
            {
                usuario.Visibilidad = false;
                _contex.Usuarios.Update(usuario);
                await _contex.SaveChangesAsync();
            }
        }

        public async Task<UsuarioRequestDTO> GetUserById(int Id)
        {
            return await _contex.Usuarios
            .AsNoTracking()
            .Where(u => u.Id == Id)
            .Select(u => new UsuarioRequestDTO
            {
                Id = u.Id,
                NombreEmpleado = u.Nombre,
                ApellidoEmpleado = u.Apellido,
                Correo = u.Correo,
                NombreUsuario = u.Usuario1,
                IdRol = u.RolId
            })
            .FirstOrDefaultAsync(); // También puedes usar FirstOrDefaultAsync
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

        //obtiene el listado de usuarios
        public async Task<IEnumerable<UsuarioRequestDTO>> ListadoUsuarios()
        {
            return await _contex.Usuarios.Where(u => u.Visibilidad).Select(u => new UsuarioRequestDTO
            {
                Id = u.Id,
                NombreEmpleado = u.Nombre,
                ApellidoEmpleado = u.Apellido,
                Correo = u.Correo,
                NombreUsuario = u.Usuario1,
                IdRol = u.RolId
            }).ToListAsync();
        }

        //modifica un usuario existente como administrador
        public async Task Modificar(UsuarioRequestDTO dto)
        {
            var usuarioExistente = await _contex.Usuarios.FindAsync(dto.Id); //busco el usuario por id

            //actualizo los datos del usuario
            usuarioExistente.Nombre = dto.NombreEmpleado.Trim();
            usuarioExistente.Apellido = dto.ApellidoEmpleado.Trim();
            usuarioExistente.Correo = dto.Correo.Trim();
            usuarioExistente.Usuario1 = dto.NombreUsuario.Trim();
            usuarioExistente.RolId = dto.IdRol;

            _contex.Usuarios.Update(usuarioExistente);
            await _contex.SaveChangesAsync();
        }

        //obtiene la lista de roles disponibles
        public async Task<IEnumerable<RolDTO>> ObtenerRoles()
        {
            return await _contex.RolesUsuarios.Select(r => new RolDTO {
                Id = r.Id,
                Rol = r.Rol
            }).ToListAsync();
        }

        //rigistra un nuevo usuario
        public async Task Registrar(UsuarioRequestDTO dto)
        {
            
                CemSys3.Models.Usuario usuario = new CemSys3.Models.Usuario
                {
                    Nombre = dto.NombreEmpleado.Trim(),
                    Apellido = dto.ApellidoEmpleado.Trim(),
                    Correo = dto.Correo.Trim(),
                    Usuario1 = dto.NombreUsuario.Trim(),
                    RolId = dto.IdRol,
                    Clave = HashPassword("1234"), // contraseña por defecto para nuevo usuarios
                    Visibilidad = true
                };

                _contex.Usuarios.Add(usuario);
                await _contex.SaveChangesAsync();
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
