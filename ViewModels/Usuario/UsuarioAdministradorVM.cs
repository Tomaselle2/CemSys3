using CemSys3.DTOs.SweetAlert;
using CemSys3.DTOs.Usuario;
using System.ComponentModel.DataAnnotations;

namespace CemSys3.ViewModels.Usuario
{
    public class UsuarioAdministradorVM
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [MaxLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres.")]
        public string? NombreEmpleado { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [MaxLength(50, ErrorMessage = "El apellido no puede exceder los 50 caracteres.")]
        public string? ApellidoEmpleado { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [MaxLength(50, ErrorMessage = "El correo no puede exceder los 50 caracteres.")]
        [EmailAddress(ErrorMessage = "El correo no es válido.")]
        public string? Correo { get; set; }

        [MaxLength(50, ErrorMessage = "El nombre de usuario no puede exceder los 50 caracteres.")]
        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        public string? NombreUsuario { get; set; }

        [Required(ErrorMessage = "El rol es obligatorio.")]
        public int? IdRol { get; set; }

        // Lista de roles disponibles para asignar al usuario
        public IEnumerable<RolDTO> Roles { get; set; } = new List<RolDTO>();

        //listado de usuarios
        public IEnumerable<UsuarioRequestDTO> Usuarios { get; set; } = new List<UsuarioRequestDTO>();

        //manero de errores
        public SweetAlertDTO? SweetAlert { get; set; }


        // Propiedades auxiliares
        public bool EsEdicion => Id.HasValue && Id.Value > 0;
        public string TextoBoton => EsEdicion ? "Editar" : "Registrar";
        public string ClaseBoton => EsEdicion ? "btn-warning" : "btn-success";
    }
}
