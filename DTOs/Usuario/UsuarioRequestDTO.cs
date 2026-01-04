namespace CemSys3.DTOs.Usuario
{
    public class UsuarioRequestDTO
    {
        public int? Id { get; set; }
        public string NombreEmpleado { get; set; } = null!;

        public string ApellidoEmpleado { get; set; } = null!;

        public string Correo { get; set; } = null!;

        public string NombreUsuario { get; set; } = null!;

        public int IdRol { get; set; }
    }
}
