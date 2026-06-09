using CemSys3.DTOs.Concesion;
using CemSys3.DTOs.Persona;
using CemSys3.DTOs.SweetAlert;
using CemSys3.DTOs.Tramite;

namespace CemSys3.ViewModels.Concesion
{
    public class HistorialConcesionVM
    {
        public int TramiteId { get; set; }
        public int NroConcesion { get; set; }
        public IEnumerable<HistorialTitularesDTO> Titulares { get; set; } = new List<HistorialTitularesDTO>();
        public IEnumerable<TramiteDTO> Tramites { get; set; } = new List<TramiteDTO>();

        public IEnumerable<DifuntoConcesionDTO> Difuntos { get; set; } = new List<DifuntoConcesionDTO>();
        public InfoGeneralDTO Dto { get; set; } = new InfoGeneralDTO();

        //manero de errores
        public SweetAlertDTO? SweetAlert { get; set; }
    }
}
