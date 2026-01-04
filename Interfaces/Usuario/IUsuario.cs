using CemSys3.DTOs.Generics;
using CemSys3.DTOs.Usuario;

namespace CemSys3.Interfaces.Usuario
{
    public interface IUsuario
    {
        string HashPassword(string password); //encripta la contraseña para usuarios nuevos
        bool VerificarPassword(string plainPassword, string storedHash); //verifica la contraseña ingresada con la almacenada
        Task<GenericResultDTO> Registrar(UsuarioRequestDTO dto); //registra un nuevo usuario como administrador
        Task<GenericResultDTO> Modificar(UsuarioRequestDTO dto); //modifica un usuario existente como administrador

    }
}
