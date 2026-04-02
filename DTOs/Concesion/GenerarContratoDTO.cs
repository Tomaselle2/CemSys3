using CemSys3.DTOs.Persona;
using CemSys3.DTOs.Tarifaria;

namespace CemSys3.DTOs.Concesion
{
    public class GenerarContratoDTO
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
        public List<DifuntoContratoDTO> Difuntos { get; set; } = new List<DifuntoContratoDTO>();
        public List<TitularesContratoDTO> Titulares { get; set; } = new List<TitularesContratoDTO>();
        public List<PrecioTarifariaDTO> PreciosNichos { get; set; } = new List<PrecioTarifariaDTO>();
        public List<PrecioTarifariaDTO> PreciosFosas { get; set; } = new List<PrecioTarifariaDTO>();


        public string baseUrl = string.Empty;
        public string PrecioEnLetras = string.Empty;
        public string formaPago { get; set; } = string.Empty;
        public int? CuotaId { get; set; }
        public decimal Precio { get; set; }
        public string? OtraFormaPago { get; set; }
        public int CantidadAniosId { get; set; }
        public DateOnly Vencimiento { get; set; }
        public DateTime fechaGeneracion { get; set; }

        public decimal PorcentajeDescuentoRenovacionConcesionAlDia { get; set; }
        public decimal PorcentajeAumentoConcesionesOtrasLocalidades { get; set; }
        public decimal PorcentajeFondoAyudaCentroSalud { get; set; }

        public string NombreIntendente { get; set; } = string.Empty;
        public bool EsRenovacion {  get; set; } = false;


    }
}
