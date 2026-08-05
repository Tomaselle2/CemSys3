using CemSys3.DTOs.Persona;

namespace CemSys3.DTOs.Concesion
{
    public class HistorialContratoTablaDTO
    {
        public int Id { get; set; }
        public int TramiteId { get; set; }
        public int? Concesion { get; set; }
        public DateTime FechaContrato { get; set; }
        public bool EsRenovacion { get; set; }
        public int TipoParcelaId { get; set; }
        public string NombreSeccion { get; set; } = string.Empty;
        public int NroParcela { get; set; }
        public int NroFila { get; set; }
        public List<PersonaTablaGeneral> Difuntos { get; set; } = new List<PersonaTablaGeneral>();
    }
}
