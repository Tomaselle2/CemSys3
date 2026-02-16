using CemSys3.DTOs.Persona;

namespace CemSys3.DTOs.Concesion
{
    public class TablaConcesionDTO
    {
        public int TramiteId { get; set; }

        public int? Concesion { get; set; }

        public bool? Visibilidad { get; set; }

        public int? TipoParcelaId { get; set; }

        public DateOnly? Vencimiento { get; set; }

        public int EstadoTramiteId { get; set; }

        public string NombreSeccion { get; set; } = string.Empty;
        public int NroParcela { get; set; }
        public int NroFila { get; set; }


        public List<PersonaTablaGeneral> Titulares { get; set; } = new List<PersonaTablaGeneral>();
        public List<PersonaTablaGeneral> Difuntos { get; set; } = new List<PersonaTablaGeneral>();

    }
}
