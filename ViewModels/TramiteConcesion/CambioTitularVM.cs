using CemSys3.DTOs.PlantillaTramite;
using CemSys3.DTOs.SweetAlert;
using CemSys3.DTOs.TramiteConcesion;

namespace CemSys3.ViewModels.TramiteConcesion
{
    public class CambioTitularVM
    {
        public PlantillaTramiteDTO PlantillaTramite { get; set; } = new PlantillaTramiteDTO();

        public CambioTitularDTO Dto { get; set; } = new CambioTitularDTO();

        //manero de errores
        public SweetAlertDTO? SweetAlert { get; set; }
    }
}
