using CemSys3.DTOs.Estadistica;
using CemSys3.DTOs.SweetAlert;

namespace CemSys3.ViewModels.Estadistica
{
    public class EstadisticasVM
    {
        public EstadisticasDTO Estadisticas { get; set; } = new EstadisticasDTO();
        public SweetAlertDTO? SweetAlert { get; set; }
    }
}
