using System.ComponentModel.DataAnnotations;

namespace CemSys3.DTOs.Persona
{
    public class TitularesContratoDTO
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "El DNI es obligatorio")]
        public string? Dni { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string? Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio")]
        public string? Apellido { get; set; }

        [Required(ErrorMessage = "El sexo es obligatorio")]
        public string? Sexo { get; set; }

        public string? Celular { get; set; }
        public string? CorreoElectronico { get; set; }

        [Required(ErrorMessage = "El domicilio es obligatorio")]
        public string? Domicilio { get; set; }
    }
}
