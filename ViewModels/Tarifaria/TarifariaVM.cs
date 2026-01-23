using CemSys3.DTOs.Seccion;
using CemSys3.DTOs.SweetAlert;
using CemSys3.DTOs.Tarifaria;

namespace CemSys3.ViewModels.Tarifaria
{
    public class TarifariaVM
    {
        public IEnumerable<TarifariaRequestDTO> ListadoPrecios {  get; set; } = new List<TarifariaRequestDTO>();

        public IEnumerable<SeccionNichoTarifariaDTO> ListadoSeccionesNicho { get; set; } = new List<SeccionNichoTarifariaDTO>();

        //manero de errores
        public SweetAlertDTO? SweetAlert { get; set; }
    }
}
