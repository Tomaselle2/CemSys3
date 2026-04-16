using CemSys3.DTOs.Persona;

namespace CemSys3.DTOs.PlantillaTramite
{
    public class GeneraStrategyDTO
    {
        public int TramiteId;
        public List<TitularesContratoDTO> TitularesActuales = new List<TitularesContratoDTO>();
        public List<TitularesContratoDTO> NuevosTitulares = new List<TitularesContratoDTO>();
        public int UsuarioId;
        public string Parentesco = string.Empty;
        public int NroParcela;
        public int NroFila;
        public string NombreSeccion = string.Empty;
        public string TipoParcela = string.Empty;
    }
}
