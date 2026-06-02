using CemSys3.DTOs.Concesion;
using CemSys3.DTOs.Paginacion;
using CemSys3.DTOs.SweetAlert;

namespace CemSys3.ViewModels.Concesion
{
    public class TablaGeneralVM
    {
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public int? Concesion { get; set; }

        public IEnumerable<TablaConcesionDTO> Listado = new List<TablaConcesionDTO>();

        //manero de errores
        public SweetAlertDTO? SweetAlert { get; set; }
        public PaginacionDTO Paginacion { get; set; } = new PaginacionDTO();
    }
}
