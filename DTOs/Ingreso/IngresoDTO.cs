using CemSys3.DTOs.Persona;
namespace CemSys3.DTOs.Ingreso
{
    public class IngresoDTO
    {
        public int TramiteId { get; set; }

        public bool Visibilidad { get; set; }

        public DateTime? FechaIngreso { get; set; }

        public int UsuarioLogueadoId { get; set; }
        public int EmpleadoIngresoId { get; set; }

        public int? EmpresaFunebreId { get; set; }

        public int ParcelaId { get; set; }

        public int DifuntoId { get; set; }

        public int EstadoDifuntoId { get; set; }

        public string? InformacionAdicional { get; set; }

        public PersonaDTO Difunto { get; set; } = new PersonaDTO();
    }
}
