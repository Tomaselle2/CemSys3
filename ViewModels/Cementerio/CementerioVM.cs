using CemSys3.DTOs.Cementerio;
using CemSys3.DTOs.Paginacion;
using CemSys3.DTOs.SweetAlert;
using System.ComponentModel.DataAnnotations;

namespace CemSys3.ViewModels.Cementerio
{
    public class CementerioVM
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [MaxLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres.")]
        public string? Nombre { get; set; }

        //listado de empresas
        public IEnumerable<CementerioRequestDTO> ListadoCementerios { get; set; } = new List<CementerioRequestDTO>();

        //manero de errores
        public SweetAlertDTO? SweetAlert { get; set; }

        public PaginacionDTO Paginacion { get; set; } = new PaginacionDTO();

        // Propiedades auxiliares
        public bool EsEdicion => Id.HasValue && Id.Value > 0;
        public string TextoBoton => EsEdicion ? "Editar" : "Registrar";
        public string ClaseBoton => EsEdicion ? "btn-warning" : "btn-success";
    }
}

