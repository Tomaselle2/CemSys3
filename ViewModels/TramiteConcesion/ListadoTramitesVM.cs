using CemSys3.DTOs.Concesion;
using CemSys3.DTOs.SweetAlert;
using CemSys3.DTOs.Tramite;

namespace CemSys3.ViewModels.TramiteConcesion
{
    public class ListadoTramitesVM
    {
        public ListadoTramitesDeConcesionDTO Dto { get; set; } = new ListadoTramitesDeConcesionDTO();
        public InfoGeneralDTO InfoGeneral { get; set; } = new(); //para info de concesion


        //manero de errores
        public SweetAlertDTO? SweetAlert { get; set; }
    }
}
