using CemSys3.DTOs.Persona;

namespace CemSys3.DTOs.TramiteConcesion
{
    public class CambioTitularDTO
    {
        public int TramiteId { get; set; }
        public int EstadoTramiteId { get; set; }
        public int? NroConcesion { get; set; }

        public int ParcelaId { get; set; }
        public string? TipoParcela { get; set; }
        public string NombreSeccion { get; set; } = string.Empty;
        public int NroParcela { get; set; }
        public int NroFila { get; set; }

        public List<TitularesContratoDTO> TitularesActuales { get; set; } = new List<TitularesContratoDTO>(); //no se modifican, vienen de BD

        public List<TitularesContratoDTO> NuevosTitulares { get; set; } = new List<TitularesContratoDTO>(); 

        public List<PersonaDTO>? NuevosTitularesPost { get; set; } //solo para el post pasan a metodo de modificar.
    }
}
