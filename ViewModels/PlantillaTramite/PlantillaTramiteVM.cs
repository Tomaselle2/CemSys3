using CemSys3.DTOs.PlantillaTramite;
using CemSys3.DTOs.SweetAlert;

namespace CemSys3.ViewModels.PlantillaTramite
{
    public class PlantillaTramiteVM
    {
        public PlantillaTramiteDTO Dto { get; set; } = new PlantillaTramiteDTO();
        public string vista { get; set; } = string.Empty;
        //manero de errores
        public SweetAlertDTO? SweetAlert { get; set; }
    }
}
