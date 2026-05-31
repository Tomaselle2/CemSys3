using CemSys3.DTOs.SweetAlert;
using System.ComponentModel.DataAnnotations;

namespace CemSys3.ViewModels.Usuario
{
    public class CambiarContraseniaUsuarioVM
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "El campo es obligatorio")]
        [StringLength(300, ErrorMessage = "La contraseña no puede superar los 300 caracteres")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{6,}$",
         ErrorMessage = "La contraseña debe tener al menos 6 caracteres, una mayúscula, un número y un carácter especial.")]
        public string? ClaveNueva { get; set; }

        [Required(ErrorMessage = "El campo es obligatorio")]
        [StringLength(300, ErrorMessage = "La contraseña no puede superar los 300 caracteres")]
        public string? ClaveAnterior { get; set; }

        public SweetAlertDTO? SweetAlert { get; set; }

    }
}
