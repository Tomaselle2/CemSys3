using CemSys3.DTOs.SweetAlert;
using System.ComponentModel.DataAnnotations;

namespace CemSys3.ViewModels.Login
{
    public class LoginVM
    {
        [Required(ErrorMessage = "El usuario es obligatorio")]
        public string? NombreUsuario { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        public string? Clave { get; set; }

        public SweetAlertDTO sweetAlertDTO { get; set; } = new SweetAlertDTO();
    }
}
