using CemSys3.DTOs.Concesion;
using CemSys3.DTOs.SweetAlert;

namespace CemSys3.ViewModels.Concesion
{
    public class HistorialConcesionVM
    {
        public int TramiteId { get; set; }
        public int NroConcesion { get; set; }
        public IEnumerable<HistorialTitularesDTO> Titulares { get; set; } = new List<HistorialTitularesDTO>();
        //manero de errores
        public SweetAlertDTO? SweetAlert { get; set; }
    }
}
