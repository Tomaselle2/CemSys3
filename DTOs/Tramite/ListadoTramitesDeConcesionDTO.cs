using AspNetCoreGeneratedDocument;
using CemSys3.DTOs.Persona;

namespace CemSys3.DTOs.Tramite
{
    public class ListadoTramitesDeConcesionDTO
    {
        public int ConcesionId { get; set; }
        public int ParcelaId { get; set; }
        public IEnumerable<RequisitosTramiteDTO> Requisitos { get; set; } = new List<RequisitosTramiteDTO>();

        public IEnumerable<TramiteDTO> TramitesIniciados { get; set; } = new List<TramiteDTO>();

        public List<DifuntoContratoDTO> Difuntos { get; set; } = new List<DifuntoContratoDTO>();

    }
}
