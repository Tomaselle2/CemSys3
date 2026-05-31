using System.ComponentModel.DataAnnotations;

namespace CemSys3.DTOs.TramitesConcesion
{
    public class FirmantesDTO
    {
        public int Id { get; set; }

        public int TramiteId { get; set; }

        public int PersonaId { get; set; }

        public string? Parentesco { get; set; }

        public bool EsTitular { get; set; }

        public DateTime? FechaAlta { get; set; }

        public bool? Visibilidad { get; set; }


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
