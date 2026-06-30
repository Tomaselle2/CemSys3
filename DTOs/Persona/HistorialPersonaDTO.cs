using CemSys3.DTOs.Tramite;

namespace CemSys3.DTOs.Persona
{
    public class HistorialPersonaDTO
    {
        public PersonaDTO Persona { get; set; } = new PersonaDTO();
        public IEnumerable<TramiteDTO> Tramites { get; set; } = new List<TramiteDTO>();
        public IEnumerable<DifuntoHistorialParcelaDTO> Parcelas { get; set; } = new List<DifuntoHistorialParcelaDTO>();
        public IEnumerable<DTO_ConcesionTitular> ConecesionesActivasTitular { get; set; } = new List<DTO_ConcesionTitular>();
    }
}
