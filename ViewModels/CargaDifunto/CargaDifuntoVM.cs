using CemSys3.DTOs.SweetAlert;
using System.ComponentModel.DataAnnotations;

namespace CemSys3.ViewModels.CargaDifunto
{
    public class CargaDifuntoVM
    {
        public int? Id { get; set; }

        [Range(0, 99999999, ErrorMessage = "El DNI no debe tener más de 8 dígitos")]
        public int? Dni { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(50, ErrorMessage = "El nombre no puede superar los 50 caracteres")]
        public string? Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(50, ErrorMessage = "El apellido no puede superar los 50 caracteres")]
        public string? Apellido { get; set; }

        public DateOnly? FechaDefuncion { get; set; }

        [Required(ErrorMessage = "El sexo es obligatorio")]
        public string? Sexo { get; set; }

        [Required(ErrorMessage = "El estado del difunto es obligatorio")]
        public int? EstadoDifuntoId { get; set; }

        [Required(ErrorMessage = "El tipo de parcela es obligatorio")]
        public int? TipoParcelaID { get; set; }

        [Required(ErrorMessage = "La sección es obligatoria")]
        public int? SeccionID { get; set; }

        [Required(ErrorMessage = "La parcela es obligatoria")]
        public int? ParcelaID { get; set; }

        public string? InformacionAdicional { get; set; }

        public DateOnly? FechaNacimiento { get; set; }

        public int? NroActa { get; set; }

        public int? NroFolio { get; set; }

        public int? NroTomo { get; set; }

        public string? NroSerie { get; set; }

        public int? NroAge { get; set; }

        public DateTime? FechaIngreso { get; set; }

        //manero de errores
        public SweetAlertDTO? SweetAlert { get; set; }
    }
}
