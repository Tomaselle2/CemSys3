using CemSys3.DTOs.Login;
using CemSys3.Interfaces.Login;
using CemSys3.Interfaces.Usuario;
using CemSys3.Models;
using Microsoft.EntityFrameworkCore;

namespace CemSys3.Business.Login
{
    public class LoginService : ILogin
    {
        private readonly AppDbContext _contex;
        private readonly IUsuario _usuarioService;

        public LoginService(AppDbContext contex, IUsuario usuarioService)
        {
            _contex = contex;
            _usuarioService = usuarioService;
        }
        public async Task<LoginResultDTO> Loguearse(LoginDTO datosLogin)
        {
            // Validaciones
            if (string.IsNullOrWhiteSpace(datosLogin.Usuario))
                return new LoginResultDTO
                {
                    Success = false,
                    ErrorMessage = "El nombre de usuario es obligatorio"
                };

            if (string.IsNullOrWhiteSpace(datosLogin.Clave))
                return new LoginResultDTO
                {
                    Success = false,
                    ErrorMessage = "La contraseña es obligatoria"
                };

            // 1️_ Buscar usuario (SOLO por usuario)
            var usuario = await _contex.Usuarios
                .FirstOrDefaultAsync(u =>
                    u.Visibilidad == true &&
                    u.Usuario1 == datosLogin.Usuario);

            if (usuario == null)
                return new LoginResultDTO
                {
                    Success = false,
                    ErrorMessage = "Datos incorrectos. Intente nuevamente."
                };

            // 2️_ Verificar contraseña (FUERA de la BD)
            bool claveValida = _usuarioService.VerificarPassword(
                datosLogin.Clave,
                usuario.Clave
            );

            if (!claveValida)
                return new LoginResultDTO
                {
                    Success = false,
                    ErrorMessage = "Datos incorrectos. Intente nuevamente."
                };

            // 3️_ Login OK
            return new LoginResultDTO
            {
                Success = true,
                UsuarioId = usuario.Id,
                NombreUsuario = usuario.Usuario1,
                NombreEmpleado = usuario.Nombre,
                ApellidoEmpleado = usuario.Apellido,
                RolId = usuario.RolId
            };
        }
    }
}
