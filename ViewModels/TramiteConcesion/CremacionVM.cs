using CemSys3.DTOs.Archivo;
using CemSys3.DTOs.HistorialEstado;
using CemSys3.DTOs.Persona;
using CemSys3.DTOs.PlantillaTramite;
using CemSys3.DTOs.SweetAlert;
using CemSys3.DTOs.Tarea;
using CemSys3.DTOs.TramitesConcesion;
using CemSys3.DTOs.TramitesConcesion.Cremacion;

namespace CemSys3.ViewModels.TramiteConcesion
{
    public class CremacionVM
    {
        public int TramiteId { get; set; }

        public int concesionId { get; set; }

        public int TipoTramiteId { get; set; }

        public CremacionDTO Dto { get; set; } = new();
        /*public List<TitularesContratoDTO> Personas { get; set; } = new();*/ //nuevos titulares

        public IEnumerable<ArchivoDTO> Archivos { get; set; } = new List<ArchivoDTO>();
        public IEnumerable<HistorialEstadosDTO> Historial { get; set; } = new List<HistorialEstadosDTO>();
        public IEnumerable<FirmantesDTO> Firmantes { get; set; } = new List<FirmantesDTO>();
        public List<TareaDTO> Tareas { get; set; } = new List<TareaDTO>();


        public List<DocumentoDTO> Documentos { get; set; } = new();

        public bool Generado => Documentos.Any();

        public SweetAlertDTO? SweetAlert { get; set; }
    }
}
