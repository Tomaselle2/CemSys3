using CemSys3.DTOs.Archivo;
using CemSys3.DTOs.HistorialEstado;
using CemSys3.DTOs.PlantillaTramite;
using CemSys3.DTOs.SweetAlert;
using CemSys3.DTOs.Tarea;
using CemSys3.DTOs.TramitesConcesion;
using CemSys3.DTOs.TramitesConcesion.CambioTitular;
using CemSys3.DTOs.TramitesConcesion.PermisoIngreso;

namespace CemSys3.ViewModels.PermisoIngreso
{
    public class PermisoIngresoVM
    {
        public int TramiteId { get; set; }

        public int concesionId { get; set; }

        public int TipoTramiteId { get; set; }

        public PermisoIngresoDTO Dto { get; set; } = new();
        public List<FirmantesDTO> Personas { get; set; } = new(); //nuevos titulares
        public List<FirmantesDTO> Firmantes { get; set; } = new List<FirmantesDTO>();
        public IEnumerable<ArchivoDTO> Archivos { get; set; } = new List<ArchivoDTO>();
        public IEnumerable<HistorialEstadosDTO> Historial { get; set; } = new List<HistorialEstadosDTO>();
        public List<TareaDTO> Tareas { get; set; } = new List<TareaDTO>();

        public string NombreDifunto { get; set; } = string.Empty;
        public List<DocumentoDTO> Documentos { get; set; } = new();

        public bool Generado => Documentos.Any();

        public SweetAlertDTO? SweetAlert { get; set; }
    } 
}
