using CemSys3.DTOs.PlantillaTramite;
using CemSys3.DTOs.SweetAlert;
using CemSys3.DTOs.Tarea;

namespace CemSys3.ViewModels.PlantillaTramite
{
    public class PlantillaTramiteVM
    {
        public PlantillaTramiteDTO Dto { get; set; } = new PlantillaTramiteDTO();
        public List<TareaDTO> Tareas { get; set; } = new List<TareaDTO>();
        public string vista { get; set; } = string.Empty;
        //manero de errores
        public SweetAlertDTO? SweetAlert { get; set; }
    }
}
