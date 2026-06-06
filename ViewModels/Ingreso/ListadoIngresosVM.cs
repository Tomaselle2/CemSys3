using CemSys3.DTOs.Ingreso;
using CemSys3.DTOs.Paginacion;
using CemSys3.DTOs.SweetAlert;

namespace CemSys3.ViewModels.Ingreso
{
    public class ListadoIngresosVM
    {
        public IEnumerable<ListadoIngresosDTO> Ingresos { get; set; } = new List<ListadoIngresosDTO>();
        public int? TipoParcelaID { get; set; }
        public int? SeccionID { get; set; }
        public int? ParcelaID { get; set; }

        public DateOnly? FechaDesde { get; set; }
        public DateOnly? FechaHasta { get; set; }

        //manero de errores
        public SweetAlertDTO? SweetAlert { get; set; }
        public PaginacionDTO Paginacion { get; set; } = new PaginacionDTO();

    }
}
