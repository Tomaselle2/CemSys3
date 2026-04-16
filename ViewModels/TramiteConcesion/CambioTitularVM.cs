using CemSys3.DTOs.Archivo;
using CemSys3.DTOs.HistorialEstado;
using CemSys3.DTOs.Persona;
using CemSys3.DTOs.PlantillaTramite;
using CemSys3.DTOs.SweetAlert;
using CemSys3.DTOs.TramitesConcesion;
//using CemSys3.DTOs.TramiteConcesion;

namespace CemSys3.ViewModels.TramiteConcesion
{
    public class CambioTitularVM
    {
        public int TramiteId { get; set; }

        public int concesionId { get; set; }

        public CambioTitularDTO Dto { get; set; } = new();

        public List<TitularesContratoDTO> Personas { get; set; } = new(); //nuevos titulares

        public IEnumerable<ArchivoDTO> Archivos { get; set; } = new List<ArchivoDTO>();
        public IEnumerable<HistorialEstadosDTO> Historial { get; set; } = new List<HistorialEstadosDTO>();

        public List<DocumentoDTO> Documentos { get; set; } = new();

        public bool Generado => Documentos.Any();

        public SweetAlertDTO? SweetAlert { get; set; }

    }
}
