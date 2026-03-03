using CemSys3.DTOs.Nota;
using CemSys3.DTOs.SweetAlert;

namespace CemSys3.ViewModels.Home
{
    public class NotificacionHomeVM
    {
        public NotificacionNotaDTO NotificacionNota { get; set; } = new NotificacionNotaDTO();

        //manero de errores
        public SweetAlertDTO? SweetAlert { get; set; }
    }
}
