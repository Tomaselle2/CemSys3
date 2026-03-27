using CemSys3.DTOs.Concesion;
using CemSys3.DTOs.SweetAlert;

namespace CemSys3.ViewModels.Concesion
{
    public class ModificarConcesionVM
    {
        public ModificarDatosConcesionDTO Dto { get; set; } = new ModificarDatosConcesionDTO();

        //manero de errores
        public SweetAlertDTO? SweetAlert { get; set; }
    }
}
