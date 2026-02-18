using CemSys3.DTOs.Parcela;
using CemSys3.DTOs.SweetAlert;

namespace CemSys3.ViewModels.Parcela
{
    public class ParcelaHistorialVM
    {
        public ParcelaHistorialDTO Historial { get; set; } = new ParcelaHistorialDTO();

        //alertas
        public SweetAlertDTO? SweetAlert { get; set; }
    }
}
