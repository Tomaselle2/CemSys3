using CemSys3.DTOs.Archivo;
using CemSys3.DTOs.HistorialEstado;
using CemSys3.DTOs.PlantillaTramite;
using CemSys3.DTOs.SweetAlert;
using CemSys3.DTOs.TramiteConcesion;

namespace CemSys3.ViewModels.TramiteConcesion
{
    public class CambioTitularVM
    {
        public PlantillaTramiteDTO PlantillaTramite { get; set; } = new PlantillaTramiteDTO();

        public CambioTitularDTO Dto { get; set; } = new CambioTitularDTO();
        public IEnumerable<ArchivoDTO> Archivos { get; set; } = new List<ArchivoDTO>();
        public IEnumerable<HistorialEstadosDTO> Historial { get; set; } = new List<HistorialEstadosDTO>();

        //manero de errores
        public SweetAlertDTO? SweetAlert { get; set; }
    }
}
