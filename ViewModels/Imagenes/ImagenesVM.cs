using CemSys3.DTOs.SweetAlert;

namespace CemSys3.ViewModels.Imagenes
{
    public class ImagenesVM
    {
       public string nombreIntendente { get; set; } = string.Empty;
        //manero de errores
        public SweetAlertDTO? SweetAlert { get; set; }
    }
}
