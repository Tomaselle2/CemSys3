using CemSys3.DTOs.Persona;
using CemSys3.DTOs.SweetAlert;
using CemSys3.DTOs.Tarifaria;

namespace CemSys3.ViewModels.Concesion
{
    public class ConcesionVM
    {
        public int TramiteId { get; set; }
        public int EstadoTramiteId { get; set; }
        public int ParcelaId { get; set; }
        public string? TipoParcela { get; set; }
        public int SeccionId { get; set; }
        public string NombreSeccion { get; set; } = string.Empty;
        public int NroParcela { get; set; }
        public int NroFila { get; set; }
        public int? NroConcesion { get; set; }
        public DateOnly Vencimiento { get; set; }
        public List<DifuntoContratoDTO> Difuntos { get; set; } = new List<DifuntoContratoDTO>();
        public List<TitularesContratoDTO> Titulares { get; set; } = new List<TitularesContratoDTO>();

        //manero de errores
        public SweetAlertDTO? SweetAlert { get; set; }

    }
}
