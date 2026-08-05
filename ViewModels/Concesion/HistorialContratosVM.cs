using CemSys3.DTOs.Concesion;
using CemSys3.DTOs.Paginacion;
using CemSys3.DTOs.SweetAlert;

namespace CemSys3.ViewModels.Concesion
{
    public class HistorialContratosVM
    {
        public DateOnly? FechaDesde { get; set; }
        public DateOnly? FechaHasta { get; set; }

        public IEnumerable<HistorialContratoTablaDTO> Listado = new List<HistorialContratoTablaDTO>();

        public SweetAlertDTO? SweetAlert { get; set; }
        public PaginacionDTO Paginacion { get; set; } = new PaginacionDTO();
    }
}
