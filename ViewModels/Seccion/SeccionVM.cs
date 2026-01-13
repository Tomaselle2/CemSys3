using CemSys3.DTOs.Paginacion;
using CemSys3.DTOs.Seccion;
using CemSys3.DTOs.SweetAlert;
using System.ComponentModel.DataAnnotations;

namespace CemSys3.ViewModels.Seccion
{
    public class SeccionVM
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [MaxLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El número de parcelas es obligatorio")]
        public int? NroParcelas { get; set; }

        [Required(ErrorMessage = "El número de filas es obligatorio")]
        public int? Filas { get; set; }

        [Required(ErrorMessage = "El tipo de numeración de los nichos es obligatorio")]
        public int TipoNumeracionParcelaId { get; set; }

        public int TipoParcelaId { get; set; }

        public IEnumerable<SeccionRequestDTO> ListadoSecciones { get; set; } = new List<SeccionRequestDTO>();

        //alertas
        public SweetAlertDTO? SweetAlert { get; set; }

        public PaginacionDTO Paginacion { get; set; } = new PaginacionDTO();

        // Propiedades auxiliares
        public bool EsEdicion => Id.HasValue && Id.Value > 0;
        public string TextoBoton => EsEdicion ? "Editar" : "Registrar";
        public string ClaseBoton => EsEdicion ? "btn-warning" : "btn-success";
    }
}
