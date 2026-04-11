using CemSys3.DTOs.TramiteConcesion;

namespace CemSys3.Interfaces.TramiteConcesion
{
    public interface ICambioTitular
    {
        Task<CambioTitularDTO> AddCambioTitular(int tramiteConcesionId, int usuarioId); //get
        Task<CambioTitularDTO> Get(int cambioTitularId, int concesionId); //get id del tramite de cambio de titular

        Task<int> CambioTitularPost(CambioTitularDTO dto); //post
    }
}
