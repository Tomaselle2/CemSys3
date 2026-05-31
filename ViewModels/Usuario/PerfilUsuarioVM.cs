using CemSys3.DTOs.SweetAlert;
using System.ComponentModel.DataAnnotations;

namespace CemSys3.ViewModels.Usuario
{
    public class PerfilUsuarioVM
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(50, ErrorMessage = "El nombre no puede superar los 50 caracteres")]
        public string? Nombre { get; set; } = null!;

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(50, ErrorMessage = "El apellido no puede superar los 50 caracteres")]
        public string? Apellido { get; set; } = null!;

        [Required(ErrorMessage = "El correo es obligatorio")]
        [StringLength(50, ErrorMessage = "El correo no puede superar los 50 caracteres")]
        public string? Correo { get; set; } = null!;

        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        [StringLength(50, ErrorMessage = "El usuario no puede superar los 50 caracteres")]
        public string? NombreUsuario { get; set; } = null!;

        public string? Rol { get; set; }

        public SweetAlertDTO? SweetAlert { get; set; }
    }
}
