using CemSys3.DTOs.SweetAlert;

namespace CemSys3.ViewModels.Concesion
{
    public class CalculoDeudaVM
    {
        public int TramiteId { get; set; }
        public string MensajeDeuda { get; set; } = string.Empty;
        //manero de errores
        public SweetAlertDTO? SweetAlert { get; set; }
    }
}
