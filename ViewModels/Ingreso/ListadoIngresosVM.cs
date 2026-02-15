using CemSys3.DTOs.Ingreso;
using CemSys3.DTOs.Paginacion;
using CemSys3.DTOs.SweetAlert;
using iText.Layout.Element;

namespace CemSys3.ViewModels.Ingreso
{
    public class ListadoIngresosVM
    {
        public IEnumerable<ListadoIngresosDTO> Ingresos { get; set; } = new List<ListadoIngresosDTO>();

        //manero de errores
        public SweetAlertDTO? SweetAlert { get; set; }
        public PaginacionDTO Paginacion { get; set; } = new PaginacionDTO();

    }
}
