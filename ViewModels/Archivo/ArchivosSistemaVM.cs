using CemSys3.DTOs.Archivo;
using CemSys3.DTOs.SweetAlert;

namespace CemSys3.ViewModels.Archivo
{
    public class ArchivosSistemaVM
    {
        public IEnumerable<ArchivoDTO> Archivos { get; set; } = new List<ArchivoDTO>();

        //manero de errores
        public SweetAlertDTO? SweetAlert { get; set; }
    }
}
