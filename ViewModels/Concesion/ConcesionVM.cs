using CemSys3.DTOs.Concesion;
using CemSys3.DTOs.HistorialEstado;
using CemSys3.DTOs.Persona;
using CemSys3.DTOs.SweetAlert;
using CemSys3.DTOs.Tarifaria;

namespace CemSys3.ViewModels.Concesion
{
    public class ConcesionVM
    {
        public InfoGeneralDTO Dto { get; set; } = new InfoGeneralDTO();
        public IEnumerable<HistorialEstadosDTO> historial = new List<HistorialEstadosDTO>();
        //manero de errores
        public SweetAlertDTO? SweetAlert { get; set; }

    }
}
