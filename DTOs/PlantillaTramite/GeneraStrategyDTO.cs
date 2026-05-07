using CemSys3.DTOs.Persona;
using CemSys3.DTOs.TramitesConcesion;

namespace CemSys3.DTOs.PlantillaTramite
{
    public class GeneraStrategyDTO
    {
        public int TramiteId;
        public int NroConcesion;
        public List<TitularesContratoDTO> TitularesActuales = new List<TitularesContratoDTO>();
        public List<TitularesContratoDTO> NuevosTitulares = new List<TitularesContratoDTO>();
        public List<DifuntoContratoDTO> Difuntos { get; set; } = new List<DifuntoContratoDTO>();


        public List<FirmantesDTO> Firmantes { get; set; } = new List<FirmantesDTO>();
        public int FirmanteId { get; set; }
        public int TipoAutorizacionId { get; set; }

        public int UsuarioId;
        public string Parentesco = string.Empty;
        public int NroParcela;
        public int NroFila;
        public string NombreSeccion = string.Empty;
        public string TipoParcela = string.Empty;
        public int CementerioId;
    }
}
