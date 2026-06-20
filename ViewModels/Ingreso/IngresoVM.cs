using CemSys3.DTOs.EmpresaSepelio;
using CemSys3.DTOs.Nota;
using CemSys3.DTOs.SweetAlert;
using CemSys3.DTOs.Usuario;
using System.ComponentModel.DataAnnotations;

namespace CemSys3.ViewModels.Ingreso
{
    public class IngresoVM
    {
        public NotaDTO NotaIngreso { get; set; } = new NotaDTO();

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

        [Required(ErrorMessage = "El empleado es obligatorio")]
        public int? EmpleadoID { get; set; }

        public int? EmpresaFunebreID { get; set; }

        [Required(ErrorMessage = "La fecha y hora de ingreso es obligatoria")]
        public DateTime? FechaHoraIngreso { get; set; }

        public bool IngresoTitularFallecido { get; set; }



        // Empleado confirmó que es la misma persona que ya estuvo y fue retirada -> reingreso
        public bool ReingresoConfirmado { get; set; }

        // Empleado confirmó que, pese a la coincidencia de nombre/apellido, es OTRA persona
        public bool EsPersonaDistinta { get; set; }

        // Id de la persona encontrada como coincidencia (titular o reingreso)
        public int? PersonaCoincidenciaId { get; set; }



        public string? InformacionAdicional { get; set; }

        public DateOnly? FechaNacimiento { get; set; }

        public string? NombreEmpresa { get; set; }

        public int? NroActa { get; set; }

        public int? NroFolio { get; set; }

        public int? NroTomo { get; set; }

        public string? NroSerie { get; set; }

        public int? NroAge { get; set; }

        public IEnumerable<UsuarioRequestDTO> ListaEmpleados { get; set; } = new List<UsuarioRequestDTO>();
        public IEnumerable<EmpresaSepelioRequestDTO> ListaEmpresasSepelio { get; set; } = new List<EmpresaSepelioRequestDTO>();

        //manero de errores
        public SweetAlertDTO? SweetAlert { get; set; }
    }
}
