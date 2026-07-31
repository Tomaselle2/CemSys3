using CemSys3.DTOs.SweetAlert;
using CemSys3.DTOs.Tramite;

namespace CemSys3.ViewModels.Tramite
{
    public class TramiteVM
    {
        public IEnumerable<TramiteDTO> Tramites = new List<TramiteDTO>();
        public SweetAlertDTO? SweetAlert { get; set; }

        public TramiteFiltroDTO Filtro { get; set; } = new TramiteFiltroDTO();
        public string? MensajeTipoNoDisponible { get; set; }

    }
}
