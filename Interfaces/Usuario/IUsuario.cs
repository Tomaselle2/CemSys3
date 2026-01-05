using CemSys3.DTOs.Generics;
using CemSys3.DTOs.Usuario;
using System.Collections;

namespace CemSys3.Interfaces.Usuario
{
    public interface IUsuario
    {
        string HashPassword(string password); //encripta la contraseña para usuarios nuevos
        bool VerificarPassword(string plainPassword, string storedHash); //verifica la contraseña ingresada con la almacenada
        Task Registrar(UsuarioRequestDTO dto); //registra un nuevo usuario como administrador
        Task Modificar(UsuarioRequestDTO dto); //modifica un usuario existente como administrador
        Task<IEnumerable<RolDTO>> ObtenerRoles(); //obtiene la lista de roles disponibles
        Task<IEnumerable<UsuarioRequestDTO>> ListadoUsuarios(); //obtiene el listado de usuarios

    }
}
