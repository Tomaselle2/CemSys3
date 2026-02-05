using CemSys3.DTOs.Nota;
using CemSys3.DTOs.Persona;

namespace CemSys3.DTOs.Ingreso
{
    public class ResumenIngresoDTO
    {
        public int TramiteId { get; set; }

        public bool Visibilidad { get; set; }

        public DateTime? FechaIngreso { get; set; }

        public int UsuarioLogueadoId { get; set; }
        public string UsuarioLogueadoNombre { get; set; } = string.Empty;
        public int EmpleadoIngresoId { get; set; }
        public string EmpleadoIngresoNombre { get; set; } = string.Empty;

        public int? EmpresaFunebreId { get; set; }
        public string? EmpresaFunebreNombre { get; set; }

        public int ParcelaId { get; set; }
        public int NroParcela { get; set; }
        public string SeccionNombre { get; set; } = string.Empty;
        public int NroFila { get; set; }

        public int DifuntoId { get; set; }

        public int EstadoDifuntoId { get; set; }
        public string EstadoDifuntoNombre { get; set; } = string.Empty;

        public string? InformacionAdicional { get; set; }

        public NotaDTO Nota { get; set; } = new NotaDTO();
        public PersonaDTO Difunto { get; set; } = new PersonaDTO();
    }
}
