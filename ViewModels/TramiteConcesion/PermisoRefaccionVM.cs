using CemSys3.DTOs.Archivo;
using CemSys3.DTOs.HistorialEstado;
using CemSys3.DTOs.PlantillaTramite;
using CemSys3.DTOs.SweetAlert;
using CemSys3.DTOs.Tarea;
using CemSys3.DTOs.TramitesConcesion;
using CemSys3.DTOs.TramitesConcesion.PermisoRefaccion;

namespace CemSys3.ViewModels.TramiteConcesion
{
    public class PermisoRefaccionVM
    {
        public int TramiteId { get; set; }

        public int concesionId { get; set; }

        public int TipoTramiteId { get; set; }

        public int DestinoCementerioId { get; set; }

        //public int ParcelaID { get; set; } //nueva parcelaID
        public PermisoRefaccionDTO Dto { get; set; } = new();

        public IEnumerable<ArchivoDTO> Archivos { get; set; } = new List<ArchivoDTO>();
        public IEnumerable<HistorialEstadosDTO> Historial { get; set; } = new List<HistorialEstadosDTO>();
        public List<FirmantesDTO> Firmantes { get; set; } = new List<FirmantesDTO>();
        public List<FirmantesDTO> Personas { get; set; } = new List<FirmantesDTO>(); //nuevos firmantes

        public List<TareaDTO> Tareas { get; set; } = new List<TareaDTO>();

        public List<DocumentoDTO> Documentos { get; set; } = new();

        public bool Generado => Documentos.Any();

        public SweetAlertDTO? SweetAlert { get; set; }
    }
}
