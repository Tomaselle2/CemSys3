using CemSys3.DTOs.Archivo;
using CemSys3.DTOs.Concesion;
using CemSys3.DTOs.SweetAlert;

namespace CemSys3.ViewModels.Concesion
{
    public class ArchivosConcesionVM
    {
        public int TramiteId { get; set; }
        public InfoGeneralDTO Dto { get; set; } = new InfoGeneralDTO();
        public IEnumerable<ArchivoDTO> Archivos { get; set; } = new List<ArchivoDTO>();

        //manero de errores
        public SweetAlertDTO? SweetAlert { get; set; }
    }
}
