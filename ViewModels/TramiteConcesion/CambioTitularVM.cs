using CemSys3.DTOs.Archivo;
using CemSys3.DTOs.HistorialEstado;
using CemSys3.DTOs.Persona;
using CemSys3.DTOs.PlantillaTramite;
using CemSys3.DTOs.SweetAlert;
//using CemSys3.DTOs.TramiteConcesion;

namespace CemSys3.ViewModels.TramiteConcesion
{
    public class CambioTitularVM
    {
        //public PlantillaTramiteDTO PlantillaTramite { get; set; } = new PlantillaTramiteDTO();
        public int TramiteId { get; set; }

        //public CambioTitularDTO Dto { get; set; } = new CambioTitularDTO();
        public List<TitularesContratoDTO> Personas { get; set; } = new();

        public IEnumerable<ArchivoDTO> Archivos { get; set; } = new List<ArchivoDTO>();
        public IEnumerable<HistorialEstadosDTO> Historial { get; set; } = new List<HistorialEstadosDTO>();
        //public List<PlantillaTramiteDTO> Plantillas { get; set; } = new();

        public List<DocumentoDTO> Documentos { get; set; } = new();

        public bool Generado { get; set; } // si ya se generaron autorizaciones

        //manero de errores
        public SweetAlertDTO? SweetAlert { get; set; }
    }
}
