using CemSys3.DTOs.SweetAlert;
using CemSys3.DTOs.Tramite;

namespace CemSys3.ViewModels.TramiteConcesion
{
    public class ListadoTramitesVM
    {
        public ListadoTramitesDeConcesionDTO Dto { get; set; } = new ListadoTramitesDeConcesionDTO();

        //manero de errores
        public SweetAlertDTO? SweetAlert { get; set; }
    }
}
