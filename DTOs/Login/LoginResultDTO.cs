namespace CemSys3.DTOs.Login
{
    public class LoginResultDTO
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public int? UsuarioId { get; set; }
        public string? NombreUsuario { get; set; }
        public string? NombreEmpleado { get; set; }
        public string? ApellidoEmpleado { get; set; }
        public int? RolId { get; set; }
    }
}
